// Copyright (c) 2014, Lee
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#include "cntsocket.h"
#include "utility.h"
#include "protocol.h"
#include "db.h"
#include "room.h"
#include "logger.h"

extern db *g_db;

extern mutex g_room_mutex;
extern mutex g_clients_mutex;
extern room_list g_room_list;

namespace
{
	room::id_type create_room_id()
	{
		random_device rd;
		mt19937 mt(rd());
		uniform_int_distribution<room::id_type> dist(0, numeric_limits<room::id_type>::max());
		room::id_type id = dist(mt);

		{
			//lock_guard<mutex> lg(g_room_mutex);

			while (true)
			{
				if (find_if(g_room_list.begin(), g_room_list.end(), [id](const room &r)
				{
					return r.id == id;
				}) != g_room_list.end())
				{
					id = dist(mt);
					continue;
				}
				else
				{
					return id;
				}
			}
		}
	}

	void broad_cast(const packet_type *header, const packet &pack, bool lock = true)
	{
		if (lock)
		{
			lock_guard<mutex> lg(g_clients_mutex);

			for (auto client : g_clients)
			{
				client->send_packet(header, pack);
			}
		}
		else
		{
			for (auto client : g_clients)
			{
				client->send_packet(header, pack);
			}
		}
	}

	room_list::iterator find_room(room::id_type id)
	{
		auto it = find_if(g_room_list.begin(), g_room_list.end(), [id](const room &r)
		{
			return r.id == id;
		});

		return it;
	}
}

cnt_socket::cnt_socket(tcp::socket *client, const tcp::endpoint &epnt) : m_socket(client), m_epnt(epnt), m_client_state({ false, false }), m_no_need_join(false)
{

}

cnt_socket::~cnt_socket()
{
	for (auto it = g_room_list.begin(); it != g_room_list.end();)
	{
		auto mem = find_if(it->members.begin(), it->members.end(), [this](const member &m)
		{
			return m.nick == m_client_state.nick;
		});

		if (mem != it->members.end())
		{
			it->broad_cast(PROTOCOL_REMOVE_CLIENT, FLAG_SUCCESSED + to_string(it->id) + '\n' + m_client_state.nick);
			it->members.erase(mem);
		}

		// If the master is exited
		if (it->master.compare(m_client_state.nick) == 0)
		{
			broad_cast(PROTOCOL_REMOVE_ROOM, FLAG_SUCCESSED + to_string(it->id), false);
			g_room_list.erase(it++);
			continue;
		}

		++it;
	}

	m_socket->close();
	m_socket.reset();

	// 	if (m_no_need_join == false)
	// 		if (m_recv_thrd.joinable())
	// 			m_recv_thrd.join();
	m_recv_thrd.detach();

	stringstream ss;
	ss << "Client disconnected. ip = " << m_epnt.address().to_string() << " port = "
		<< m_epnt.port();
	LOGGING(ss.str());
}

void cnt_socket::recv_func()
{
	vector<packet_type> buf(MAX_BYTES_NUMBER);
	vector<packet_type> real_buf(MAX_BYTES_NUMBER + 1);
	size_t i = 0, j = 0, packet_size = 0;

	while (true)
	{
		size_t read_bytes = m_socket->read_some(buffer(buf, MAX_BYTES_NUMBER), m_latest_error);

		// When server is closing
		if (m_latest_error == asio::error::connection_aborted)
			return;

		if ((m_latest_error == asio::error::connection_reset) ||
			(m_latest_error == asio::error::eof))
		{
			//detach_thread();
			goto delete_client;
		}

		if (read_bytes <= 0)
		{
			//detach_thread();
			goto delete_client;
		}

		buf.erase(buf.begin() + read_bytes, buf.end());

		for (i = 0; i < read_bytes; i++)
		{
			real_buf[j] = buf[i];
			packet_size++;

			if (buf[i] == EOP)
			{
				if (packet_processor(packet(real_buf.begin(), real_buf.begin() + packet_size - 1)) == false)
				{
					goto delete_client;
				}
				j = 0;
				packet_size = 0;
			}
			else
			{
				j++;
			}
		}
		if (j != 0)
		{
			real_buf[j] = EOP;
			if (packet_processor(packet(real_buf.begin(), real_buf.begin() + j - 1)) == false)
			{
				goto delete_client;
			}
		}

		buf = vector<packet_type>(MAX_BYTES_NUMBER);
	}

delete_client:
#if defined(_MSC_VER) && defined(PENGCHAT3SERVER_DEBUG_MODE)
	OutputDebugString(TEXT("Receive thread destroying\n"));
#endif

	/* Delete this */
	{
		extern mutex g_clients_mutex;
		lock_guard<mutex> lg(g_clients_mutex);
		g_clients.remove_if([this](const cnt_ptr &p)
		{
			return p.get() == this;
		});
	}
	return;
}

