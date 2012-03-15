using System;
using System.Linq;
using System.Text;
using System.Resources;
using System.Collections;

namespace HTM.UIHtmCal
{
    public class Utilities
    {


        public static string GetResourceValue(string resFileName, string resKey)
        {
            System.Resources.ResXResourceReader r = 
                new System.Resources.ResXResourceReader(resFileName+ ".resx");

            string returnRes = "";

            // Iterate through the resources return wanted resource value
            foreach (DictionaryEntry d in r)
            {
                if (d.Key.Equals(resKey))
                {
                    returnRes = d.Value as string;
                }
            }

            r.Close();

            return returnRes;
        }
    }
}
