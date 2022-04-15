using System.Net;
using System.Net.Sockets;
using System.Text;

const string ip = "127.0.0.1";
const int port = 8080;

var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
var tspSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

tspSocket.Bind(tcpEndPoint);
tspSocket.Listen(1);

bool quit = false;
var listener = tspSocket.Accept();
while (!quit)
{
    var data = new byte[256];
    var size = 0;
    var str = new StringBuilder();

    do
    {
        size = listener.Receive(data);
        str.Append(Encoding.UTF8.GetString(data, 0, size));
    }
    while (listener.Available > 0);

    if (str.ToString().Contains("exit"))
    {
        quit = true;
    }
    else if (File.Exists(str.ToString()))
    {
        var fileInfo = File.ReadAllText(str.ToString());
        listener.Send(Encoding.UTF8.GetBytes(fileInfo));
    }
    else if (Directory.Exists(str.ToString()))
    {
        var files = Directory.GetFiles(str.ToString());
        var dir = Directory.GetDirectories(str.ToString());
        foreach (var d in dir)
        {
            listener.Send(Encoding.UTF8.GetBytes(d + "\n"));
        }
        foreach (var item in files)
        {
            listener.Send(Encoding.UTF8.GetBytes(item + "\n"));
        }
    }
    else
    {
        listener.Send(Encoding.UTF8.GetBytes("command not found\n"));
        listener.Send(Encoding.UTF8.GetBytes("You can:\nView the constents of the file\nView the contents of the directory"));
    }
}

listener.Shutdown(SocketShutdown.Both);
listener.Close();
