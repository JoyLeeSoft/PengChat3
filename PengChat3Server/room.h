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

#ifndef room_h_
#define room_h_

#include "common.h"

#define PASSWORD_NOTUSED "\a"

struct member
{
	string nick;
	enum class member_state : uint8_t
	{
		online,
		busy,
	} state;
	cnt_socket *sock;

	static packet to_packet(const member &m)
	{
		return (m.nick + '\t' + to_string((uint8_t)m.state));
	}
};

struct room
{
	typedef uint16_t max_connector_type;
	typedef uint32_t id_type;
	
	id_type id;
	string name;
	string master;
	max_connector_type max_num;
	string password;
	
	list<member> members;

	void broad_cast(const packet_type *header, const packet &pack);

	static packet to_packet(const room &r)
	{
		return (to_string(r.id) + '\t' + r.name + '\t' + r.master + '\t' + to_string(r.max_num) + '\t' +
			((r.password != "") ? "1" : "0"));
	}

	/*static room_list::iterator get_room_from_id(id_type id)
	{
		lock_guard<mutex> lg(g_room_mutex);

		return find_if(g_room_list.begin(), g_room_list.end(), [id](const room &r)
		{
			return r.id == id;
		});
	}*/
};

typedef list<room> room_list;

#endif