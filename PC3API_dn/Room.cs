using System;
using System.Collections.Generic;
using System.Text;

namespace PC3API_dn
{
    public class Room
    {
        public string Name { get; private set; }

        public string Master { get; private set; }

        public short MaxConnectorNum { get; private set; }

        public bool IsNeedPassword { get; private set; }

        internal static Room ToRoom(string pack)
        {
            var s = pack.Split('\t');

            Room r = new Room();
            r.Name = s[0];
            r.Master = s[1];
            r.MaxConnectorNum = Convert.ToInt16(s[2]);
            r.IsNeedPassword = s[3] == "1";

            return r;
        }
    }
}
