using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpChatClient
{
    static void Main()
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Loopback, 9000));
        Console.WriteLine("서버에 연결됨.");

        byte[] buffer = new byte[1024];

        while (true)
        {
            Console.Write(">> ");
            string? msg = Console.ReadLine();
            if (msg == null) continue;

            if(msg == "Delay")
            { 
                for (int i = 0; i < 100000; i++)
                    client.Send(Encoding.UTF8.GetBytes($"{i}\n"));
            
                continue;
            }
            client.Send(Encoding.UTF8.GetBytes(msg));

            if (msg == "exit") break;

            int recv = client.Receive(buffer);
            string reply = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine("[서버] " + reply);
            if (reply == "exit") break;
        }

        client.Close();
    }
}
