
class Program
{
    static readonly Random rand = new Random();
    static async Task Main()
    {

        
        Console.WriteLine("학습용 코드입니다. 1~4 값 중 하나를 입력해주세요 \n1.await, 2.Task, 3.병렬처리_요청 순서대로, 4.병럴처리_응답이 오는대로");
        string ? input = Console.ReadLine();


        switch (input)
        {

            case "1":     //1. await 사용
            {
                Console.WriteLine($"await 으로 응답 요청\n\n");

                while (true)
                {
                    Console.WriteLine($"응답 요청");
                    //1. await
                    string result = await study_function();     //await은 함수가 끝날 때 까지 1초간 기다림,  함수 결과가 반드시 필요할 때.
                    Console.WriteLine($"{result}");
                }


            }
            break;


            case "2":   //2. Task 사용
            { 
                Console.WriteLine($"Task로 응답 요청\n\n");

                while (true)
                {
                    Console.WriteLine($"응답 요청");

                    //2. Task
                    Task<string> resultTask = study_function();   //Task는 기다리지 않음,   병렬처리용
                    Console.WriteLine($"{resultTask}");

                }


             }
                break;

            case "3":  //Task를 이용해 병렬 작업.  요청 순서대로 처리
            {
                Console.WriteLine($"병렬 요청 1 : 요청 순서대로 처리\n\n");

                List<Task<string>> taskList = new();   //요청들 정리를 위해 list를 생성
                                                       //10개의 작업에 병렬 요청
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"{i} 에게 응답 요청");
                    //taskList.Add(study_function_parallel(i));   //순서대로 처리
                    taskList.Add(study_function_randomDelay(i));   //순서대로 처리


                }

                    //순서대로 처리
                    foreach (Task<string> task in taskList)
                {
                    string result = await task;
                    Console.WriteLine(result);

                }



            }
            break;



            case "4": //Task를 이용해 병렬 작업.  응답 순서대로 처리
                {
                    Console.WriteLine($"병렬 요청 2 : 응답 순서대로 처리\n\n");
                    List<Task<string>> taskList = new();   //요청들 정리를 위해 list를 생성
                                                           //10개의 작업에 병렬 요청

                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"{i} 에게 응답 요청");
                        taskList.Add(study_function_randomDelay(i));   //각 작업에 딜레이가 0.5~1.5 로 랜덤

                    }
                    Console.WriteLine($"\n");

                    //while(taskList.Any())  //그대로 사용하면 순회 도중 구조변경으로 인해 버그가 발생함

                    List<Task<string>> copytasklist = taskList.ToList();  //리스트 복사
                    while (copytasklist.Any())
                    {
                        Task<string> finishedTask = await Task.WhenAny(copytasklist);
                        Console.WriteLine(await finishedTask);
                        copytasklist.Remove(finishedTask);  // 복사본 수정  안전함

                    }
            }
                break;

            default:
                Console.WriteLine($"1부터 4 사이의 값을 입력해주세요");
                break;

        }


      


    }

    static async Task<string> study_function()
    {
        await Task.Delay(1000);
        return "응답 수신\n";
    }

    static async Task<string> study_function_parallel(int id)
    {
        await Task.Delay(1000);
        return $"{id}의 응답 수신\n";
    }


    static async Task<string> study_function_randomDelay(int id)
    {
        int delay = rand.Next(500, 1501); // 딜레이는 0.5~1.5초
        await Task.Delay(delay);

        return $"{id}의 응답 수신\n";
    }


}
