using ivr_webhook.Helpers;
using System;
using System.Web.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class SummaryController : BaseController
    {
        [HttpGet]
        public ActionResult ProvideSummary()
        {
            var response = new VoiceResponse();
            try
            {
                Log4NetLogger.Debug($"CallSid = {Session["CallSid"] as string} \n" +
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
                                    $"{(double)Session["HourlyRate"]} \n" +
                                    $"{(double)Session["ParkingCost"]} \n" +
                                    $"{Session["ParkUntil"] as string} \n" +
                                    $"{Session["UserDefaultCardEnding"] as string} \n" +
                                    $"{Session["UseDefaultCard"] as string} \n" +
                                    $"{Session["CardNumberEntered"]} \n" +
                                    $"{Session["CardNumberConfirmed"]} \n" +
                                    $"{Session["CardExpiryDateEntered"]} \n" +
                                    $"{Session["CardExpiryDateConfirmed"]} \n" +
                                    $"{Session["CvvNumberEntered"]} \n" +
                                    $"{Session["CvvNumberConfirmed"]} \n" +
                                    $"End of Summary \n");
                
                var summary = string.Format(ConversationHelper.FlowSummary, string.IsNullOrWhiteSpace(Session["CallerName"] as string) ? "Guest" : (string)Session["CallerName"],
                    Utilities.AddSpaceBetweenChars(Session["RequestedParkingLocation"] as string),
                    Utilities.AddSpaceBetweenChars(Session["CurrentVrn"] as string),
                    (int) Session["HoursConfirmed"]);
                Log4NetLogger.Info(summary);
                var say = new Say(summary, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                say.SsmlProsody(rate: "slow");
                response.Append(say);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Error within Summar/ProvideSummary", e);
                throw;
            }
            return TwiML(response);
        }
    }
}