using System;
using System.Collections.Generic;
using System.Text;

namespace PC3API_dn
{
    public class Room
    {
        public uint ID { get; private set; }

        public string Name { get; private set; }

        public string Master { get; private set; }

        public short MaxConnectorNum { get; private set; }

        public bool IsNeedPassword { get; private set; }

        internal List<string> Members_ = new List<string>();

        public string[] Members { get { return Members_.ToArray(); } }

        internal static Room ToRoom(string pack)
        {
            var s = pack.Split('\t');

            if (s.Length == 5)
            {
                Room r = new Room();
                r.ID = Convert.ToUInt32(s[0]);
                r.Name = s[1];
                r.Master = s[2];
                r.MaxConnectorNum = Convert.ToInt16(s[3]);
                r.IsNeedPassword = s[4] == "1";

                return r;
            }
            else
            {
                return null;
            }
        }
    }
}
