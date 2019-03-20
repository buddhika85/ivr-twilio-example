using ivr_webhook.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using IBM.WatsonDeveloperCloud.SpeechToText.v1;
using IBM.WatsonDeveloperCloud.Util;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class VrnController : BaseController
    {
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
                var isRegistered = Session["IsRegisteredUser"] as string == "yes";
                var incomingCallNumber = Session["IncomingCallerNumber"] as string;

                if (isRegistered && !string.IsNullOrWhiteSpace(incomingCallNumber))
                {
                    var defaultVrn = ApiHelper.GetDefaultCarVrn(incomingCallNumber);
                    if (string.IsNullOrWhiteSpace(defaultVrn))
                    {
                        // no default VRN - Get VRN
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                    }
                    else
                    {
                        var gatherOptionsList = new List<Gather.InputEnum>
                        {
                            Gather.InputEnum.Dtmf
                        };
                        Session["DefaultVrn"] = defaultVrn;
                        // are we using default VRN?
                        var gather = new Gather(
                            input: gatherOptionsList,
                            numDigits: 1,
                            language: ConversationHelper.GatherLanguage,
                            timeout: 5,
                            method: HttpMethod.Get,
                            action: Url.ActionUri("UseDefaultVrnOrNot", "Vrn")
                        );
                        gather.Say(string.Format(ConversationHelper.UseDefaultVrn, Utilities.AddSpaceBetweenChars((string)Session["DefaultVrn"])), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                        response.Append(gather);

                        // nothing received
                        var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        response.Append(say);
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GatherVrn", "Vrn"));
                    }
                }
                else
                {
                    // not regisreted - Get VRN
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in VrnController/GatherVrn", e);
                throw;
            }
            return TwiML(response);
        }

        /// <summary>
        /// Use Default VRN?
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult UseDefaultVrnOrNot(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();

                    if (digits == "*")
                    {
                        Session["CurrentVrn"] = Session["DefaultVrn"];
                        Log4NetLogger.Info("VRN Confirmed - " + Session["CurrentVrn"]);
                        //return RedirectToAction("ProvideSummary");
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                        return TwiML(response);
                    }
                    else
                    {
                        //var say = new Say(ConversationHelper.InputUnidentified, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                        //response.Append(say);

                        // get new parking location
                        Session["CurrentVrn"] = null;
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                        return TwiML(response);
                    }
                }
                else
                {
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in VrnController/OnLocationCodeEntered", e);
                throw;
            }
        }

        /// <summary>
        /// Get a new VRN
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCurrentVrn(string retry = "no")
        {
            var response = new VoiceResponse();
            try
            {
                ////var gatherOptionsList = new List<Gather.InputEnum>
                ////{
                ////    Gather.InputEnum.Speech     // Speech input
                ////};

                ////var gather = new Gather(
                ////    input: gatherOptionsList,
                ////    hints: "A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, one, two, three, four, five, size, seven, eight, nine, zero, " /*+ ApiHelper.GetVrnHints()*/,   // 0,1,2,3,4,5,6,7,8,9
                ////    speechTimeout: "Auto",
                ////    finishOnKey: "*",
                ////    method: HttpMethod.Get,
                ////    language: ConversationHelper.GatherLanguage,
                ////    timeout: 5,
                ////    action: Url.ActionUri("OnVrnGatherComplete", "Vrn")
                ////);
                ////gather.Say(retry == "no" ? ConversationHelper.SayVrn : ConversationHelper.SayVrnAgain, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                ////response.Append(gather);

                
                response.Say(retry == "no" ? ConversationHelper.SayVrn : ConversationHelper.SayVrnAgain, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Record(maxLength: 60, timeout: 0, action: Url.ActionUri("VrnRecorded", "Vrn"));

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                var uri = GetRedirectUri("Vrn", "GetCurrentVrn", new Dictionary<string, string> { {"retry", "no"} });
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in VrnController/GetCurrentVrn", e);
                throw;
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
                Session["CurrentVrn"] = speechResult;
                Session["VrnConfidence"] = confidence.ToString("0.0");
                var transcript = string.Format(ConversationHelper.ConfirmedVrnQuestion, Utilities.AddSpaceBetweenChars(speechResult));
                var gather = new Gather(
                    input: gatherOptionsList,
                    numDigits: 1,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    method: HttpMethod.Get,
                    action: Url.ActionUri("OnVrnGatherConfirmed", "Vrn")
                );

                Log4NetLogger.Info($"Gathered VRN {speechResult} with confidence {confidence}");

                gather.Say(transcript, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                var uri = GetRedirectUri("Vrn", "OnVrnGatherComplete", new Dictionary<string, string> { { "speechResult", speechResult }, { "confidence", confidence.ToString("0.0") } } );
                // Url.ActionUri($"OnVrnGatherComplete?speechResult={speechResult}&confidence={confidence}", "Vrn")
                response.Redirect(method: HttpMethod.Get, url: uri);
               
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in VrnController/OnVrnGatherComplete", e);
                throw;
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
                var response = new VoiceResponse();
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    digits = digits.Trim();
                    if (digits == "*")
                    {
                        Log4NetLogger.Info($"Confirmed VRN {Session["CurrentVrn"] as string}");
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetParkTime", "ParkTime"));
                        return TwiML(response);
                    }
                    else
                    {
                        // get VRN again
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                        return TwiML(response);
                    }
                }
                else
                {
                    //return RedirectToAction("OnVrnGatherComplete", new { speechResult = Session["CurrentVrn"] as string, confidence = 0 });
                    //Url.ActionUri($"OnVrnGatherComplete?speechResult={Session["CurrentVrn"] as string}&confidence=0", "Vrn")
                    var uri = GetRedirectUri("Vrn", "OnVrnGatherComplete", new Dictionary<string, string> { { "speechResult", Session["CurrentVrn"] as string }, { "confidence", Session["VrnConfidence"] as string } });
                    response.Redirect(method: HttpMethod.Get, url: uri);
                    return TwiML(response);
                }

            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in VrnController/OnVrnGatherConfirmed", e);
                return RedirectToAction("OnVrnGatherComplete", new { speechResult = Session["CurrentVrn"] as string, confidence = 0 });
            }
        }


        //// new IBM watson changes
        public ActionResult VrnRecorded(BodyData bodyData)
        {
            try
            {
                string recordingUrl = bodyData.RecordingUrl;
                Log4NetLogger.Debug($"IBM - {recordingUrl}");

                var response = new VoiceResponse();
                response.Say("Thanks. take a listen to what you said.");
                response.Play(new Uri(recordingUrl));

                TokenOptions iamAssistantTokenOptions = new TokenOptions
                {
                    IamApiKey = ConfigurationManager.AppSettings["watsonAPIKey"],
                    ServiceUrl = ConfigurationManager.AppSettings["watsonUrl"]
                };

                SpeechToTextService _speechToText = new SpeechToTextService(iamAssistantTokenOptions);

                var streamFromUrl = Utilities.GetStreamFromUrl(recordingUrl);
                var results =
                    _speechToText.Recognize(audio: streamFromUrl, contentType: "audio/wav",
                        //keywords: new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
                        keywords: new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "zero" },
                        keywordsThreshold: 0.5f, model: "en-US_NarrowbandModel");


                if (results?.Results[0]?.Alternatives[0]?.Confidence != null)
                {
                    var predictedTranscript = results.Results[0].Alternatives[0].Transcript;
                    var predictionConfidence = results.Results[0].Alternatives[0].Confidence;
                    Log4NetLogger.Info($"IBM Confidence : {predictionConfidence} \n IBM Transcript : {predictedTranscript}");
                    //response.Say($"You said, \n {predictedTranscript}. \n The IBM identifying confidence is, \n{predictionConfidence * 100} percent");

                    var gatherOptionsList = new List<Gather.InputEnum>
                    {
                        Gather.InputEnum.Dtmf
                    };
                    Session["CurrentVrn"] = predictedTranscript;
                    Session["VrnConfidence"] = predictionConfidence.ToString();
                    ////var transcript = string.Format(ConversationHelper.ConfirmedVrnQuestion, Utilities.AddSpaceBetweenChars(predictedTranscript));
                    var transcript = string.Format(ConversationHelper.ConfirmedVrnQuestion, predictedTranscript);
                    var gather = new Gather(
                        input: gatherOptionsList,
                        numDigits: 1,
                        language: ConversationHelper.GatherLanguage,
                        timeout: 5,
                        method: HttpMethod.Get,
                        action: Url.ActionUri("OnVrnGatherConfirmed", "Vrn")
                    );

                    Log4NetLogger.Info($"Gathered VRN {predictedTranscript} with confidence {predictionConfidence}");

                    gather.Say(transcript, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    response.Append(gather);

                    var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);

                    //var uri = GetRedirectUri("Vrn", "OnVrnGatherComplete", new Dictionary<string, string> { { "speechResult", speechResult }, { "confidence", confidence.ToString("0.0") } });
                    //response.Redirect(method: HttpMethod.Get, url: uri);

                    response.Say(ConversationHelper.VrnNotGathered);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                }
                else
                {
                    Session["CurrentVrn"] = null;
                    Session["VrnConfidence"] = null;

                    Log4NetLogger.Error("VRN Results not found");
                    response.Say(ConversationHelper.VrnNotGathered);

                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCurrentVrn", "Vrn"));
                }

                response.Say("Goodbye.");

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/Recorded", e);
                return TwiML(RejectCall());
            }
        }
    }
}