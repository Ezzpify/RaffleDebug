using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaffleDebug
{
    static class Functions
    {
        /// <summary>
        /// If an object is int
        /// </summary>
        /// <param name="Expression">object to check</param>
        /// <returns>Returns true if number</returns>
        public static bool IsNumeric(object Expression)
        {
            double retNum;
            bool isNum = double.TryParse(Convert.ToString(Expression), 
                System.Globalization.NumberStyles.Any, 
                System.Globalization.NumberFormatInfo.InvariantInfo, 
                out retNum);
            return isNum;
        }
    }
}
