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

cnt_socket::cnt_socket(tcp::socket *client) : m_socket(client), m_client_state({ false, false, false }),
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
	vector<packet_type> buf(MAX_BYTES_NUMBER);
	vector<packet_type> real_buf(MAX_BYTES_NUMBER);
	size_t i = 0, j = 0, packet_size = 0;

	while (true)
	{
		size_t read_bytes = m_socket->read_some(buffer(buf, MAX_BYTES_NUMBER), m_latest_error);

		if (read_bytes <= 0)
			return;

		if (m_latest_error == asio::error::connection_aborted)
			return;

		if ((m_latest_error == asio::error::connection_reset) ||
			(m_latest_error == asio::error::eof))
		{
			m_client_state.need_to_delete = true;
			return;
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
				
			}
		}

	}
}

bool cnt_socket::packet_processor(packet &pack)
{
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
			m_client_state.need_to_delete = true;
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
		m_client_state.need_to_delete = true;
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

		// send message
		return true;
	}
	catch (runtime_error &)
	{
		m_client_state.need_to_delete = true;
		// log
		return false;
	}
}