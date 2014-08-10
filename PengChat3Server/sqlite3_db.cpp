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

/*#include <sqlite3.h>

#ifdef _MSC_VER
#pragma comment(lib, "sqlite3")
#endif

#include "sqlite3_db.h"

sqlite3_db::sqlite3_db()
{

}

sqlite3_db::~sqlite3_db()
{

}

void sqlite3_db::find_nick(const packet &id, const packet &pw, packet &nick)
{
	using namespace boost;

	sqlite3 *sql3;

	if (sqlite3_open(DBName, &sql3) != SQLITE_OK)
	{
		errMsg = (const char_utf8 *)sqlite3_errmsg(sql3);
		return false;
	}

	packet qry = (format("select Nick from %1% where Id = '%2%' and Pw = '%3%';") % MemberTableName % id % pw).str();
	char_utf8 *err;

	struct QueryResult
	{
		bool m_is_failed;
		packet m_nick;
	} result = { true, "" };

	if (sqlite3_exec(sql3, qry.c_str(), [](void *param, int argc, char_utf8 **argv, char_utf8 **azColName)
	{
		QueryResult *p = (QueryResult *)param;

		if (argc > 0)
		{
			p->m_is_failed = false;
			p->m_nick = argv[0];
		}

		return 0;
	}, &result, &err) != SQLITE_OK)
	{
		errMsg = err;
		sqlite3_close(sql3);
		return false;
	}

	if (result.m_is_failed)
	{
		errMsg = "Invalid Account";
		sqlite3_close(sql3);
		return false;
	}

	errMsg = "";
	nick = result.m_nick;

	sqlite3_close(sql3);
	return true;
}*/