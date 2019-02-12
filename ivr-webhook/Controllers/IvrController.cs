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
    // http://localhost:49843/elmah.axd
    // https://www.twilio.com/console/voice/calls/logs?startDate=2019-02-01+00%3A00%3A00&endDate=2019-02-28+23%3A59%3A59
    // https://www.twilio.com/console/voice/recordings/recording-logs
    public class IvrController : BaseController
    {
        // GET: IVR
        public ActionResult Index()
        {
            return View();
        }

        #region VRN

        /// <summary>
        /// Get VRN
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GatherVrn()
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Speech    // Speech input
                    //Gather.InputEnum.Dtmf
                };

                var gather = new Gather(
                    input: gatherOptionsList,
                    hints: "A, B, C, D,E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, one, two, three, four, five, size, seven, eight, nine, zero",   // 0,1,2,3,4,5,6,7,8,9
                    speechTimeout: "Auto",
                    finishOnKey:"*",
                    method: HttpMethod.Get,
                    language: ConversationHelper.GatherLanguage,
                    action: Url.ActionUri("OnVrnGatherComplete", "Ivr")    // will be called automaticaly when transcription is ready 
                    );
                gather.Say("VRN Please", ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVrn", "Ivr"));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/GatherVrn");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// On VRN gather complete
        /// </summary>
        /// <param name="speechResult"></param>
        /// <param name="confidence"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVrnGatherComplete(string speechResult, double confidence)
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                Session["VRN"] = speechResult;
                var transcript = string.Format(ConversationHelper.ConfirmedVrnQuestion, speechResult);
                var gather = new Gather(
                    input: gatherOptionsList,
                    numDigits:1,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    method: HttpMethod.Get,
                    action: Url.ActionUri("OnVrnGatherConfirmed", "Ivr")    // will be called automaticaly when transcription is ready 
                );

                LogMessage($"1 Gathered VRN {speechResult} and Confidence {confidence}");

                gather.Say(transcript, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                //RedirectToAction("OnVrnGatherComplete", new { speechResult = speechResult, confidence = confidence });
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri($"OnVrnGatherComplete?speechResult={speechResult}&confidence={confidence}", "Ivr"));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/OnVrnGatherComplete");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// Confirm VRN gathered
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVrnGatherConfirmed(string digits)
        {
           
            try
            {
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();
                    if (digits == "*")
                    {
                        LogMessage(digits + "\t" + string.Format(ConversationHelper.ConfirmedVrnMessage, Session["VRN"] as string));
                        return RedirectToAction("GatherVMaker");
                    }

                    // get VRN again
                    return RedirectToAction("GatherVrn");
                }

                return RedirectToAction("OnVrnGatherComplete", new { speechResult = Session["VRN"] as string, confidence = 0 });
            }
            catch (Exception e)
            {
                return RedirectToAction("OnVrnGatherComplete", new {speechResult = Session["VRN"] as string, confidence = 0});
            }
        }

        #endregion VRN

        #region VMaker

        /// <summary>
        /// Gather vehicle maker
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GatherVMaker()
        {
            var response = new VoiceResponse();
            try
            {
                var confirmedMessage = string.Format(ConversationHelper.ConfirmedVrnMessage, Session["VRN"] as string);
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Speech    // Speech input
                };

                var gather = new Gather(
                    input: gatherOptionsList,
                    hints: "Toyota, Nissan, B M W, Mercedes, Ford, Volvo, Honda, Mazda, Jaguar, Volkswagen, Peugeot, Audi",   // 0,1,2,3,4,5,6,7,8,9
                    speechTimeout: "Auto",
                    finishOnKey: "*",
                    method: HttpMethod.Get,
                    language: ConversationHelper.GatherLanguage,
                    action: Url.ActionUri("OnVMakerGatherComplete", "Ivr")    // will be called automaticaly when transcription is ready 
                );
                gather.Say("V Maker Please", ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVMaker", "Ivr"));

            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/GatherVMaker");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// Vehicle maker gather complete
        /// </summary>
        /// <param name="speechResult"></param>
        /// <param name="confidence"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVMakerGatherComplete(string speechResult, double confidence)
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                Session["VMaker"] = speechResult;
                var transcript = string.Format(ConversationHelper.ConfirmedVMakerQuestion, speechResult);
                var gather = new Gather(
                    input: gatherOptionsList,
                    numDigits: 1,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    method: HttpMethod.Get,
                    action: Url.ActionUri("OnVMakerGatherConfirmed", "Ivr")    // will be called automaticaly when transcription is ready 
                );

                LogMessage($"2 Gathered Maker {speechResult} and Confidence {confidence}");

                gather.Say(transcript, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri($"OnVMakerGatherComplete?speechResult={speechResult}&confidence={confidence}", "Ivr"));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/OnVMakerGatherComplete");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// Vehicle Maker gather confirmed
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVMakerGatherConfirmed(string digits)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();
                    if (digits == "*")
                    {
                        LogMessage(digits + "\t" + string.Format(ConversationHelper.ConfirmedVMakerMessage, Session["VMaker"] as string));
                        return RedirectToAction("GatherVColor");
                    }

                    // get VMaker again
                    return RedirectToAction("GatherVMaker");
                }

                return RedirectToAction("OnVMakerGatherComplete", new { speechResult = Session["VMaker"] as string, confidence = 0 });
            }
            catch (Exception e)
            {
                return RedirectToAction("OnVMakerGatherComplete", new { speechResult = Session["VMaker"] as string, confidence = 0 });
            }
        }


        #endregion VMaker

        #region Color
        /// <summary>
        /// Gather Vehicle color
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GatherVColor()
        {
            var response = new VoiceResponse();
            try
            {
                var confirmedMessage =
                    string.Format(ConversationHelper.ConfirmedVMakerMessage, Session["VMaker"] as string);
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Speech    // Speech input
                    //Gather.InputEnum.Dtmf
                };

                var gather = new Gather(
                    input: gatherOptionsList,
                    hints: "red,blue,green,white,black,silver,yellow,purple,orange,pink",   // 0,1,2,3,4,5,6,7,8,9
                    speechTimeout: "Auto",
                    finishOnKey: "*",
                    method: HttpMethod.Get,
                    language: ConversationHelper.GatherLanguage,
                    action: Url.ActionUri("OnVColorGatherComplete", "Ivr")    // will be called automaticaly when transcription is ready 
                );
                gather.Say("Color Please", ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                //return RedirectToAction("GatherVColor");
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVColor", "Ivr"));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/GatherVColor");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// On vehicle color gather complete
        /// </summary>
        /// <param name="speechResult"></param>
        /// <param name="confidence"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVColorGatherComplete(string speechResult, double confidence)
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                Session["VColor"] = speechResult;
                var transcript = string.Format(ConversationHelper.ConfirmedVColorQuestion, speechResult);
                var gather = new Gather(
                    input: gatherOptionsList,
                    numDigits: 1,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    method: HttpMethod.Get,
                    action: Url.ActionUri("OnVColorGatherConfirmed", "Ivr")    // will be called automaticaly when transcription is ready 
                );

                LogMessage($"3 Gathered Color {speechResult} and Confidence {confidence}");

                gather.Say(transcript, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                //return RedirectToAction("OnVColorGatherComplete", new {speechResult, confidence});
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri($"OnVColorGatherComplete?speechResult={speechResult}&confidence={confidence}", "Ivr"));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/OnVColorGatherComplete");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// On vehicle color gather confirmed
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OnVColorGatherConfirmed(string digits)
        {

            try
            {
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();
                    if (digits == "*")
                    {
                        LogMessage(digits + "\t" + string.Format(ConversationHelper.ConfirmedVColorQuestion, Session["VColor"] as string));
                        return RedirectToAction("ProvideSummary");
                    }

                    // get VMaker again
                    return RedirectToAction("GatherVColor");
                }

                return RedirectToAction("OnVColorGatherComplete", new { speechResult = Session["VColor"] as string, confidence = 0 });
            }
            catch (Exception e)
            {
                return RedirectToAction("OnVColorGatherComplete", new { speechResult = Session["VColor"] as string, confidence = 0 });
            }
        }
        #endregion Color


        [HttpGet]
        public ActionResult ProvideSummary()
        {
            var response = new VoiceResponse();
            try
            {
                var summary = string.Format(ConversationHelper.Summary, Session["VRN"] as string,
                    Session["VMaker"] as string, Session["VColor"] as string);
                LogMessage(summary);
                var say = new Say(summary, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/ProvideSummary");
                response = RejectCall();
            }
            return TwiML(response);
        }

        /// <summary>
        /// Reject call
        /// </summary>
        /// <returns></returns>
        private VoiceResponse RejectCall()
        {
            var response = new VoiceResponse();
            var reject = new Reject("busy");
            //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
            response.Append(reject);
            return response;
        }
    }
}