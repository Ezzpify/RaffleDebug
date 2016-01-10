using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaffleDebug
{
    static class Commands
    {
        /// <summary>
        /// Get type int for a command
        /// </summary>
        /// <param name="str">Console input string</param>
        /// <returns>Returns 0 if invalid command</returns>
        public static int GetType(string str)
        {
            switch (str)
            {
                case "quit":
                    return 1;
                case "winner":
                    return 2;
                case "size":
                    return 3;
            }

            return 0;
        }


        /// <summary>
        /// Formats and returns a command class from input
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Returns null if invalid</returns>
        public static Config.Command GetCommand(string str)
        {
            Config.Command command = new Config.Command();
            str = str.ToLower();

            if (str.Contains(" "))
            {
                string[] split = str.Split(new string[] { " " }, 2, StringSplitOptions.None);
                command.header = split[0];
                
                if (split[1].Contains(" "))
                {
                    command.arguments = split[1].Split(' ').ToList();
                }
                else
                {
                    command.arguments.Add(split[1]);
                }
            }
            else
            {
                command.header = str;
                command.arguments = new List<string>();
            }

            return command;
        }
    }
}
