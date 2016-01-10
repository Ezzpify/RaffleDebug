using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

namespace RaffleDebug
{
    class Provably
    {
        /// <summary>
        /// Class for storing provably values
        /// </summary>
        public class ProvablyHolder
        {
            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }
            public double PrivateRandom { get; set; }
            public double RaffleRandom { get; set; }
        }


        /// <summary>
        /// Truely random class
        /// </summary>
        public class RandomProvider
        {
            private static int seed = Environment.TickCount;
            private ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
                new Random(Interlocked.Increment(ref seed))
            );

            public Random GetThreadRandom()
            {
                return randomWrapper.Value;
            }
        }


        /// <summary>
        /// General variables
        /// </summary>
        private RandomProvider mRandom;
        public ProvablyHolder mProvablyOld;
        public ProvablyHolder mProvablyNew;


        /// <summary>
        /// Class constructor
        /// </summary>
        public Provably()
        {
            mRandom = new RandomProvider();
        }


        /// <summary>
        /// Generates and sets new values before a winner is picked
        /// </summary>
        public void PreWin()
        {
            /*If probably is null, we'll generate new values*/
            /*This will cause the first pot to be "invalid" after every restart*/
            if (mProvablyOld == null)
            {
                mProvablyOld = Generate();
            }

            /*Generate new values*/
            mProvablyNew = Generate();
        }


        /// <summary>
        /// Sets provably values after a winner has been picked
        /// </summary>
        public void PostWin()
        {
            mProvablyOld = mProvablyNew;
        }


        /// <summary>
        /// Generates new provably values
        /// </summary>
        /// <returns>Returns provably class</returns>
        private ProvablyHolder Generate()
        {
            /*Vars*/
            var enc = Encoding.ASCII;
            var randomDouble = mRandom.GetThreadRandom().NextDouble();
            var r = randomDouble * 100;

            /*Get new key*/
            var key = new byte[16];
            mRandom.GetThreadRandom().NextBytes(key);
            string sKey = Convert.ToBase64String(key);

            /*Hash*/
            byte[] rBytes = enc.GetBytes(r.ToString());
            byte[] kBytes = enc.GetBytes(sKey);
            byte[] hash;
            using (var hasher = new HMACSHA1(kBytes))
            {
                hash = hasher.ComputeHash(rBytes);
            }

            /*Return Provably*/
            return new ProvablyHolder
            {
                PublicKey = string.Join("", hash.Select(x => x.ToString("x2"))),
                PrivateKey = sKey,
                PrivateRandom = r,
                RaffleRandom = randomDouble
            };
        }
    }
}
