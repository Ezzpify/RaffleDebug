using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaffleDebug
{
    public class Raffle<T>
    {
        /*Class for each entry ticket*/
        public class Ticket
        {
            public T Key { get; private set; }
            public double Weight { get; private set; }
            public double Odds { get; set; }

            /*Set ticket*/
            public Ticket(T key, double weight)
            {
                Key = key;
                Weight = weight;
                Odds = 0;
            }
        }

        /*Ticket list*/
        public List<Ticket> tickets = new List<Ticket>();
        private static Random rand = new Random();

        /*Add a new ticket*/
        public void Add(T key, double weight)
        {
            tickets.Add(new Ticket(key, weight));
        }

        /*Draw a winner*/
        public Ticket Draw(double r)
        {
            goerror:
            /*If provably value is passed it won't be 0*/
            if (r == 0)
            {
                r = rand.NextDouble() * tickets.Sum(a => a.Weight);
            }
            else
            {
                r *= tickets.Sum(a => a.Weight);
            }

            /*Vars*/
            double min = 0;
            double max = 0;

            /*Winner*/
            Ticket winner = null;

            /*Get a winner*/
            foreach (var ticket in tickets)
            {
                max += ticket.Weight;

                if (min <= r && r < max)
                {
                    winner = ticket;
                    winner.Odds = r;
                    break;
                }

                min = max;
            }

            /*ehhh... don't mind me lol*/
            if (winner == null)
            {
                r = 0;
                goto goerror;
            }

            /*Return the winner*/
            return winner;
        }
    }
}
