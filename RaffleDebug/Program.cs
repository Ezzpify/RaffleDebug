using System;

namespace RaffleDebug
{
    class Program
    {
        /// <summary>
        /// Private variables
        /// </summary>
        private static Session session { get; set; }


        /// <summary>
        /// Main function
        /// Runs session and collects commands
        /// </summary>
        /// <param name="args">No args</param>
        static void Main(string[] args)
        {
            /*Print commands*/
            Console.Title = "RaffleDebug | By xXIncompetentCoder69Xx";
            Console.WriteLine("Commands:\n"
                + "quit - Quits application\n"
                + "winner - Picks a new winner\n"
                + "size n - Sets new raffle size (default 10) (example: size 5)\n"
                + "-----------------------------------------------\n");

            /*Instance session*/
            session = new Session();

            /*Get commands*/
            while (true)
            {
                var command = Commands.GetCommand(Console.ReadLine());
                if (command.header.Length > 0)
                {
                    int type = Commands.GetType(command.header);
                    switch (type)
                    {
                        case 0:
                            /*Invalid command*/
                            break;
                        case 1:
                            /*quit*/
                            Environment.Exit(1);
                            break;
                        case 2:
                            /*winner*/
                            session.PickWinner(false);
                            break;
                        case 3:
                            /*size*/
                            if (Functions.IsNumeric(command.arguments[0]))
                            {
                                session.mRaffleSize = Convert.ToInt32(command.arguments[0]);
                                Console.WriteLine(session.mRaffleSize);
                            }
                            break;
                    }
                }
            }
        }
    }
}
