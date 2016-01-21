using System.Net;
using System.Web;
using System.Threading.Tasks;

namespace RaffleDebug
{
    static class Website
    {
        /// <summary>
        /// Sends information to the website
        /// </summary>
        /// <param name="Url">Url to contact</param>
        /// <param name="Parameter">Parameter string to send</param>
        private static void SendInformation(string Url, string Parameter)
        {
            bool success = false;
            int attempts = 10;
            while (attempts-- > 0)
            {
                if (UploadString(Url, Parameter) == "OK")
                {
                    success = true;
                    break;
                }
            }

            if (success)
            {
                UpdateJson();
            }
        }

        /// <summary>
        /// Attempts to update the json on the website
        /// </summary>
        /// <returns>Returns true if we received OK from the site</returns>
        private static bool UpdateJson()
        {
            int tries = 10;
            while (tries-- > 0)
            {
                if (UploadString(APIEndpoints.host + APIEndpoints.updatejson) == "OK")
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Sends the updated state of an offer to the website
        /// </summary>
        /// <param name="OfferId">OfferId of the offer</param>
        /// <param name="QueueId">QueueId of the offer that we got from the original json from website</param>
        /// <param name="Status">Status of trade "yes" = failed "no" = not failed</param>
        public static void UpdateEntry(string OfferId, int QueueId, string Status)
        {
            string request = string.Format("action=TradeOfferSent&tradelink={0}&queId={1}&failed={2}", OfferId, QueueId, Status);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Sends information about bot to website for display
        /// </summary>
        /// <param name="updateQueueClassString">Serialized class string for class Config.UpdateQueue</param>
        public static void UpdateQueue(string updateQueueClassString)
        {
            string request = string.Format("action=updatequeinf&queinfo={0}", updateQueueClassString);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Sends information about raffle size and upcoming size
        /// </summary>
        /// <param name="updateRaffleClassString"></param>
        public static void UpdateRaffle(string updateRaffleClassString)
        {
            string request = string.Format("action=updatepotsize&queinfo={0}", updateRaffleClassString);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Sends a new entry to the website
        /// </summary>
        /// <param name="SteamId">SteamId of user</param>
        /// <param name="QueueId">QueueId of users</param>
        public static void NewEntry(ulong SteamId, int QueueId)
        {
            string request = string.Format("action=newraffleentry&steamid={0}&queId={1}", SteamId, QueueId);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Sends in winner of a raffle to website
        /// </summary>
        /// <param name="SteamId">SteamId of winner</param>
        /// <param name="PublicKey">Provably</param>
        /// <param name="PrivateKey">Provably</param>
        /// <param name="PrivateRandom">Provably</param>
        public static void NewWinner(ulong SteamId, string PublicKey, string PrivateKey, double PrivateRandom)
        {
            string request = string.Format("action=newraffle&winnerid={0}&PublicKey={1}&PrivateKey={2}&Percentage={3}", SteamId, PublicKey, HttpUtility.UrlEncode(PrivateKey), PrivateRandom);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Sends update to website that a particular queueId is now in a queue and waiting to be put in raffle
        /// </summary>
        /// <param name="QueueId">QueueId of user</param>
        public static void InQueue(int QueueId)
        {
            string request = string.Format("action=inque&queId={0}", QueueId);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Uploads the cut item to the website
        /// </summary>
        /// <param name="QueueId">QueueId of winner of the raffle where this item got cut</param>
        /// <param name="ClassId">ClassId of the item that got cut</param>
        public static void UploadCutItem(string QueueId, string ClassId)
        {
            string request = string.Format("action=cutitem&queId={0}&CutItem={1}", QueueId, ClassId);
            SendInformation(APIEndpoints.host + APIEndpoints.process, request);
        }


        /// <summary>
        /// Downloads a string from the internet from the specified url
        /// </summary>
        /// <param name="URL">Url to download string from</param>
        /// <returns>Returns string of url website source</returns>
        public static string DownloadString(string URL)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    return wc.DownloadString(URL);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }


        /// <summary>
        /// Upload a string with arguments to a specified url
        /// </summary>
        /// <param name="URL">Url to upload to</param>
        /// <param name="arguments">Arguments to upload</param>
        /// <returns>Returns string of website response to upload</returns>
        public static string UploadString(string URL, string arguments = "")
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    return wc.UploadString(URL, arguments);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}
