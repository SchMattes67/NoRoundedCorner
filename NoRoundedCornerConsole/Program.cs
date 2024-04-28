using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static EventWaitHandle _waitHandle = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            bool showdebug = args.Contains("debug");

            Console.WriteLine("Set all window corner from rounded to unrounded in real-time");
            Console.WriteLine("start with parameter 'debug' for debuging messages");
            Console.WriteLine();

            RoundCorner roundCorner = new RoundCorner(showdebug);

        }
    }
}
