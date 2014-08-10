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

#ifndef common_h_
#define common_h_

// Common header files include

// C++ standard library
#include <cstdint>
#include <iostream>
#include <string>
#include <thread>
#include <memory>
#include <list>
#include <array>
#include <exception>

// Boost library
#include <boost/noncopyable.hpp>
#include <boost/tokenizer.hpp>

#include <boost/asio.hpp> // Async I/O

// Using namespaces
using namespace std;

using namespace boost;
using namespace boost::asio;
using namespace boost::asio::ip;

// Packet typedefs
typedef char packet_type;
typedef string packet;

// Global variables
class cnt_socket;
typedef std::shared_ptr<cnt_socket> client_ptr;
extern list<client_ptr> g_clients;

// Defines constant
#define SERVER_PORT_NUMBER 13333
#define MAX_BYTES_NUMBER 1024
#define PACKET_HEADER_SIZE 4
#define EOP (packet_type)'\0'

// Warnings
#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#define _SCL_SECURE_NO_WARNINGS
#endif

#endif