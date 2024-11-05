using System.Linq;

namespace Client
{
    public partial class Main : Form
    {
        public Protocol protocol;

        public Main(Protocol protocol)
        {
            this.protocol = protocol;
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Thread receiveThread = new Thread(protocol.ReceivePackets)
            {
                IsBackground = true
            };
            receiveThread.Start();

            while (true)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string message = Console.ReadLine();

            if (message.Equals(""))
                return;

            protocol.SendPacket(new MessagePacket()
            {
                Message = message
            });
        }
    }
}
