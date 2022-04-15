using System.Net;
using System.Net.Sockets;
using System.Text;

const string ip = "127.0.0.1";
const int port = 8080;

var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
var tspSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
tspSocket.Connect(tcpEndPoint);

bool quit = false;

while (!quit)
{
    Console.Write("Enter info: ");

    var message = Console.ReadLine();
    var data = Encoding.UTF8.GetBytes(message);

    var buffer = new byte[256];
    var size = 0;
    var answer = new StringBuilder();

    tspSocket.Send(data);

    do
    {
        size = tspSocket.Receive(buffer);
        answer.Append(Encoding.UTF8.GetString(buffer));
    }
    while (tspSocket.Available > 0);

    if (message.Contains("exit"))
    {
        quit = true;
    }

    Console.WriteLine(answer.ToString());
}

tspSocket.Shutdown(SocketShutdown.Both);
tspSocket.Close();
