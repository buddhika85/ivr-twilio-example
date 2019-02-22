using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ivr_webhook.Helpers;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class ParkTimeController : BaseController
    {
        // GET: ParkTime
        public ActionResult GetParkTime()
        {
            var response = new VoiceResponse();
            try
            {
                var isLimitedParkTime = ApiHelper.IsLimitedParkTime(Session["RequestedParkingLocation"] as string);
                if (isLimitedParkTime)
                {
                    Session["isLimitedParkTime"] = "yes";
                    var isMaxTimeReached = ApiHelper.IsMaxTimeReached(Session["RequestedParkingLocation"] as string);
                    if (isMaxTimeReached)
                    {
                        // Terminator
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("ParkingHoursUnavailableTermination", "ParkTime"));
                    }
                    else
                    {
                        var hoursAvailable =
                            ApiHelper.GetAvailableParkingTimeForPark(Session["RequestedParkingLocation"] as string);
                        Session["HoursAvailable"] = hoursAvailable.ToString();
                        if (hoursAvailable <= 0)
                        {
                            // Terminator
                            response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("ParkingHoursUnavailableTermination", "ParkTime"));
                        }
                        else
                        {
                            
                            var maxHoursMessage = string.Format(ConversationHelper.MaxHoursYouCanStay, hoursAvailable, Utilities.AddSpaceBetweenChars(Session["RequestedParkingLocation"] as string));
                            var say = new Say(maxHoursMessage, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                            response.Append(say);

                            // get parking hours
                            response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkHours", "ParkTime"));
                        }
                    }
                }
                else
                {
                    Session["isLimitedParkTime"] = "no";
                    var hoursAvailable = ApiHelper.GetDefaultMaxParkingHours();
                    Session["HoursAvailable"] = hoursAvailable.ToString();
                    if (hoursAvailable <= 0)
                    {
                        // Terminator
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("ParkingHoursUnavailableTermination", "ParkTime"));
                    }
                    else
                    {
                        var maxHoursMessage = string.Format(ConversationHelper.MaxHoursYouCanStay, hoursAvailable, Utilities.AddSpaceBetweenChars(Session["RequestedParkingLocation"] as string));
                        var say = new Say(maxHoursMessage, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        response.Append(say);

                        // get parking hours
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkHours", "ParkTime"));
                    }
                }
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in ParkTimeController/GetParkTime", e);
                throw;
            }
        }

        /// <summary>
        /// Gather parking hours
        /// </summary>
        /// <returns></returns>
        public ActionResult GatherParkHours()
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                var gather = new Gather(
                    input: gatherOptionsList,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    method: HttpMethod.Get,
                    finishOnKey: "*",
                    action: Url.ActionUri("OnCarParkHoursEntered", "ParkTime")
                );
                gather.Say(ConversationHelper.EnterParkTime, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkHours", "ParkTime"));

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in ParkTimeController/GatherParkHours", e);
                throw;
            }
        }

        /// <summary>
        /// On number of parking hours entered
        /// </summary>
        /// <returns></returns>
        public ActionResult OnCarParkHoursEntered(string digits)
        {
            var response = new VoiceResponse();
            try
            {
                if (string.IsNullOrWhiteSpace(digits) && string.IsNullOrWhiteSpace(Session["HoursAvailable"] as string))
                {
                    // This cannot happen 
                    Log4NetLogger.Error($"These cannot be null - user key strokes = {digits}, HoursAvailable on car park = null");
                    var say = new Say(ConversationHelper.ErrorInGettingParkingTime, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                }
                else
                {
                    var hoursAvailable = int.Parse(Session["HoursAvailable"] as string);    // cannot be null
                    var hoursRequested = int.Parse(digits);                                 // cannot be null
                    if (hoursAvailable <= 0)  
                    {
                        // This cannot happen 
                        Log4NetLogger.Error($"HoursAvailable cannot be zero - HoursAvailable on car park = {hoursAvailable} - Hours requested by user = {hoursRequested}");
                        var say = new Say(ConversationHelper.ErrorInGettingParkingTime, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        response.Append(say);
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                    }
                    else
                    {
                        if (hoursRequested <= 0)
                        {
                            // zero or less hours requested
                            Log4NetLogger.Info($"Invalid Hour Request by user - {Session["IncomingCallerNumber"] as string} , Hours requested by user = {hoursRequested}");
                            var say = new Say(string.Format(ConversationHelper.InvalidHourRequest, hoursRequested) , ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                            response.Append(say);
                            response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                        }
                        else
                        {
                            if (hoursRequested > hoursAvailable)
                            {
                                // invalid request
                                Log4NetLogger.Info($"Invalid Hour Request by user - {Session["IncomingCallerNumber"] as string} , Hours requested by user = {hoursRequested} > Hours Remaining = {hoursAvailable}");
                                var say = new Say(string.Format(ConversationHelper.TooManyHourRequest, hoursRequested), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                                response.Append(say);
                                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                            }
                            else
                            {
                                // ok - get it confirmed
                                Session["HoursRequested"] = hoursRequested;
                                Log4NetLogger.Info($"Valid Parking Hours request {hoursRequested}");

                                var gatherOptionsList = new List<Gather.InputEnum>
                                {
                                    Gather.InputEnum.Dtmf
                                };
                                var gather = new Gather(
                                    input: gatherOptionsList,
                                    numDigits: 1,
                                    language: ConversationHelper.GatherLanguage,
                                    timeout: 5,
                                    method: HttpMethod.Get,
                                    action: Url.ActionUri("OnParkingHoursConfirmed", "ParkTime")
                                );
                                gather.Say(string.Format(ConversationHelper.ConfirmedParkingHours, hoursRequested), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                                response.Append(gather);

                                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                                response.Append(say);
                                var uri = GetRedirectUri("ParkTime", "OnCarParkHoursEntered", new Dictionary<string, string> { { "digits", digits } }); //Url.ActionUri($"OnCarParkHoursEntered?digits={digits}", "ParkTime")
                                response.Redirect(method: HttpMethod.Get, url: uri);
                            }
                        }
                    }
                }

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in ParkTimeController/OnCarParkHoursEntered", e);
                throw;
            }
        }

        /// <summary>
        /// Confirm parking hours requested
        /// </summary>
        /// <returns></returns>
        public ActionResult OnParkingHoursConfirmed(string digits)
        {
            var response = new VoiceResponse();
            try
            {
                if (digits == "*")
                {
                    Session["HoursConfirmed"] = Session["HoursRequested"];
                    Log4NetLogger.Info("Number of Parking Hours Input Confirmed - " + Session["HoursConfirmed"]);
                    //return RedirectToAction("ProvideSummary", "Summary");
                    return RedirectToAction("GetPaymentInfo", "Payment");
                }
                else
                {
                    Session["HoursRequested"] = null;
                    Session["HoursConfirmed"] = null;
                    // get Parking location again
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in ParkTimeController/CofirmHoursRequested", e);
                throw;
            }
        }

        /// <summary>
        /// Parking hours unavaibleble - Hangging up
        /// </summary>
        /// <returns></returns>
        public ActionResult ParkingHoursUnavailableTermination()
        {
            var response = new VoiceResponse();
            try
            {
                Log4NetLogger.Info($"Caller - {Session["IncomingCallerNumber"] as string}, Could not park at - {Session["RequestedParkingLocation"] as string} - Reason - Max Time Reached. Hangging up.");
                var cannotParkMaxTimeReached = string.Format(ConversationHelper.MaxTimeReached,
                    Session["RequestedParkingLocation"] as string);
                var say = new Say(cannotParkMaxTimeReached, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                //response.Hangup();
                var uri = GetRedirectUri("Base", "HangupConversation", new Dictionary<string, string> {{ "reason", "ParkingHoursUnavailableTermination" } });
                response.Redirect(method: HttpMethod.Get, url: uri);
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in ParkTimeController/ParkingHoursUnavailableTermination", e);
                throw;
            }
        }
     }
}