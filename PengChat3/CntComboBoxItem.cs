using PC3API_dn;

namespace PengChat3
{
    public class CntComboBoxItem
    {
        public string Text { get; set; }

        public PengChat3ClientSock Sock { get; set; }

        ~CntComboBoxItem()
        {
            if (Sock != null)
            {
                Sock.Dispose();
                Sock = null;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}