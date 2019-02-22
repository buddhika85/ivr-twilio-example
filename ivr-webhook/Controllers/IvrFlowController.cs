using System;
using System.Web.Mvc;
using ivr_webhook.Helpers;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using System.Collections.Generic;
using Twilio.AspNet.Mvc;
using Twilio.Http;

namespace ivr_webhook.Controllers
{
    public class IvrFlowController : BaseController
    {
        // GET: IvrFlow
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StartCall(string from, string callSid)
        {
            var response = new VoiceResponse();
            try
            {
                // validate for incoming caller number
                if (string.IsNullOrWhiteSpace(callSid) || string.IsNullOrWhiteSpace(from))
                {
                    Log4NetLogger.Warn($"Caller Id or Incoming Caller number unidentified at {DateTime.Now}, Hangging up");
                    var say = new Say(ConversationHelper.CallbackNonePrivate,
                        ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Hangup();
                }
                else
                {
                    Session["CallSid"] = string.IsNullOrWhiteSpace(callSid) ? null : callSid;
                    Session["IncomingCallerNumber"] = string.IsNullOrWhiteSpace(from) ? null : from;
                    Log4NetLogger.Info($"Date : {DateTime.Now}, Caller Id : {Session["CallSid"]}, Incoming number : {Session["IncomingCallerNumber"]}");
                    
                    // 1 Welcome user
                    var isRegisteredUser = ApiHelper.IsRegisteredUser(from);
                    Log4NetLogger.Info($"{(string) Session["IncomingCallerNumber"]} - Is Registered - {isRegisteredUser}");
                    if (isRegisteredUser)
                    {
                        Session["IsRegisteredUser"] = "yes";
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("WelcomeRegisteredUser", "IvrFlow"));
                    }
                    else
                    {
                        Session["IsRegisteredUser"] = "no";
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("WelcomeNewUser", "IvrFlow"));
                    }
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/Welcome", e);
                response = RejectCall();
            }
            return TwiML(response);
        }

        #region Welcome

        /// <summary>
        /// Welcome resgistered user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WelcomeRegisteredUser()
        {
            var response = new VoiceResponse();
            try
            {
                Log4NetLogger.Info("Welcome Registeterd User");
                // say welcome
                Session["CallerName"] = ApiHelper.GetCallerNameByPhoneNumber(Session["IncomingCallerNumber"] as string);
                var welcomeMessage = string.Format(ConversationHelper.WelcomeRegisteredUser, (string) Session["CallerName"]);
                response.Append(new Say(welcomeMessage, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage));

                Session["LastParkingLocation"] =
                    ApiHelper.GetLastparkingLocation(Session["IncomingCallerNumber"] as string);
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                if (string.IsNullOrWhiteSpace((string) Session["LastParkingLocation"]))
                {
                    // no parking history - get location code
                    //RedirectToAction("GatherParkingLocationCode");
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
                }
                else
                {
                    // are we parking in the previouse car park?
                    var gather = new Gather(
                        input: gatherOptionsList,
                        numDigits: 1,
                        language: ConversationHelper.GatherLanguage,
                        timeout: 5,
                        method: HttpMethod.Get,
                        action: Url.ActionUri("SameParkingLocationOrNot", "LocationCode")
                    );
                    gather.Say(string.Format(ConversationHelper.ParkAtSameLocationCode, Utilities.AddSpaceBetweenChars((string) Session["LastParkingLocation"])), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    response.Append(gather);

                    // nothing received
                    var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("WelcomeRegisteredUser", "IvrFlow"));
                }

            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/WelcomeRegisteredUser", e);
                throw;
            }
            return TwiML(response);
        }

        /// <summary>
        /// Welcome new user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WelcomeNewUser()
        {
            var response = new VoiceResponse();
            try
            {
                Log4NetLogger.Info("Welcome New User");

                // say welcome
                Session["CallerName"] = null;
                Session["LastParkingLocation"] = null;
                response.Append(new Say(ConversationHelper.WelcomeNewUser, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage));

                // no parking history - get location code
                //RedirectToAction("GatherParkingLocationCode");
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherParkingLocationCode", "LocationCode"));
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/WelcomeNewUser", e);
                throw;
            }
            return TwiML(response);
        }

        #endregion Welcome
    }
}