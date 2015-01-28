using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ES_Problems_With_Car.Models
{
    public static class Extensions
    {
        public static bool HasKeyInKeys(this NameValueCollection collection, string key)
        {
            bool hasIn = false;
            string[] arr = collection.AllKeys;
            foreach (string str in arr)
            {
                if (str == "PreviousReasonID") return hasIn = true;
            }
            return hasIn;
        }
    }
}