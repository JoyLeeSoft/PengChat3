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
#include "logger.h"
#include "room.h"

list<cnt_ptr> g_clients;

db *g_db;
logger *g_log;
room_list g_room_list;

mutex g_clients_mutex, g_room_mutex;

int main(int argc, char *argv[])
{
#if defined(_WIN32) || defined(_WIN64)
	EnableMenuItem(GetSystemMenu(GetConsoleWindow(), FALSE), SC_CLOSE, MF_DISABLED);
#endif

#ifdef PENGCHAT3_ENABLE_LOGGING
	try
	{
		g_log = new logger("PengChat3Server.log");
	}
	catch (...)
	{
		cerr << "Fatal error : Cannot create log file";
		cin.get();
		return EXIT_FAILURE;
	}
#endif

	// Basic I/O service object
	io_service io_srv;
	tcp::acceptor server(io_srv);

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
		LOGGING("PengChat3 server is running now.");

		ip_ban_list ban_list(g_db->load_ip_ban_list());

		while (true)
		{
			tcp::socket *client = new tcp::socket(io_srv);
			tcp::endpoint client_epnt;

			server.accept(*client, client_epnt, err);

			/* Error check */
			if (err)
			{
				delete client;

				if (err.value() == asio::error::interrupted)
					return;

				if (server.is_open() == false)
					return;

				stringstream ss;
				ss << "Could not accept client. error code = " << err.value() << "\n" + err.message();
				LOGGING(ss.str());
			}
			/*=====================================================================================================*/

			/* Ip ban check */
			if (find(ban_list.begin(), ban_list.end(), client_epnt.address().to_string()) != ban_list.end())
			{
				delete client;
				stringstream ss;
				ss << "Client " << client_epnt.address().to_string() << ":" << client_epnt.port() << 
					" is already banned, Disconnected.";
				LOGGING(ss.str());
				continue;
			}
			/*=====================================================================================================*/

			stringstream ss;
			ss << "Client connected. ip: " << client_epnt.address().to_string() << ", port: "
				<< client_epnt.port();
			LOGGING(ss.str());

			/* Try to create client */
			cnt_socket *cnt = nullptr;

			try
			{
				cnt = new cnt_socket(client, client_epnt);
				cnt->run();
				g_clients.push_back(cnt_ptr(cnt));
			}
			catch (const system_error &e)
			{
				delete cnt;

				/* If could not create thread */
				if (e.code().value() == EAGAIN)
				{
					string ip = client_epnt.address().to_string();

					ban_list.push_back(ip);
					g_db->insert_ip_ban(ip);

					{
						lock_guard<mutex> lg(g_clients_mutex);
						
						for (auto it = g_clients.begin(); it != g_clients.end();)
						{
							if ((*it)->ip() == ip)
							{
								g_clients.erase(it++);
								continue;
							}

							++it;
						}
					}

					continue;
				}
				/*=====================================================================================================*/

				stringstream ss;
				ss << "Unhandled exception! error code = " << e.code().value() << " message = " << e.code().message();
				LOGGING(ss.str());
				cin.putback('\n');

				break;
			}
		}
	});

	cin.get();
	
	g_clients.clear();

	// DB shutdown
	delete g_db;

	// Accept thread shutdown
	server.close();
	if (acpt_thrd.joinable())
		acpt_thrd.join();

	LOGGING("PengChat3 server is stopping now.");

#ifdef PENGCHAT3_ENABLE_LOGGING
	delete g_log;
#endif

#if defined(_WIN32) || defined(_WIN64)
	EnableMenuItem(GetSystemMenu(GetConsoleWindow(), FALSE), SC_CLOSE, MF_ENABLED);
#endif

	return EXIT_SUCCESS;
}