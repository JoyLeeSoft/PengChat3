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

#include "logger.h"

logger::logger(const string &filename) : m_file(filename, ios_base::out | ios_base::app)
{
	if (m_file.is_open() == false)
	{
		throw -1;
	}
}

logger::~logger()
{
	{
		lock_guard<mutex> lg(m_mtx);

		m_file << "================================================================================";
		m_file << "\n\n\n\n";

		m_file.flush();
	}

	m_file.close();
}

void logger::logging(const string &msg)
{
	using namespace chrono;

	lock_guard<mutex> lg(m_mtx);

	std::time_t t = system_clock::to_time_t(system_clock::now());
	string time_str(std::ctime(&t));
	time_str.erase(time_str.find_last_of('\n'));

	string temp = "[" + time_str + "] ";

	m_file << temp + msg + '\n';
	m_file.flush();
}