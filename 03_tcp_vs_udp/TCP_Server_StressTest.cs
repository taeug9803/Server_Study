using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TcpServer
{
    public static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 9000);
        server.Start();
        Console.WriteLine("TCP 서버 시작됨. 클라이언트 대기 중...");

        while (true)
        {

            using TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("클라이언트 연결됨");

            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("클라이언트가 연결을 종료함.");
                    break;
                }

                string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[클라이언트] : {receivedText}");

                // 클라이언트에 응답
                string response = $"수신 확인: {receivedText}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }


        }
    }
}
