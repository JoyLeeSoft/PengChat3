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
extern room_list g_room_list;

cnt_socket::cnt_socket(tcp::socket *client, const tcp::endpoint &epnt) : m_socket(client), m_epnt(epnt), m_client_state({ false, false }), m_no_need_join(false)
{
	
}

cnt_socket::~cnt_socket()
{
	//m_socket->shutdown(socket_base::shutdown_both);
	m_socket->close();
	m_socket.reset();

	if (m_no_need_join == false)
		if (m_recv_thrd.joinable())
			m_recv_thrd.join();

	stringstream ss;
	ss << "Client disconnected. ip = " << m_epnt.address().to_string() << " port = "
		<< m_epnt.port() << '\n';
	g_log->logging(ss.str());
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
			goto delete_client;

		if (read_bytes <= 0)
			goto delete_client;

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
	m_recv_thrd.detach(); // Thread detach
	m_no_need_join = true;

	/* Delete this */
	{
		extern mutex g_clients_mutex;
		lock_guard<mutex> lg(g_clients_mutex);
		g_clients.remove_if([this](const cnt_ptr &p)
		{
			return p.get() == this;
		});
	}
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

	if (header.compare(PROTOCOL_LOGIN) == 0)
	{
		auto id_and_pw = split_packet(pack, packet("\n"));
		if (id_and_pw.size() == 2)
			return on_login(id_and_pw[0], id_and_pw[1]);
		else
			return false;
	}
	else if (header.compare(PROTOCOL_GET_ROOM_INFO) == 0)
	{
		on_get_room_info();
	}
	else if (header.compare(PROTOCOL_CREATE_ROOM) == 0)
	{
		auto name_maxnum_pw = split_packet(pack, packet("\n"));

		if (name_maxnum_pw.size() == 3)
		{
			using boost::lexical_cast;
			using boost::bad_lexical_cast;

			room::max_connector_type i;

			try
			{
				i = lexical_cast<room::max_connector_type>(name_maxnum_pw[1]);
			}
			catch (const bad_lexical_cast &)
			{
				send_packet(PROTOCOL_CREATE_ROOM, to_string((uint8_t)create_room_error::unknown_capacity));
				return true;
			}

			on_create_room(name_maxnum_pw[0], i, name_maxnum_pw[2]);
		}
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
		m_client_state.is_logged = true;
		m_client_state.nick = nick;

		send_packet(PROTOCOL_LOGIN, nick);

		return true;
	}
	catch (runtime_error &)
	{
		send_packet(PROTOCOL_LOGIN, "");
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
			temp += (r.name + '\t' + r.master + '\t' + to_string(r.max_num) + '\t' + 
				((r.password != "") ? "1" : "0") + '\n');
		}
	}

	if (temp != "")
		temp.erase(temp.find_last_of('\n'));

	send_packet(PROTOCOL_GET_ROOM_INFO, temp);
}

void cnt_socket::on_create_room(const packet &name, room::max_connector_type max_num, const packet &pw)
{
	static const packet password_notused = "\t";

	{
		lock_guard<mutex> lg(g_room_mutex);

		if (find_if(g_room_list.begin(), g_room_list.end(), [name](const room &r)
		{
			return r.name == name;
		}) != g_room_list.end())
		{
			send_packet(PROTOCOL_CREATE_ROOM, to_string((uint8_t)create_room_error::room_name_overlap));
			return;
		}

		g_room_list.push_back({ name, m_client_state.nick, max_num, (pw != password_notused) ? pw : ""});
	}

	send_packet(PROTOCOL_ADD_ROM, name + '\t' + m_client_state.nick + '\t' + to_string(max_num) + '\t' +
		((pw != password_notused) ? "1" : "0"));
}

void cnt_socket::send_packet(const packet_type *header, const packet &pack)
{
	packet buf = header + pack + EOP;

	m_socket->write_some(buffer(buf, buf.size()), m_latest_error);
}

void cnt_socket::run()
{
	m_recv_thrd = thread(mem_fun(&cnt_socket::recv_func), this);
}