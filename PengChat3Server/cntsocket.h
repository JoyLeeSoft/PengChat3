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

#ifndef cntsocket_h_
#define cntsocket_h_

#include "common.h"

class cnt_socket final : private boost::noncopyable
{
public:
	cnt_socket(tcp::socket *client);
	~cnt_socket();

private:
	typedef std::shared_ptr<tcp::socket> socket_ptr;
	socket_ptr m_socket;
	system::error_code m_latest_error;

	thread m_recv_thrd;

	std::array<packet_type, MAX_BYTES_NUMBER> m_buf;
	vector<packet_type> m_temp_buf;

	struct client_state 
	{
	public:
		bool is_real_client, need_to_delete, is_logged;
	} m_client_state;

private:
	void recv_func();
	bool packet_processor(packet_header_type header, const vector<packet_type> &packet);

	bool on_check_real(const vector<packet_type> &packet);
	bool on_login(const string_utf8 &id, const string_utf8 &pw);

public:
	bool is_need_to_delete() const { return m_client_state.need_to_delete; }
};

#endif