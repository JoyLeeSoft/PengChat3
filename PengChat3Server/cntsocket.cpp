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

cnt_socket::cnt_socket(tcp::socket *client) : m_socket(client), m_recv_thrd(bind(&cnt_socket::recv_func,
	this)), m_is_real_client(false), m_need_to_delete(false)
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

		// If the client is not using PengChat3 API, delete this client (ex: telnet, hand-made tcp program)
		if (m_is_real_client == false)
		{
			if (read_bytes == 4)
				if (strcmp(m_buf.data(), g_api_password) == 0)
					continue;

			m_need_to_delete = true;
		}
	}
}