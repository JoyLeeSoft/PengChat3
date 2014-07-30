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

#include "common.h"

int main(int argc, char *argv[])
{
	// Basic I/O service object
	io_service io_srv;

	// tcp protocol 
	tcp protocol = tcp::v4();

	tcp::acceptor server(io_srv);
	tcp::endpoint epnt(protocol, SERVER_PORT_NUMBER);

	system::error_code err;
	server.open(protocol, err);

	if (err)
	{
		cerr << "Could not create server. error code = " << err.value() << "\n" + err.message() + '\n';
		return -1;
	}

	server.bind(epnt);
	server.listen();

	while (true)
	{
		tcp::socket *client = new tcp::socket(io_srv);
		tcp::endpoint client_epnt;

		server.accept(*client, client_epnt, err);

		if (err)
		{
			delete client;
			cerr << "Could not accept client. error code = " << err.value() << "\n" + err.message() + '\n';
		}

		clog << "Client accepted. ip = " << client_epnt.address().to_string() << " port = " 
			<< client_epnt.port() << '\n';

		delete client;
	}

	return 0;
}