bool cnt_socket::packet_processor(packet &pack)
{
	// If is not real packet
	if (pack.size() < PACKET_HEADER_SIZE)
		return false;

	packet header(pack.begin(), pack.begin() + PACKET_HEADER_SIZE);
	pack.erase(pack.begin(), pack.begin() + PACKET_HEADER_SIZE);

	// When the first call
	if (m_client_state.is_real_client == false)
	{
		// Need to check real
		if (header.compare(PROTOCOL_CHECK_REAL) == 0)
		{
			return on_check_real(pack);
		}
		else
		{
			return false;
		}
	}
	// When the second call
	if (m_client_state.is_logged == false)
	{
		// Need to login
		if (header.compare(PROTOCOL_LOGIN) == 0)
		{
			auto id_and_pw = split_packet(pack, packet("\n"));
			if (id_and_pw.size() == 2)
				return on_login(id_and_pw[0], id_and_pw[1]);
			else
				return false;
		}
	}

	// After, client can using all protocols.
	if (header.compare(PROTOCOL_GET_ROOM_INFO) == 0)
	{
		on_get_room_info();
	}
	else if (header.compare(PROTOCOL_CREATE_ROOM) == 0)
	{
		auto name_maxnum_pw = split_packet(pack, packet("\n"));

		if (name_maxnum_pw.size() == 3)
		{
			try
			{
				room::max_connector_type i = lexical_cast<room::max_connector_type>(name_maxnum_pw[1]);
				on_create_room(name_maxnum_pw[0], i, name_maxnum_pw[2]);
			}
			catch (const bad_lexical_cast &)
			{
				send_packet(PROTOCOL_CREATE_ROOM, FLAG_FAILED + to_string((uint8_t)create_room_error::unknown_capacity));
				return true;
			}
		}
	}
	else if (header.compare(PROTOCOL_REMOVE_ROOM) == 0)
	{
		room::id_type id = 0;

		try
		{
			id = room::to_room_id(pack);
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_REMOVE_ROOM, FLAG_FAILED + to_string((uint8_t)delete_room_error::unknown_room_id));
			return true;
		}

		on_remove_room(id);
	}
	else if (header.compare(PROTOCOL_ADD_CLIENT) == 0)
	{
		auto roomid_pw = split_packet(pack, packet("\n"));

		room::id_type id = 0;

		try
		{
			id = room::to_room_id(roomid_pw[0]);
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_ADD_CLIENT, FLAG_FAILED + to_string((uint8_t)entry_to_room_error::unknown_room_id));
			return true;
		}

		on_add_client(id, roomid_pw[1]);
	}
	else if (header.compare(PROTOCOL_REMOVE_CLIENT) == 0)
	{
		room::id_type id = 0;

		try
		{
			id = room::to_room_id(pack);
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_REMOVE_CLIENT, FLAG_FAILED + to_string((uint8_t)exit_from_room_error::unknown_room_id));
			return true;
		}

		on_remove_client(id);
	}
	else if (header.compare(PROTOCOL_GET_MEMBERS) == 0)
	{
		room::id_type id = 0;

		try
		{
			id = room::to_room_id(pack);
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_GET_MEMBERS, FLAG_FAILED + to_string((uint8_t)get_members_error::unknown_room_id));
			return true;
		}

		on_get_members(id);
	}
	else if (header.compare(PROTOCOL_CHANGE_STATE) == 0)
	{
		auto temp = split_packet(pack, packet("\n"));

		room::id_type id = 0;
		member::member_state state;

		try
		{
			id = room::to_room_id(temp[0]);
			state = (member::member_state)atoi(temp[1].c_str());
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_CHANGE_STATE, FLAG_FAILED + to_string((uint8_t)change_status_error::unknown_room_id));
			return true;
		}

		on_change_state(id, state);
	}
	else if (header.compare(PROTOCOL_SEND_CHAT) == 0)
	{
		auto temp = split_packet(pack, packet("\n"));

		room::id_type id = 0;

		try
		{
			id = room::to_room_id(temp[0]);
		}
		catch (const bad_lexical_cast &)
		{
			send_packet(PROTOCOL_SEND_CHAT, FLAG_FAILED + to_string((uint8_t)send_chat_error::unknown_room_id));
			return true;
		}

		string sender = temp[1];
		string chat = temp[2];
		on_send_chat(id, sender, chat);
	}

	return true;
}

bool cnt_socket::on_check_real(const packet &pack)
{
	// If the client is not using PengChat3 API, delete this client (ex: telnet, hand-made tcp program)
	if (pack.compare(MAGIC_NUMBER) == 0)
	{
		m_client_state.is_real_client = true;
		return true;
	}
	else
	{
		return false;
	}
}

