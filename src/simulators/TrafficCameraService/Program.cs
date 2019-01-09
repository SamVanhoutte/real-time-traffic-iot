using System;
using System.Threading.Tasks;

namespace TrafficCameraService
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Hello World!");
                Task.Delay(5000).Wait();
            }
        }
    }
}
