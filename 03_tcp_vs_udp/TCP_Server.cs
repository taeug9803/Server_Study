using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpChatServer
{
    static Random rand = new Random();
    static void Main()
    {
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Any, 9000));
        server.Listen(1);
        Console.WriteLine("TCP 서버 시작됨. 클라이언트 대기 중...");

        Socket client = server.Accept();
        Console.WriteLine("클라이언트 연결됨.");

        byte[] buffer = new byte[1024];

        while (true)
        {
            int recv = client.Receive(buffer);
            string msg = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine("[클라이언트] :" + msg);
            if (msg == "exit") break;

            Console.Write(">> ");
            string? reply = Console.ReadLine();
            if (reply == null) continue;

            client.Send(Encoding.UTF8.GetBytes(reply));
            if (reply == "exit") break;
        }

        client.Close();
        server.Close();
    }
}