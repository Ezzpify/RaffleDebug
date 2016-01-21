using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Timers;

namespace RaffleDebug
{
    class Session
    {
        /// <summary>
        /// Represents if the bot is currently in running state
        /// </summary>
        public bool mIsRunning { get; set; } = true;


        /// <summary>
        /// Represents if the raffle is open and accepts entries
        /// </summary>
        public bool mRaffleOpen { get; set; } = true;


        /// <summary>
        /// Represents the current raffle size limit
        /// </summary>
        public int mRaffleSize { get; set; } = 10;


        /// <summary>
        /// Private variables
        /// </summary>
        private Thread mBetThread;
        private Thread mQueueThread;
        private Provably mProvably;
        private List<Config.Entry> mRaffleEntryList;
        private List<Config.TradeOffer> mQueuedAcceptedOffersList;
        private System.Timers.Timer mRaffleTimer;


        /// <summary>
        /// Class constructor
        /// Starts main session thread and instances vars
        /// </summary>
        public Session()
        {
            Console.WriteLine("Stating up...");

            /*Instance*/
            mRaffleEntryList = new List<Config.Entry>();
            mQueuedAcceptedOffersList = new List<Config.TradeOffer>();
            mProvably = new Provably();

            /*Set timer*/
            mRaffleTimer = new System.Timers.Timer();
            mRaffleTimer.Interval = TimeSpan.FromSeconds(180).TotalMilliseconds - 2000;
            mRaffleTimer.Elapsed += new ElapsedEventHandler(RaffleTimerCallback);

            /*Start bet thread*/
            mBetThread = new Thread(BetThread);
            mBetThread.Start();

            /*Start queue thread*/
            mQueueThread = new Thread(QueueThread);
            mQueueThread.Start();
        }


        /// <summary>
        /// TimerCallback - This occurs when timer runs out
        /// This will pick a winner from the current entries
        /// </summary>
        private void RaffleTimerCallback(object o, ElapsedEventArgs e)
        {
            /*Close raffle*/
            Console.WriteLine("Timer ran out, picking winner");
            mRaffleOpen = false;
            mRaffleTimer.Stop();

            /*Wait and pick a winner*/
            Thread.Sleep(2000);
            PickWinner();
        }


        /// <summary>
        /// Picks a winner from the entered raffles
        /// </summary>
        public void PickWinner(bool closedByTimer = true)
        {
            if (mRaffleEntryList.Count > 0)
            {
                /*Close raffle*/
                Console.WriteLine("Picking winner...");
                mRaffleOpen = false;
                mRaffleTimer.Stop();
                mProvably.PreWin();
                Thread.Sleep(500);

                /*If it was not finished by timer, send in an invalid timestamp to stop timer*/
                if (!closedByTimer)
                {
                    Config.UpdateRaffle UR = new Config.UpdateRaffle()
                    {
                        currentsize = mRaffleSize,
                        raffletime = DateTime.UtcNow.AddHours(-1).ToString("yyyy MM dd HH mm ss")
                    };
                    string jsonUR = JsonConvert.SerializeObject(UR, Formatting.None);
                    Website.UpdateRaffle(jsonUR);
                }

                /*Start new raffle*/
                var raffle = new Raffle<Config.Entry>();
                foreach (var entry in mRaffleEntryList)
                {
                    raffle.Add(entry, entry.Value);
                }

                /*Draw a winner*/
                var winner = raffle.Draw(mProvably.mProvablyOld.RaffleRandom);
                Console.WriteLine("Winner: {0}", winner.Key.SteamID);
                Website.NewWinner(
                    winner.Key.SteamID,
                    mProvably.mProvablyNew.PublicKey,
                    mProvably.mProvablyOld.PrivateKey,
                    mProvably.mProvablyOld.PrivateRandom);
                mProvably.PostWin();
                Thread.Sleep(10000);

                /*Reset raffle*/
                mRaffleOpen = true;
                mRaffleEntryList.Clear();
            }
            else
            {
                Console.WriteLine("Need at least one entry to pick a winner");
            }
        }


