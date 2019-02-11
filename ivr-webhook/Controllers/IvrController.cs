using ivr_webhook.Helpers;
using ivr_webhook.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    // http://localhost:49843/elmah.axd
    // https://www.twilio.com/console/voice/calls/logs?startDate=2019-02-01+00%3A00%3A00&endDate=2019-02-28+23%3A59%3A59
    // https://www.twilio.com/console/voice/recordings/recording-logs
    public class IvrController : TwilioController
    {
        public static string NgRokPath { get; } = " https://5b2491ec.ngrok.io/Ivr";

        public FlowExecutionData FlowExecutionData { get; set; }

        public IvrController()
        {
            FlowExecutionData =  new FlowExecutionData();
        }

        // GET: IVR
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// Get VRN
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Welcome()
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Speech,    // Speech input
                    //Gather.InputEnum.Dtmf
                };

                var gather = new Gather(
                    input: gatherOptionsList,
                    hints: "A, B, C, D,E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, one, two, three, four, five, size, seven, eight, nine, zero",   // 0,1,2,3,4,5,6,7,8,9
                    speechTimeout: "Auto",
                    finishOnKey:"*",
                    language: ConversationHelper.GatherLanguage,
                    action: Url.ActionUri("OnVrnGatherComplete", "Ivr")    // will be called automaticaly when transcription is ready 
                    );
                gather.Say(ConversationHelper.InputVrn, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/Welcome");
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
        [HttpPost]
        public TwiMLResult OnVrnGatherComplete(string speechResult, double confidence)
        {
            var response = new VoiceResponse();
            try
            {
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                FlowExecutionData.Vrn = speechResult;
                var transcript = $"You said, \n {speechResult}, \nTo continue press * on your keypad, \nOr to retry press any other key";
                var gather = new Gather(
                    input: gatherOptionsList,
                    numDigits:1,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    action: Url.ActionUri("OnVrnGatherConfirmed", "Ivr")    // will be called automaticaly when transcription is ready 
                );

                gather.Say(ConversationHelper.InputVrn, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
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
        [HttpPost]
        public ActionResult OnVrnGatherConfirmed(string digits)
        {
           
            try
            {
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();
                    if (digits == "*")
                    {
                        var response = new VoiceResponse();
                        var say = new Say($"You confirmed your VRN which is {FlowExecutionData.Vrn}, Good Bye",
                            ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        response.Append(say);
                        return TwiML(response);
                    }
                    else
                    {
                        // get VRN again
                        return RedirectToAction("Welcome");
                    }
                }
                else
                {
                    return RedirectToAction("OnVrnGatherComplete", new { speechResult = FlowExecutionData.Vrn, confidence = 0 });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("OnVrnGatherComplete", new {speechResult = FlowExecutionData.Vrn, confidence = 0});
            }
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