using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaffleDebug
{
    public class Config
    {
        /// <summary>
        /// Class that holds information about an item
        /// </summary>
        public class ItemId
        {
            /*Deserialization part*/
            /*These vallues will be set automatically*/
            public long item_Id { get; set; }
            public long class_Id { get; set; }
            public string Item_Name { get; set; }
            public double Item_Price { get; set; }
        }


        /// <summary>
        /// Class that  holds information about a trade offer that we got from the website
        /// </summary>
        public class TradeOffer
        {
            /*Deserialization part*/
            /*These vallues will be set automatically*/
            public string Username { get; set; }
            public ulong SteamId { get; set; }
            public int QueId { get; set; }
            public double TotalPrice { get; set; }
            public string TradeToken { get; set; }
            public string SecurityToken { get; set; }
            public List<ItemId> item_Ids { get; set; }
        }
        

        /// <summary>
        /// Class that holds information about a raffle entry
        /// </summary>
        public class Entry
        {
            public ulong SteamID { get; set; }
            public int QueueID { get; set; }
            public double Value { get; set; }
            public List<ItemId> Items { get; set; }
        }


        /// <summary>
        /// Class that holds in about timer information that we send serialized to the website
        /// </summary>
        public class UpdateRaffle
        {
            public int currentsize { get; set; }
            public string raffletime { get; set; }
        }


        /// <summary>
        /// Class that holds information about a command that is read from Console.ReadLine()
        /// </summary>
        public class Command
        {
            public string header { get; set; }
            public List<string> arguments { get; set; } = new List<string>();
        }
    }
}
