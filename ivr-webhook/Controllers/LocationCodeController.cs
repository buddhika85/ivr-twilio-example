using ivr_webhook.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class LocationCodeController : BaseController
    {
        /// <summary>
        /// Get Parking Location Code
        /// </summary>
        /// <returns></returns>
        public ActionResult GatherParkingLocationCode()
        {
            try
            {
                var response = new VoiceResponse();
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
                    action: Url.ActionUri("OnLocationCodeEntered", "LocationCode")
                );
                gather.Say(ConversationHelper.GetLocationCode, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in LocationCodeController/GatherParkingLocationCode", e);
                throw;
            }
        }

        /// <summary>
        /// A new parking location has been entered by the user
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult OnLocationCodeEntered(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();

                    if (ApiHelper.IsParkingLocationValid(digits))
                    {
                        Session["RequestedParkingLocation"] = digits;
                        var gatherOptionsList = new List<Gather.InputEnum>
                        {
                            Gather.InputEnum.Dtmf
                        };
                        var gather = new Gather(
                            input: gatherOptionsList,
                            numDigits: 1,
                            language: ConversationHelper.GatherLanguage,
                            timeout: 2,
                            method: HttpMethod.Get,
                            action: Url.ActionUri("OnLocationCodeConfirmed", "LocationCode")
                        );
                        gather.Say(string.Format(ConversationHelper.ConfirmedParkingLocationCodeQuestion, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                        response.Append(gather);

                        // nothing received
                        var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        response.Append(say);
                        //var urlStr = $"/LocationCode/OnLocationCodeEntered?digits={digits}";
                        //Log4NetLogger.Debug($"url Str - {urlStr}");
                        //var uri = new Uri(urlStr, UriKind.Relative);
                        var uri = GetRedirectUri("LocationCode", "OnLocationCodeEntered", new Dictionary<string, string> { { "digits", digits } });
                        Log4NetLogger.Debug($"relativeToAbsolute - {uri}");
                        response.Redirect(method: HttpMethod.Get, url: uri);

                        return TwiML(response);
                    }
                    else
                    {
                        // invalid parking location code
                        Log4NetLogger.Warn("Location Code Input Invalid - " + digits + ", asking again from caller - " + Session["IncomingCallerNumber"]);
                        var say = new Say(ConversationHelper.ParkingLocationCodeInvalid, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                        response.Append(say);

                        // get parking location again
                        Session["RequestedParkingLocation"] = null;
                        //return RedirectToAction("GatherParkingLocationCode");
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                        return TwiML(response);
                    }

                }
                else
                {
                    var say = new Say(ConversationHelper.InputUnidentified, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    // get parking location again
                    //return RedirectToAction("GatherParkingLocationCode");
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in LocationCodeController/OnLocationCodeEntered", e);
                throw;
            }
        }

        public ActionResult OnLocationCodeConfirmed(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (digits == "*")
                {
                    Log4NetLogger.Info("Location Code Input Confirmed - " + Session["RequestedParkingLocation"]);
                    //return RedirectToAction("ProvideSummary");
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVrn", "Vrn"));
                    return TwiML(response);
                }
                else
                {
                    Session["RequestedParkingLocation"] = null;
                    // get Parking location again
                    //return RedirectToAction("GatherParkingLocationCode");
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in LocationCodeController/OnLocationCodeConfirmed", e);
                throw;
            }
        }


        /// <summary>
        /// Previouse parking location ?
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult SameParkingLocationOrNot(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();

                    if (digits == "*")
                    {
                        Session["RequestedParkingLocation"] = Session["LastParkingLocation"];
                        Log4NetLogger.Info("Location Code Input Confirmed - " + Session["RequestedParkingLocation"]);
                        //return RedirectToAction("ProvideSummary");
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVrn", "Vrn"));
                        return TwiML(response);
                    }
                    else
                    {
                        // get new parking location
                        Session["RequestedParkingLocation"] = null;
                        //return RedirectToAction("GatherParkingLocationCode");
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                        return TwiML(response);
                    }
                }
                else
                {
                    var say = new Say(ConversationHelper.InputUnidentified, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    // get parking location again
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in LocationCodeController/OnLocationCodeEntered", e);
                throw;
            }
        }
    }
}