bool cnt_socket::on_login(const packet &id, const packet &pw)
{
	packet nick;

	try
	{
		g_db->find_nick(id, pw, nick);

		{
			lock_guard<mutex> lg(g_clients_mutex);

			if (find_if(g_clients.begin(), g_clients.end(), [nick](const cnt_ptr &p)
			{
				return p->nick() == nick;
			}) != g_clients.end())
			{
				send_packet(PROTOCOL_LOGIN, FLAG_FAILED + to_string((uint8_t)login_error::already_logged));
				return false;
			}
		}

		m_client_state.is_logged = true;
		m_client_state.nick = nick;

		send_packet(PROTOCOL_LOGIN, FLAG_SUCCESSED + nick);

		LOGGING("Client logged. nick: " + nick);

		return true;
	}
	catch (runtime_error &)
	{
		send_packet(PROTOCOL_LOGIN, FLAG_FAILED + to_string((uint8_t)login_error::wrong_id_pw));
		return false;
	}
}

void cnt_socket::on_get_room_info()
{
	packet temp = "";

	{
		lock_guard<mutex> lg(g_room_mutex);

		for (auto r : g_room_list)
		{
			temp += (room::to_packet(r) + '\n');
		}
	}

	if (temp != "")
		temp.erase(temp.find_last_of('\n'));

	send_packet(PROTOCOL_GET_ROOM_INFO, temp);
}

void cnt_socket::on_create_room(const packet &name, room::max_connector_type max_num, const packet &pw)
{
	room::id_type id = 0;
	room new_room;
	member master;

	{
		lock_guard<mutex> lg(g_room_mutex);

		if (find_if(g_room_list.begin(), g_room_list.end(), [name](const room &r)
		{
			return r.name == name;
		}) != g_room_list.end())
		{
			send_packet(PROTOCOL_CREATE_ROOM, FLAG_FAILED + to_string((uint8_t)create_room_error::room_name_overlap));
			return;
		}

		id = create_room_id();
		new_room = { id, name, m_client_state.nick, max_num, (pw.compare(PASSWORD_NOTUSED) == 0) ? "" : pw };

		g_room_list.push_back(new_room);
	}

	// Send room has created.
	broad_cast(PROTOCOL_CREATE_ROOM, FLAG_SUCCESSED + room::to_packet(new_room));

	stringstream ss;
	ss << "Room created. name: " << new_room.name << ", master: " << new_room.master;
	LOGGING(ss.str());
}

void cnt_socket::on_remove_room(room::id_type id)
{
	{
		lock_guard<mutex> lg(g_room_mutex);

		auto it = find_room(id);

		if (it == g_room_list.end())
		{
			send_packet(PROTOCOL_REMOVE_ROOM, FLAG_FAILED + to_string((uint8_t)delete_room_error::room_not_exist));
			return;
		}

		if (it->master != m_client_state.nick)
		{
			send_packet(PROTOCOL_REMOVE_ROOM, FLAG_FAILED + to_string((uint8_t)delete_room_error::access_denied));
			return;
		}

		string name = it->name;
		g_room_list.erase(it);

		LOGGING("Room destroyed. name: " + name);
	}

	broad_cast(PROTOCOL_REMOVE_ROOM, FLAG_SUCCESSED + to_string(id));
}

void cnt_socket::on_add_client(room::id_type id, const packet &pw)
{
	lock_guard<mutex> lg(g_room_mutex);

	auto it = find_room(id);

	// If room doesn't  not exist
	if (it == g_room_list.end())
	{
		send_packet(PROTOCOL_ADD_CLIENT, FLAG_FAILED + to_string((uint8_t)entry_to_room_error::room_not_exist));
		return;
	}

	if (it->max_num != 0)
	{
		// If the room is full
		if (it->members.size() == it->max_num)
		{
			send_packet(PROTOCOL_ADD_CLIENT, FLAG_FAILED + to_string((uint8_t)entry_to_room_error::room_is_full));
			return;
		}
	}

	// If the password is wrong
	if (it->password != "")
	{
		if (pw != it->password)
		{
			send_packet(PROTOCOL_ADD_CLIENT, FLAG_FAILED + to_string((uint8_t)entry_to_room_error::password_is_wrong));
			return;
		}
	}

	if (find_if(it->members.begin(), it->members.end(), [this](const member &m)
	{
		return m.nick == m_client_state.nick;
	}) != it->members.end())
	{
		send_packet(PROTOCOL_ADD_CLIENT, FLAG_FAILED + to_string((uint8_t)entry_to_room_error::already_entered));
		return;
	}

	member new_member = { m_client_state.nick, member::member_state::online, this };

	it->members.push_back(new_member);
	it->broad_cast(PROTOCOL_ADD_CLIENT, FLAG_SUCCESSED + to_string(it->id) + '\n' + member::to_packet(new_member));

	stringstream ss;
	ss << "Member joined into room \'" << it->name << "\'. nick: " << new_member.nick;

	LOGGING(ss.str());
}

