using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpChatServer
{
    static void Main()
    {
        //소켓 생성
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        //클라이언트의 정보를 받는 용도.
        EndPoint clientEP = new IPEndPoint(IPAddress.Any,   //접근허용
                                            0);             //0 : 포트번호는 클라이언트로부터 자동으로

        server.Bind(new IPEndPoint(IPAddress.Any,           //접근허용
                                  9000));                   //서버의 포트번호는 9000


        byte[] buffer = new byte[1024];
        Console.WriteLine("UDP 서버 시작.");

        while (true)
        {
            //recv는 수신 받은게 몇바이트 크기인지
            int recv = server.ReceiveFrom(buffer,           //수신한 데이터
                                          ref clientEP);    //클라이언트의 IP정보


            string msg = Encoding.UTF8.GetString(buffer, 0, recv);
            Console.WriteLine("[클라이언트] " + msg);
            if (msg == "exit") break;


            //서버에서도 데이터 보내보기 위해 채팅처럼 구현
            Console.Write(">> ");
            string? reply = Console.ReadLine();     //타이핑 한 내용물 읽기
            if (reply == null) continue;        //비어있으면 루프 돌기


            //클라이언트에게 버퍼 전송
            server.SendTo(Encoding.UTF8.GetBytes(reply),    //타이핑 한 내용물(string)을 바이트로 변환해서 전송
                           clientEP);                       //목적지는 clientEP

            if (reply == "exit") break;
        }

        server.Close();
    }
}
