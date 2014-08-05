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

extern db *g_db;

cnt_socket::cnt_socket(tcp::socket *client) : m_socket(client), m_client_state({false, false, false}),
	m_recv_thrd(bind(&cnt_socket::recv_func, this))
{

}

cnt_socket::~cnt_socket()
{
	m_socket->close();

	if (m_recv_thrd.joinable())
		m_recv_thrd.join();

#ifdef _DEBUG
	printf("cnt_socket destroyed");
#endif
}

void cnt_socket::recv_func()
{
	while (true)
	{
		std::size_t read_bytes = m_socket->read_some(buffer(m_buf, MAX_BYTES_NUMBER), m_latest_error);

		if (m_latest_error == asio::error::connection_aborted)
			return;
	
		if ((m_latest_error == asio::error::connection_reset) ||
			(m_latest_error == asio::error::eof))
		{
			m_client_state.need_to_delete = true;
			return;
		}

		// Add packet into the buffer
		m_temp_buf.insert(m_temp_buf.end(), m_buf.begin(), m_buf.end());

		// If the packet is not perfect
		if (*(m_buf.end() - 1) != PROTOCOL_SEPARATOR)
		{
			continue;
		}
		else
		{
			vector<packet_type> readed = vector<packet_type>(m_temp_buf.begin(), m_temp_buf.begin() + read_bytes);

			// Processing packets
			for (auto packet : split<packet_type>(readed, PROTOCOL_SEPARATOR))
			{
				if (packet.empty())
					continue;

				if (packet_processor(unpacking_array<packet_header_type>(packet.data()), // Packet header unpacking
					vector<packet_type>(packet.begin() + sizeof(packet_header_type), packet.end())) == false) // Packet unpacking
				{
					break;
				}
			}

			m_temp_buf.clear();
		}
	}
}

bool cnt_socket::packet_processor(packet_header_type header, const vector<packet_type> &packet)
{
	// When the first call
	if (m_client_state.is_real_client == false)
	{
		// Need to check real
		if (header != (packet_header_type)packet_header::packet_check_real)
		{
			m_client_state.need_to_delete = true;
			return false;
		}
	}

	switch ((packet_header)header)
	{
	// Check client is real
	case packet_header::packet_check_real:
		return on_check_real(packet);
	
	// Login 
	case packet_header::packet_login:
		{
			auto id_and_pw = split<packet_type>(packet, '\n');
			if (id_and_pw.size() == 2)
				return on_login(string_utf8(id_and_pw[0].begin(), id_and_pw[0].end()),
				string_utf8(id_and_pw[1].begin(), id_and_pw[1].end()));
			else
				return false;
		}
		break;
		
	default:
		return false;
	}
}

bool cnt_socket::on_check_real(const vector<packet_type> &packet)
{
	// If the client is not using PengChat3 API, delete this client (ex: telnet, hand-made tcp program)
	if (g_magic_number == packet)
	{
		m_client_state.is_real_client = true;
		return true;
	}
	else
	{
		m_client_state.need_to_delete = true;
		return false;
	}
}

bool cnt_socket::on_login(const string_utf8 &id, const string_utf8 &pw)
{
	string_utf8 nick;
	
	try
	{
		g_db->find_nick(id, pw, nick);
		m_client_state.is_logged = true;

		// send message
		return true;
	}
	catch (runtime_error &e)
	{
		m_client_state.need_to_delete = true;
		// log
		return false;
	}
}