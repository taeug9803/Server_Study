using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


//telnet 127.0.0.1 8888
class Program
{
    static TcpListener listener;
    static ConcurrentDictionary<TcpClient, int> clientIds = new();          //  <클라이언트,id값>   
    static ConcurrentDictionary<string, List<TcpClient>> rooms = new();   // <채팅방, 유저들 리스트>

    static Random random = new();

    static async Task Main()
    {
        //서버 역할 등록
        listener = new TcpListener(IPAddress.Any, 8888);
        listener.Start();
        Console.WriteLine("Server started.");
        
        while (true)
        {
            //클라이언트가 접속할 때까지(=TCP 연결 요청이 올 때 까지) 비동기로 대기
            //클라이언트가 접속하면 TcpClient 객체를 반환받습니다
            var client = await listener.AcceptTcpClientAsync();


            _ = HandleClient(client);       //async Task 함수의 경우 반환값을 받지 않으면 경고가 뜨기에 _= 를 함
                                            //내부에서 await을 쓰기 때문에 async Taks함수.
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        int clientId = random.Next(10000, 99999);
        clientIds[client] = clientId;   // clientIds는 맨 위에서 선언한 ConcurrentDictionary.     map의 <키값,데이터값> 인데   변수이름[키값] = 데이터값  처럼 사용 가능함
        Console.WriteLine($"Client connected. ID: {clientId}");

        var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };


        string currentRoom = null;

        try
        {
            //WriteLineAsync : 비동기적으로 텍스트 출력하는 함수. 
            await writer.WriteLineAsync($"[Server] Your ID is {clientId}. \n /help  is explain code");

            while (true)
            {
                //ReadLineAsync : 비동기적으로 텍스트 읽는 함수
                string? line = await reader.ReadLineAsync();   //텍스트가 없어도 멈추지 않게 await,  텍스트가 없어 null이여도 터지지 않게 string대신 string? 을 사용
                if (line == null)       //들어온 텍스트가 없으면 루프 종료.
                    break;

                line = line.Trim();   //깔끔하게 앞뒤 공백 제거
                Console.WriteLine($"Received (ID:{clientId}): {line}\n");    // ID와 텍스트를 출력.  ex)  Received (ID:12345): 안녕하세요


                //사용법 설명용 도움말 기능.
                if (line.StartsWith("/help"))
                {
                    //WriteLineAsync는 비동기 출력이기 때문에, await을 사용하지 않으면 텍스트 순서가 꼬일 수 있음
                    await writer.WriteLineAsync("\n\n\n-------");
                    await writer.WriteLineAsync("/show room \n show room list");
                    await writer.WriteLineAsync("/join {roomname} \n ex) /join room1");
                    await writer.WriteLineAsync("/leave \n leave the current room");

                    await writer.WriteLineAsync("/show user \n show a list of users in the current room");
                    await writer.WriteLineAsync("/show all user \n Shows all users and the rooms they are in");

                    await writer.WriteLineAsync("/leave \n leave the current room");
                    await writer.WriteLineAsync("-------\n\n\n");
                }

                else if (line.StartsWith("/show room"))   //존재하는 채팅방을 보여달라
                {

                    await writer.WriteLineAsync("[Server] Room List:");
                    foreach (var room in rooms.Keys)
                    {
                        await writer.WriteLineAsync("- " + room);
                    }

                    await writer.WriteLineAsync("\n\nYou can create room. Use /join {RoomName}\n\n");
                }
                
                else if (line.StartsWith("/show user")) // 현재 방에 존재하는 유저를 보여달라
                {
                    if (currentRoom != null)
                    {
                        foreach (var c in rooms[currentRoom])
                        {
                            int id = clientIds.ContainsKey(c) ? clientIds[c] : -1;
                            await writer.WriteLineAsync($"- User ID: {id}");
                        }
                    }
                    else
                    {
                        await writer.WriteLineAsync("\n\nJoin to Room first.\n\n");
                    }
                }
                else if (line.StartsWith("/show all user"))  // 서버에 존재하는 모든 유저와, 그들이 들어가있는 채팅방 이름을 보여달라
                {
                    await writer.WriteLineAsync("[Server] All connected users and their rooms:");

                    foreach (var kvp in clientIds)
                    {
                        client = kvp.Key; // 새로 선언 없이 할당만
                        int id = kvp.Value;

                        string userRoom = "<not in any room>";
                        foreach (var roomEntry in rooms)
                        {
                            if (roomEntry.Value.Contains(client))
                            {
                                userRoom = roomEntry.Key;
                                break;
                            }
                        }

                        await writer.WriteLineAsync($"- User ID: {id} | Room: {userRoom}");
                    }

                }



                else if (line.StartsWith("/join "))
                {
                    string roomName = line[6..];

                    // 현재 방에서 나가기
                    if (currentRoom != null)
                    {
                        rooms[currentRoom].Remove(client);

                        // 방이 비면 삭제
                        if (rooms[currentRoom].Count == 0)
                        {
                            rooms.TryRemove(currentRoom, out _);
                            Console.WriteLine($"Room '{currentRoom}' removed (empty).");
                        }
                    }

                    // 새 방에 입장
                    if (!rooms.ContainsKey(roomName))
                        rooms[roomName] = new List<TcpClient>();

                    rooms[roomName].Add(client);
                    currentRoom = roomName;
                    Console.WriteLine($"Client (ID:{clientId}) joined room '{roomName}'.");

                    await writer.WriteLineAsync($"[Server] You have joined room '{roomName}'.");
                }
                else if (line.StartsWith("/leave"))
                {
                    if (currentRoom != null)
                    {
                        rooms[currentRoom].Remove(client);
                        Console.WriteLine($"Client (ID:{clientId}) left room '{currentRoom}'.");
                        await writer.WriteLineAsync("[Server] You have left the room.");

                        // 방이 비면 삭제
                        if (rooms[currentRoom].Count == 0)
                        {
                            rooms.TryRemove(currentRoom, out _);
                            Console.WriteLine($"Room '{currentRoom}' has been removed because it's empty.");
                        }

                        currentRoom = null;
                    }
                }
                else if (line.StartsWith("/debug"))
                {
                    Console.WriteLine($"[DEBUG] roomName: '{currentRoom}'");
                }

                else
                {
                    if (currentRoom != null)
                    {
                        foreach (var c in rooms[currentRoom])
                        {
                            var cWriter = new StreamWriter(c.GetStream(), new UTF8Encoding(false)) { AutoFlush = true };

                            await cWriter.WriteLineAsync($"[{currentRoom}][{clientId}] : {line}");
                        }
                    }
                    else
                    {
                        await writer.WriteLineAsync($"please join to room. if you need help enter /help");
                    }
                }

            }
        }
        finally
        {
            if (currentRoom != null)
            {
                rooms[currentRoom].Remove(client);
            }
            clientIds.TryRemove(client, out _);
            client.Close();
            Console.WriteLine($"Client disconnected (ID:{clientId}).");
        }
    }
}