void cnt_socket::on_remove_client(room::id_type id)
{
	lock_guard<mutex> lg(g_room_mutex);

	auto it = find_room(id);

	// If room doesn't  not exist
	if (it == g_room_list.end())
	{
		send_packet(PROTOCOL_REMOVE_CLIENT, FLAG_FAILED + to_string((uint8_t)exit_from_room_error::room_not_exist));
		return;
	}

	it->broad_cast(PROTOCOL_REMOVE_CLIENT, FLAG_SUCCESSED + to_string(it->id) + '\n' + m_client_state.nick);

	it->members.remove_if([this](const member &m)
	{
		return m.nick == m_client_state.nick;
	});

	stringstream ss;
	ss << "Member exited from room \'" << it->name << "\'. nick: " << m_client_state.nick;

	LOGGING(ss.str());

	// If the room is empty
	/*if (it->members.empty())
	{
	// Delete room
	string name = it->name;
	g_room_list.erase(it);

	LOGGING("Room destroyed. name: " + name);

	broad_cast(PROTOCOL_REMOVE_ROOM, FLAG_SUCCESSED + to_string(id));
	return;
	}

	// If the master is exited
	if (it->master.compare(m_client_state.nick) == 0)
	{
	// Change master
	it->master = it->members.begin()->nick;

	it->broad_cast(PROTOCOL_MASTER_CHANGE, to_string(it->id) + '\n' + it->master);
	send_packet(PROTOCOL_MASTER_CHANGE, to_string(it->id) + '\n' + it->master);
	}*/
}

void cnt_socket::on_get_members(room::id_type id)
{
	lock_guard<mutex> lg(g_room_mutex);

	auto it = find_room(id);

	// If room doesn't not exist
	if (it == g_room_list.end())
	{
		send_packet(PROTOCOL_GET_MEMBERS, FLAG_FAILED + to_string((uint8_t)get_members_error::room_not_exist));
		return;
	}

	packet temp = "";

	for (auto mem : it->members)
	{
		temp += member::to_packet(mem) + '\a';
	}

	if (temp != "")
		temp.erase(temp.find_last_of('\a'));

	send_packet(PROTOCOL_GET_MEMBERS, FLAG_SUCCESSED + to_string(it->id) + '\n' + temp);
}

void cnt_socket::on_change_state(room::id_type id, member::member_state state)
{
	{
		lock_guard<mutex> lg(g_room_mutex);

		auto it = find_room(id);

		if (it == g_room_list.end())
		{
			send_packet(PROTOCOL_REMOVE_ROOM, FLAG_FAILED + to_string((uint8_t)change_status_error::room_not_exist));
			return;
		}

		auto mem = find_if(it->members.begin(), it->members.end(), [this](const member &m)
		{
			return m.nick == m_client_state.nick;
		});

		mem->state = state;

		it->broad_cast(PROTOCOL_CHANGE_STATE, FLAG_SUCCESSED + to_string(it->id) + '\n' + mem->nick + '\n' +
			to_string((uint8_t)state));

		string temp;

		switch (state)
		{
		case member::member_state::online: temp = "online"; break;
		case member::member_state::busy: temp = "busy"; break;
		}

		stringstream ss;
		ss << "Member state changed. nick: " << mem->nick << " state: " << temp;

		LOGGING(ss.str());
	}
}

void cnt_socket::on_send_chat(room::id_type id, const string &sender, const string &chat)
{
	{
		lock_guard<mutex> lg(g_room_mutex);

		auto it = find_room(id);

		if (it == g_room_list.end())
		{
			send_packet(PROTOCOL_SEND_CHAT, FLAG_FAILED + to_string((uint8_t)send_chat_error::room_not_exist));
			return;
		}

		it->broad_cast(PROTOCOL_SEND_CHAT, FLAG_SUCCESSED + to_string(it->id) + '\n' + sender + '\n' + chat);
	}
}

void cnt_socket::send_packet(const packet_type *header, const packet &pack)
{
	if (m_client_state.is_logged || (strncmp(header, PROTOCOL_LOGIN, PACKET_HEADER_SIZE) == 0))
	{
		packet buf = header + pack + EOP;

		m_socket->write_some(buffer(buf, buf.size()), m_latest_error);
	}
}

void cnt_socket::run()
{
	m_recv_thrd = thread(mem_fun(&cnt_socket::recv_func), this);
}