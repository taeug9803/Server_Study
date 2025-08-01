using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpServer
{
    static void Main()
    {
        UdpClient server = new UdpClient(9000);                                     
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        byte[] buffer;
        Console.WriteLine("UDP 서버 시작됨.");

        while (true)
        {
            // 수신
            buffer = server.Receive(ref remoteEP);
            string msg = Encoding.UTF8.GetString(buffer);

            Console.WriteLine($"{msg}");

            // 종료 명령시 서버 종료
            if (msg == "exit")
                break;

            // 응답
            string response = $"수신: {msg}";
            byte[] data = Encoding.UTF8.GetBytes(response);
            server.Send(data, data.Length, remoteEP);
        }

        server.Close();
    }
}
