using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpChatClient
{
    static void Main()
    {

        //소켓 생성
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //TCP면 ProtocolType.TCP
        
        EndPoint serverEP = new IPEndPoint(IPAddress.Loopback,  // 서버는 자기자신의 IP주소.  학습용이기 때문
                                            9000);              //포트 번호 9000.  서버의 포트와 같아야 통신

        //클라이언트가  서버의 IP 정보를 저장해두기 위한 변수(수신한 데이터의 출처 저장용)
        EndPoint fromEP = new IPEndPoint(IPAddress.Any, 0);     

        Console.WriteLine("UDP 채팅 시작됨.");

        byte[] buffer = new byte[1024];

        while (true)
        {
            Console.Write(">> ");
            string? msg = Console.ReadLine();
            if (msg == null) continue;


            //UDP 손실 테스트.  입력내용에 관계 없이 1부터 10만까지 10만개를 빠르게 보내봄
            if (msg == "Delay") 
                for (int i=0; i<100000; i++)
                    client.SendTo(Encoding.UTF8.GetBytes($"{i}"), serverEP);  //문자열을 buffer로 변환하고 serverEP에 전송

            else
                client.SendTo(Encoding.UTF8.GetBytes(msg), serverEP);  //문자열을 buffer로 변환하고 serverEP에 전송

            if (msg == "exit") break;

            int recv = client.ReceiveFrom(buffer, ref fromEP);          //서버(fromEP)로부터 받은 버퍼를 buffer에 저장,   recv는 그 버퍼의 크기
            string reply = Encoding.UTF8.GetString(buffer, 0, recv);    //buffer를 문자열로 변환해서 저장, 이후 콘솔에 출력함.
            Console.WriteLine("[서버] " + reply);
            if (reply == "exit") break;
        }

        client.Close();
    }
}