        /// <summary>
        /// Main thread
        /// Gets offers from website and updates entries
        /// </summary>
        private void BetThread()
        {
            Console.WriteLine("Bet thread started");
            while (mIsRunning)
            {
                Thread.Sleep(2500);
                try
                {
                    /*Download json string from website*/
                    string webOffers = Website.DownloadString("http://www.csgo-draw.com/API/BOTAPI/GETINVs.php");
                    if (webOffers.Length < 15)
                        continue;

                    /*Attempt to deserialize string*/
                    Config.TradeOffer[] receivedOffers = JsonConvert.DeserializeObject<Config.TradeOffer[]>(webOffers);
                    if (receivedOffers == null)
                        continue;

                    /*Go through all offers received*/
                    lock (receivedOffers)
                    {
                        foreach (var tradeOffer in receivedOffers)
                        {
                            /*Update the trade offer*/
                            Console.WriteLine("New offer from {0}", tradeOffer.SteamId);
                            Console.WriteLine(JsonConvert.SerializeObject(tradeOffer, Formatting.Indented));
                            Website.UpdateEntry("123", tradeOffer.QueId, "no");
                            Website.InQueue(tradeOffer.QueId);

                            /*Add accepted offer to queue*/
                            mQueuedAcceptedOffersList.Add(tradeOffer);
                        }
                    }
                }
                catch(Exception Ex)
                {
                    Console.WriteLine("Exception: {0}", Ex.Message);
                }
            }
        }


        /// <summary>
        /// Queue thread
        /// This checks for queued accepted offers and enters them to website
        /// </summary>
        private void QueueThread()
        {
            Console.WriteLine("Queue thread started");
            while (mIsRunning)
            {
                Thread.Sleep(2000);

                while (!mRaffleOpen) { Thread.Sleep(250); }
                var offersToBeRemoved = new List<Config.TradeOffer>();
                lock (mQueuedAcceptedOffersList)
                {
                    /*Go through all queued offers*/
                    foreach (var tradeOffer in mQueuedAcceptedOffersList)
                    {
                        if (!mRaffleOpen)
                            break;

                        /*Add entry to website and to raffle list*/
                        Website.NewEntry(tradeOffer.SteamId, tradeOffer.QueId);
                        Console.WriteLine("{0} has been entered into the raffle", tradeOffer.SteamId);

                        /*Form a new entry and add to entry list*/
                        Config.Entry uEntry = new Config.Entry()
                        {
                            SteamID = tradeOffer.SteamId,
                            QueueID = tradeOffer.QueId,
                            Value = tradeOffer.TotalPrice,
                            Items = tradeOffer.item_Ids
                        };
                        mRaffleEntryList.Add(uEntry);
                        offersToBeRemoved.Add(tradeOffer);

                        /*If we have enough items, start raffle*/
                        if (mRaffleEntryList.Sum(o => o.Items.Count) >= mRaffleSize)
                        {
                            PickWinner(false);
                        }
                        else if (mRaffleEntryList.Count >= 2 && !mRaffleTimer.Enabled)
                        {
                            StartRaffleTimer();
                        }
                    }
                }

                /*Remove offers from accepted queue*/
                mQueuedAcceptedOffersList = mQueuedAcceptedOffersList.Except(offersToBeRemoved).ToList();
            }
        }


        /// <summary>
        /// Starts the raffle timer
        /// </summary>
        private void StartRaffleTimer()
        {
            /*Get utc timestamp*/
            Console.WriteLine("Starting timer");
            string utcTimestamp = DateTime.UtcNow.AddMinutes(3).ToString("yyyy MM dd HH mm ss");

            /*Send info to site and start timer*/
            Config.UpdateRaffle ur = new Config.UpdateRaffle()
            {
                currentsize = mRaffleSize,
                raffletime = utcTimestamp
            };
            string jsonur = JsonConvert.SerializeObject(ur, Formatting.Indented);
            Website.UpdateRaffle(jsonur);
            mRaffleTimer.Start();
        }
    }
}
