using System;
using System.Collections.Generic;
using System.Text;

namespace passivBotCheck {
    public static class Log {

        public static void Warning(string message) {
            Console.WriteLine($"{DateTime.Now} WARNING: {message}", ConsoleColor.Yellow);
        }

        public static void Alert(string message) {
            Console.WriteLine($"{DateTime.Now} ALERT: {message}", ConsoleColor.Red);
        }

        public static void Error(string message) {
            Console.WriteLine($"{DateTime.Now} ERROR: {message}", ConsoleColor.Magenta);
        }

        public static void Status(string message) {
            Console.WriteLine($"{DateTime.Now} STATUS: {message}", ConsoleColor.Green);
        }

        public static void Init(string message) {
            Console.WriteLine($"{DateTime.Now} INIT: {message}");
        }


    }
}
