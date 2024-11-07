using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome5279();
            Welcome7192();
            Console.ReadKey();
        }

        static partial void Welcome7192();
        private static void Welcome5279()
        {
            Console.Write("Enter your name: ");
            string userName = Console.ReadLine() ?? "";
            Console.WriteLine("{0}, welcome to my first console aplication", userName);
        }
    }
}
