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
#include "cntsocket.h"
#include "utility.h"
#include "db.h"
#include "null_db.h"

list<client_ptr> g_clients;

db *g_db;

int main(int argc, char *argv[])
{
#if defined(_WIN32) || defined(_WIN64)
	EnableMenuItem(GetSystemMenu(GetConsoleWindow(), FALSE), SC_CLOSE, MF_GRAYED);
#endif

	// Basic I/O service object
	io_service io_srv;
	tcp::acceptor server(io_srv);

	bool delete_thrd_run = false;

	// Thread for accept the client
	thread acpt_thrd([&io_srv, &server]()
	{
		// tcp protocol 
		tcp protocol = tcp::v4();

		tcp::endpoint epnt(protocol, SERVER_PORT_NUMBER);

		system::error_code err;
		server.open(protocol, err);

		if (err)
		{
			cerr << "Could not create server. Please press enter to exit."
				<< "error code = " << err.value() << ' ' + err.message() + '\n';
			return;
		}

		server.bind(epnt);
		server.listen();

		g_db = 
#ifdef PENGCHAT3_DB_NULL
		new null_db();
#endif

		clog << "PengChat3 server is running... Please press enter to exit server.\n";

		while (true)
		{
			tcp::socket *client = new tcp::socket(io_srv);
			tcp::endpoint client_epnt;

			server.accept(*client, client_epnt, err);

			if (err)
			{
				delete client;

				if (err.value() == asio::error::interrupted)
					return;

				if (server.is_open() == false)
					return;

				cerr << "Could not accept client. error code = " << err.value() << "\n" + err.message() + '\n';
			}

			clog << "Client accepted. ip = " << client_epnt.address().to_string() << " port = "
				<< client_epnt.port() << '\n';

			cnt_socket *cnt = new cnt_socket(client);
			g_clients.push_back(client_ptr(cnt));
		}
	});

	delete_thrd_run = true;
	thread delete_thrd([&delete_thrd_run]()
	{
		while (delete_thrd_run)
		{
			// CPU overload protection
			this_thread::sleep_for(chrono::seconds(1));

			if (g_clients.empty())
				continue;

			g_clients.remove_if([](const client_ptr &p)
			{
				return p->is_need_to_delete();
			});
		}
	});

	cin.get();
	
	// Clients shutdown
	g_clients.clear();

	// DB shutdown
	delete g_db;

	// Delete thread shutdown
	delete_thrd_run = false;
	if (delete_thrd.joinable())
		delete_thrd.join();

	// Accept thread shutdown
	server.close();
	if (acpt_thrd.joinable())
		acpt_thrd.join();

	return 0;
}