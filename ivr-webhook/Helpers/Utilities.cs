using System;
using System.IO;
using System.Net;

namespace ivr_webhook.Helpers
{
    public static class Utilities
    {
        /// <summary>
        /// Adds comma between characters of a string 
        /// ABCD => A B C D
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AddSpaceBetweenChars(string str)
        {
            try
            {
                return string.Join<char>(" ", str) + " ";
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in Utilities/AddSpaceBetweenChars", e);
                throw;
            }
        }

        public static Stream GetStreamFromUrl(string url)
        {
            try
            {
                byte[] imageData = null;

                using (var wc = new WebClient())
                    imageData = wc.DownloadData(url);

                return new MemoryStream(imageData);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in Utilities/GetStreamFromUrl", e);
                throw;
            }
        }
    }
}