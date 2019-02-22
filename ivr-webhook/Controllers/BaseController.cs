using ivr_webhook.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class BaseController : TwilioController
    {
        public DataApiHelper ApiHelper { get; set; }

        public BaseController()
        {
            ApiHelper = new DataApiHelper();
        }

        /// <summary>
        /// Elmah logger
        /// </summary>
        /// <param name="message"></param>
        public void LogMessage(string message)
        {
            ErrorLog.LogError(new Exception(), message);
        }

        /// <summary>
        /// Reject call
        /// </summary>
        /// <returns></returns>
        public VoiceResponse RejectCall()
        {
            var response = new VoiceResponse();
            var reject = new Reject("busy");
            //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
            response.Append(reject);
            return response;
        }

        /// <summary>
        /// Hangup the conversation
        /// </summary>
        /// <returns></returns>
        public ActionResult HangupConversation(string reason)
        {
            try
            {
                Log4NetLogger.Info($"\n Hangup (not by user but the app) Conversation - caller {Session["IncomingCallerNumber"] as string} , reason - {reason}\n");
                var response = new VoiceResponse();
                response.Hangup();
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Error within BaseController/HangupConversation", e);
                throw;
            }
        }

        /// <summary>
        /// Logs summary of the call inputs
        /// </summary>
        public void LogSummary()
        {
            try
            {
                Log4NetLogger.Debug( "Summary After the end of the Complete Successful Flow" + 
                                    $"CallSid = {Session["CallSid"] as string} \n" +
                                    $"IncomingCallerNumber = {Session["IncomingCallerNumber"] as string} \n" +
                                    $"IsRegisteredUser = {Session["IsRegisteredUser"] as string} \n" +
                                    $"LastParkingLocation = {Session["LastParkingLocation"] as string} \n" +
                                    $"RequestedParkingLocation = {Session["RequestedParkingLocation"] as string} \n" +
                                    $"DefaultVrn = {Session["DefaultVrn"] as string} \n" +
                                    $"CurrentVrn = {Session["CurrentVrn"] as string} \n" +
                                    $"isLimitedParkTime = {Session["isLimitedParkTime"] as string} \n" +
                                    $"HoursAvailable = {Session["HoursAvailable"] as string} \n" +
                                    $"HoursRequested = {(int)Session["HoursRequested"]} \n" +
                                    $"HoursConfirmed = {(int)Session["HoursConfirmed"]} \n" +
                                    $"HourlyRate = {(double)Session["HourlyRate"]} \n" +
                                    $"ParkingCost = {(double)Session["ParkingCost"]} \n" +
                                    $"ParkUntil = {Session["ParkUntil"] as string} \n" +
                                    $"UserDefaultCardEnding = {Session["UserDefaultCardEnding"] as string} \n" +
                                    $"UseDefaultCard = {Session["UseDefaultCard"] as string} \n" +
                                    $"CardNumberEntered = {Session["CardNumberEntered"]} \n" +
                                    $"CardNumberConfirmed = {Session["CardNumberConfirmed"]} \n" +
                                    $"CardExpiryDateEntered = {Session["CardExpiryDateEntered"]} \n" +
                                    $"CardExpiryDateConfirmed = {Session["CardExpiryDateConfirmed"]} \n" +
                                    $"CvvNumbereEntered = {Session["CvvNumberEntered"]} \n" +
                                    $"CvvNumberConfirmed = {Session["CvvNumberConfirmed"]} \n" +
                                    $"End of Summary \n");
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Error within BaseController/LogSummary", e);
                throw;
            }
        }

        /// <summary>
        /// creates and returns a Uri based on inputs
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <param name="queryStringsCollection"></param>
        /// <returns></returns>
        public Uri GetRedirectUri(string controllerName, string actionName, Dictionary<string, string> queryStringsCollection)
        {
            try
            {
                string urlStr = $"/{controllerName}/{actionName}";
                if (queryStringsCollection != null && queryStringsCollection.Any())
                {

                    for (int i = 0; i < queryStringsCollection.Count; i++)
                    {
                        //Console.WriteLine("Key: {0}, Value: {1}", dict.Keys.ElementAt(i), dict[dict.Keys.ElementAt(i)]);
                        if (i == 0)
                        {
                            urlStr +=
                                $"?{queryStringsCollection.Keys.ElementAt(i)}={queryStringsCollection[queryStringsCollection.Keys.ElementAt(i)]}";
                        }
                        else
                        {
                            urlStr +=
                                $"&{queryStringsCollection.Keys.ElementAt(i)}={queryStringsCollection[queryStringsCollection.Keys.ElementAt(i)]}";
                        }
                    }
                }

                var url = new Uri(urlStr, UriKind.Relative);
                return url;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Error within BaseController/GetRedirectUri", e);
                throw;
            }
        }
    }
}