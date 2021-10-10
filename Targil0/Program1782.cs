using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1782();
            Welcome1816();
            Console.ReadKey();
        }

        static partial void Welcome1816();

        private static void Welcome1782()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", name);
        }
    }
}
