# Server_Study


# TCP 채팅 서버 - 비동기 처리 및 방 관리 테스트

이 프로젝트는 C#의 `TcpListener`와 비동기 프로그래밍을 활용하여  
멀티 클라이언트 채팅 서버를 구현한 예제입니다.

## 주요 기능

- 클라이언트별 고유 ID 부여
- 여러 채팅방(Room) 관리
- 입장, 퇴장 기능
- 방별 채팅 메시지 브로드캐스트
- 비동기 입출력 처리로 다중 접속 지원

---


## 코드
[Program.cs](02_socket_chat/Program.cs)

### 서버 로그

Server started.
Client connected. ID: 74702
Client connected. ID: 68951

Received (ID: 74702): /join room1
Client (ID: 74702) joined room 'room1'.

Received (ID: 74702): hello

Received (ID: 68951): /join room1
Client (ID: 68951) joined room 'room1'.

Received (ID: 68951): hello2

Received (ID: 74702): test1

Received (ID: 68951): /leave
Client (ID: 68951) left room 'room1'.

Received (ID: 74702): test2




### 클라이언트 A (ID: 68951)


[Server] Your ID is 68951.
/help is explain code

/join room1
[Server] You have joined room 'room1'.

hello2
[room1][68951] : hello2
[room1][74702] : test1

/leave
[Server] You have left the room.






### 클라이언트 B (ID: 74702)


[Server] Your ID is 74702.
/help is explain code

/join room1
[Server] You have joined room 'room1'.

hello
[room1][74702] : hello
[room1][68951] : hello2

test1
[room1][74702] : test1

test2
[room1][74702] : test2


## 동작 설명

- 클라이언트 A가 먼저 `room1`에 입장 후 메시지 전송.
- 아직 방에 입장하지 않은 B는 메세지를 전송받지 못함.
- 클라이언트 B가 같은 방에 입장 후 메시지 전송.
- 두 클라이언트 모두 서로의 메시지를 정상적으로 수신함  
- 클라이언트 A가 방을 나가면, 이후 B의 메시지는 A에게 전달되지 않음  
- 서버는 입장, 퇴장, 메시지 수신 이벤트를 정상적으로 기록함
  
