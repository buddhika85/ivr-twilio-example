using ivr_webhook.Helpers;
using System;
using System.Threading;
using System.Web.Mvc;
using ivr_webhook.Models;
using Twilio;
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
        // GET: IVR
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public TwiMLResult Welcome()
        //{
        //    var response = new VoiceResponse();
        //    try
        //    {
        //        response.Say("Please say your user Id, example ABC123, \n and press star when done", Say.VoiceEnum.Alice, null, Say.LanguageEnum.EnGb);
        //        // record and transcribe users voice 
        //        // https://www.twilio.com/docs/voice/twiml/record?code-sample=code-record-a-voicemail&code-language=C%23&code-sdk-version=5.x
        //        response.Record(
        //            transcribe: true,
        //            transcribeCallback: new Uri("https://35eb31e3.ngrok.io/Ivr/HandleRecordedVrn"),
        //            finishOnKey: "*");
        //        response.Say("I did not receive a recording");
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.LogError(e, "Error within ivr/Welcome");
        //        response = RejectCall();
        //    }

        //    return TwiML(response);
        //}

        [HttpPost]
        public ActionResult Welcome()
        {
            var response = new VoiceResponse();
            try
            {
                response.Say("Please say \n", Say.VoiceEnum.Alice, 1, Say.LanguageEnum.EnGb);
                // record and transcribe users voice 		
                response.Record(
                    //timeout: 30,
                    //maxLength: 30,
                    playBeep: true,
                    action: new Uri("http://8949de9d.ngrok.io/Ivr/RecordingCompleted"),
                    transcribe: true,
                    transcribeCallback: new Uri("http://8949de9d.ngrok.io/Ivr/HandleTranscribedVrn"),

                    finishOnKey: "*");

                response.Say("I did not receive a recording");

                //string uri = new Uri("http://8949de9d.ngrok.io/Ivr/HandleTranscribedVrn").AbsoluteUri;
                //return Content(string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Say>Record your message at the beep</Say><Record maxLength=\"60\" timeout=\"10\" transcribeCallback=\"{0}\" transcribe=\"true\"/></Response>", uri));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/Welcome");
                //response = RejectCall();
                return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Say>exception</Say></Response>");
            }

            return TwiML(response);
        }

        [HttpPost]
        public TwiMLResult RecordingCompleted()
        {
            var response = new VoiceResponse();
            try
            {
                var RecordingSid = Request.Params["RecordingSid"];
                var Status = Request.Params["CallStatus"];
                var Duration = int.Parse(Request.Params["RecordingDuration"]);
                var Url = Request.Params["RecordingUrl"];

                // reading the transcibed result
                response.Say("Recoding Completed. You said, \n");
                response.Play(new Uri(Url));
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/RecordingCompleted");
                response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
            }
            return TwiML(response);
        }


        //https://www.twilio.com/docs/voice/tutorials/ivr-screening-csharp-mvc
        [HttpPost]
        public TwiMLResult HandleTranscribedVrn()
        {
            var response = new VoiceResponse();
            try
            {
                // get the transcribed result - https://www.twilio.com/docs/voice/twiml/record#transcribe
                var result = new TranscribedResult
                {
                    TranscriptionSid = Request.Params["TranscriptionSid"],
                    TranscriptionText = Request.Params["TranscriptionText"],
                    TranscriptionUrl = Request.Params["TranscriptionUrl"],
                    TranscriptionStatus = Request.Params["TranscriptionStatus"],
                    RecordingSid = Request.Params["RecordingSid"],
                    RecordingUrl = Request.Params["RecordingUrl"],
                    AccountSid = Request.Params["AccountSid"]
                };

           
                response.Say($"Transcribed message is,\n {result.TranscriptionText}");
                
                // done
                response.Say("Good Bye", Say.VoiceEnum.Alice, null, Say.LanguageEnum.EnGb);
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/HandleTranscribedVrn");
                response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
            }

            response.Hangup();
            return TwiML(response);
        }


        [HttpPost]
        public TwiMLResult HandleRecordedVrn()
        {
            var response = new VoiceResponse();
            try
            {
                var result = GetTranscribedResult();
                var message =
                    $"TranscriptionText = {result.TranscriptionText}";  // , Status = {result.TranscriptionStatus}, RecordingSid = {result.RecordingSid}, RecordingUrl = {result.RecordingUrl}
                ErrorLog.LogError(new Exception(), message);
                response.Say($"You said,\n {message}");
                response.Say(ConversationHelper.Bye);
                response.Hangup();
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error within ivr/HandleRecordedVrn");
                response = RejectCall();
            }
            return TwiML(response);
        }

        //[HttpPost]
        //public TwiMLResult HandleTranscribedVrn()
        //{
        //    var response = new VoiceResponse();
        //    try
        //    {
        //        // get the transcribed result - https://www.twilio.com/docs/voice/twiml/record#transcribe
        //        var result = new TranscribedResult
        //        {
        //            TranscriptionSid = Request.Params["TranscriptionSid"],
        //            TranscriptionText = Request.Params["TranscriptionText"],
        //            TranscriptionUrl = Request.Params["TranscriptionUrl"],
        //            TranscriptionStatus = Request.Params["TranscriptionStatus"],
        //            RecordingSid = Request.Params["RecordingSid"],
        //            RecordingUrl = Request.Params["RecordingUrl"],
        //            AccountSid = Request.Params["AccountSid"]
        //        };
               
        //        // reading the transcibed result
        //        response.Say("You said,\n {0}", result.TranscriptionText);
               
        //        // done
        //        response.Say(ConversationHelper.Bye, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.LogError(e, "Error within ivr/HandleTranscribedVrn");
        //        response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
        //    }
        //    return TwiML(response);
        //}

        private TranscribedResult GetTranscribedResult()
        {
            try
            {
                var incomingCall = new TranscribedResult
                {
                    AccountSid = Request.Params["AccountSid"],
                    TranscriptionSid = Request.Params["Sid"],
                    TranscriptionText = Request.Params["TranscriptionText"],
                    TranscriptionUrl = Request.Params["Uri"],
                    TranscriptionStatus = Request.Params["Status"],
                    RecordingSid = Request.Params["RecordingSid"]



                    //RecordingUrl = Request.Params["RecordingUrl"],
                    //CallSid = Request.Params["CallSid"],
                   
                    //From = Request.Params["From"],
                    //To = Request.Params["To"],
                    //CallStatus = Request.Params["CallStatus"],
                    //ApiVersion = Request.Params["ApiVersion"],
                    //Direction = Request.Params["Direction"],
                    //ForwardedFrom = Request.Params["ForwardedFrom"]
                };
                return incomingCall;
            }
            catch (Exception e)
            {
                ErrorLog.LogError(e, "Error when converting transcribed text to model class");
                throw;
            }
        }

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