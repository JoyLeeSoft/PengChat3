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

#ifndef utility_h_
#define utility_h_

#include "common.h"

template <typename T> inline const T unpacking_array(const packet_type *data)
{
	return *(reinterpret_cast<const T *>(data));
}

template <typename T> inline vector<packet_type> packing_array(T data)
{
	return vector<packet_type>(reinterpret_cast<packet_type *>(&data), reinterpret_cast<packet_type *>(&data + 1));
}

/*template <> inline vector<char> to_byte_array(const char *data)
{
	return vector<char>((const char *)data, (const char *)data + strlen(data));
}*/

template<typename T> inline vector<vector<T> > split(const vector<T> &vec, const T &token)
{
	vector<vector<T> > ret;
	ret.push_back(vector<T>());
	int idx = 0;
	
	for (typename vector<T>::const_iterator it = vec.begin(); it != vec.end(); it++)
	{
		if (*it != token)
			ret.operator[](idx).push_back(*it);
		else
		{
			idx++;
			ret.push_back(vector<T>());
		}
	}

	return ret;
}

#endif