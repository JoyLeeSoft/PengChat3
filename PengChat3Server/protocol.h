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

#ifndef protocol_h_
#define protocol_h_

#include "common.h"

const packet_type MAGIC_NUMBER[] = { 0x01, 0x04, 0x03, 0x09, 0x00 };

const packet_type PROTOCOL_CHECK_REAL[] = { 'C', 'H', 'C', 'K', 0x00 };
const packet_type PROTOCOL_LOGIN[] = { 'L', 'G', 'I', 'N', 0x00 };
const packet_type PROTOCOL_GET_ROOM_INFO[] = { 'G', 'T', 'R', 'I', 0x00 };
const packet_type PROTOCOL_CREATE_ROOM[] = { 'C', 'T', 'R', 'M', 0x00 };
const packet_type PROTOCOL_DELETE_ROOM[] = { 'D', 'T', 'R', 'M', 0x00 };
const packet_type PROTOCOL_ADD_CLIENT[] = { 'A', 'D', 'C', 'T', 0x00 };
const packet_type PROTOCOL_REMOVE_CLIENT[] = { 'R', 'V', 'C', 'T', 0x00 };
const packet_type PROTOCOL_GET_MEMBERS[] = { 'G', 'T', 'M', 'B', 0x00 };
const packet_type PROTOCOL_CHANGE_STATE[] = { 'C', 'H', 'S', 'T', 0x00 };

const packet_type FLAG_SUCCESSED = '1';
const packet_type FLAG_FAILED = '0';

enum class login_error : uint8_t
{
	wrong_id_pw = 1,
	already_logged,
};

enum class create_room_error : uint8_t
{
	unknown_capacity = 1,
	room_name_overlap,
};

enum class delete_room_error : uint8_t
{
	unknown_room_id = 1,
	room_not_exist,
	access_denied,
};

enum class entry_to_room_error : uint8_t
{
	unknown_room_id = 1,
	room_not_exist,
	room_is_full,
	password_is_wrong,
	already_entered,
};

enum class exit_from_room_error : uint8_t
{
	unknown_room_id = 1,
	room_not_exist,
};

enum class get_members_error : uint8_t
{
	unknown_room_id = 1,
	room_not_exist,
};

enum class change_status_error : uint8_t
{
	unknown_room_id = 1,
	room_not_exist,
};

#endif