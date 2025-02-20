using Capitalov;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Vexium_Net
{
    internal class Feature
    {
        private MemoryRaider _memory = new MemoryRaider();
        private nint _client;
        private nint _instruction;
        private byte[] _realBytes = new byte[] { 0x32, 0xC0 };

        public Feature()
        {
            try
            {
                _memory.Inject("cs2");
                _client = _memory.GetModuleBase("client.dll");
            }
            catch (Exception)
            {
                Environment.Exit(0);
            }

            Start();
        }

        private void Start()
        {
            _instruction = _client + 0x867F20;
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                _memory.WriteBytes(_instruction, _realBytes);
            };

            Task.Run(() => StartServer());
        }

        private void StartServer()
        {
            int port = 8080;

            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            DateTime lastReceivedTime = DateTime.Now;

            Task.Run(() =>
            {
                while (true)
                {
                    if ((DateTime.Now - lastReceivedTime).TotalSeconds > 60)
                    {
                        _memory.WriteBytes(_instruction, _realBytes);
                        lastReceivedTime = DateTime.Now;
                    }

                    Thread.Sleep(1000);
                }
            });

            while (true)
            {
                using (TcpClient client = server.AcceptTcpClient())
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (request.Contains("GLOW"))
                    {
                        _memory.Nop(_instruction, _realBytes.Length);
                        lastReceivedTime = DateTime.Now;
                    }
                    else
                    {
                        _memory.WriteBytes(_instruction, _realBytes);
                    }
                }
            }
        }

        [STAThread]
        public static void Main()
        {
            Feature feature = new Feature();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
