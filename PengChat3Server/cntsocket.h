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
#include "room.h"

class cnt_socket final : private boost::noncopyable
{
public:
	cnt_socket(tcp::socket *client, const tcp::endpoint &epnt);
	~cnt_socket();

private:
	typedef std::shared_ptr<tcp::socket> socket_ptr;
	socket_ptr m_socket;
	tcp::endpoint m_epnt;
	system::error_code m_latest_error;

	thread m_recv_thrd;

	struct client_state 
	{
	public:
		bool is_real_client, is_logged;
		string nick;
	} m_client_state;

	bool m_no_need_join;

private:
	void recv_func();
	bool packet_processor(packet &pack);

	bool on_check_real(const packet &pack);
	bool on_login(const packet &id, const packet &pw);
	void on_get_room_info();
	void on_create_room(const packet &name, room::max_connector_type max_num, const packet &pw);
	void on_delete_room(room::id_type id);
	void on_entry_to_room(room::id_type id, const packet &pw);
	void on_exit_from_room(room::id_type id);
	void on_get_members(room::id_type id);
	void on_change_state(room::id_type id, member::member_state state);

public:
	void send_packet(const packet_type *header, const packet &pack);

	void run();

public:
	const string ip() const
	{
		return m_epnt.address().to_string();
	}

	const string nick() const
	{
		return m_client_state.nick;
	}
};

#endif