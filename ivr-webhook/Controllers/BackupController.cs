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
    public class BackupController : TwilioController
    {
        //// GET: IVR
        //public ActionResult Index()
        //{
        //    //var urlBuilder =
        //    //    new System.UriBuilder(Request.Url.AbsoluteUri)
        //    //    {
        //    //        Path = Url.Action("TranscribeVrnRecorded"),

        //    //    };
        //    return View();
        //}

        //[HttpPost]
        //public TwiMLResult Welcome()
        //{
        //    var response = new VoiceResponse();
        //    try
        //    {
        //        response.Say(ConversationHelper.WelcomeMessage + ConversationHelper.InputVrn, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
        //        response.Record(method: Twilio.Http.HttpMethod.Post, maxLength: 60,
        //                        transcribe: true,
        //                        transcribeCallback: new Uri("https://35eb31e3.ngrok.io/Ivr/HandleRecordedVrn"),
        //                        finishOnKey: "*");
        //        response.Say("Transcription In Progress");
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.LogError(e, "Error within ivr/Welcome");
        //        response = RejectCall();
        //    }

        //    return TwiML(response);
        //}

        //[HttpPost]
        //public TwiMLResult HandleRecordedVrn()
        //{
        //    var response = new VoiceResponse();
        //    try
        //    {
        //        //var RecordingSid = Request.Params["RecordingSid"];
        //        //var RecordingUrl = Request.Params["RecordingUrl"];
        //        //ErrorLog.LogError(new Exception(), RecordingUrl + ' ' + RecordingSid);

        //        var result = GetTranscribedResult();
        //        var message =
        //            $"TranscriptionText = {result.TranscriptionText}";  // , Status = {result.TranscriptionStatus}, RecordingSid = {result.RecordingSid}, RecordingUrl = {result.RecordingUrl}
        //        ErrorLog.LogError(new Exception(), message);
        //        response.Say($"You said,\n {message}");
        //        response.Say(ConversationHelper.Bye);
        //        response.Hangup();
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.LogError(e, "Error within ivr/HandleRecordedVrn");
        //        response = RejectCall();
        //    }
        //    return TwiML(response);
        //}

        //[HttpPost]
        //public TwiMLResult HandleTranscribedVrn()
        //{
        //    var response = new VoiceResponse();
        //    try
        //    {
        //        //int milliseconds = 10000;
        //        //Thread.Sleep(milliseconds);
        //        var result = GetTranscribedResult();
        //        var message =
        //            $"TranscriptionText = {result.TranscriptionText}";  // , Status = {result.TranscriptionStatus}, RecordingSid = {result.RecordingSid}, RecordingUrl = {result.RecordingUrl}
        //        ErrorLog.LogError(new Exception(), message);

        //        response.Say("You said,\n {0}", result.TranscriptionText);
        //        //response.Play(new Uri(incomingCall.RecordingUrl), null, null);

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

        //private TranscribedResult GetTranscribedResult()
        //{
        //    try
        //    {
        //        var incomingCall = new TranscribedResult
        //        {
        //            AccountSid = Request.Params["AccountSid"],
        //            TranscriptionSid = Request.Params["Sid"],
        //            TranscriptionText = Request.Params["TranscriptionText"],
        //            TranscriptionUrl = Request.Params["Uri"],
        //            TranscriptionStatus = Request.Params["Status"],
        //            RecordingSid = Request.Params["RecordingSid"]



        //            //RecordingUrl = Request.Params["RecordingUrl"],
        //            //CallSid = Request.Params["CallSid"],

        //            //From = Request.Params["From"],
        //            //To = Request.Params["To"],
        //            //CallStatus = Request.Params["CallStatus"],
        //            //ApiVersion = Request.Params["ApiVersion"],
        //            //Direction = Request.Params["Direction"],
        //            //ForwardedFrom = Request.Params["ForwardedFrom"]
        //        };
        //        return incomingCall;
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorLog.LogError(e, "Error when converting transcribed text to model class");
        //        throw;
        //    }
        //}

        //private VoiceResponse RejectCall()
        //{
        //    var response = new VoiceResponse();
        //    var reject = new Reject("busy");
        //    //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
        //    response.Append(reject);
        //    return response;
        //}
    }

    ////// http://localhost:49843/elmah.axd
    ////// https://www.twilio.com/console/voice/calls/logs?startDate=2019-02-01+00%3A00%3A00&endDate=2019-02-28+23%3A59%3A59
    ////// https://www.twilio.com/console/voice/recordings/recording-logs
    ////public class IvrController : TwilioController
    ////{
    ////    public static string NgRokPath { get; } = " http://ba823225.ngrok.io/Ivr";

    ////    // GET: IVR
    ////    public ActionResult Index()
    ////    {
    ////        return View();
    ////    }

    ////    [HttpPost]
    ////    public ActionResult Welcome()
    ////    {
    ////        var response = new VoiceResponse();
    ////        try
    ////        {
    ////            response.Say("Please say \n", Say.VoiceEnum.Alice, 1, Say.LanguageEnum.EnGb);
    ////            // record and transcribe users voice 		
    ////            response.Record(
    ////                timeout: 30,
    ////                maxLength: 30,
    ////                playBeep: true,
    ////                action: new Uri(NgRokPath + "/RecordingCompleted"),
    ////                transcribe: true,
    ////                transcribeCallback: new Uri(NgRokPath + "/TranscribeCallback"),
    ////                finishOnKey: "*");

    ////            response.Say("I did not receive a recording");
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            ErrorLog.LogError(e, "Error within ivr/Welcome");
    ////            response = RejectCall();
    ////        }
    ////        return TwiML(response);
    ////    }

    ////    [HttpPost]
    ////    public TwiMLResult RecordingCompleted()
    ////    {
    ////        var response = new VoiceResponse();
    ////        try
    ////        {
    ////            var recordingSid = Request.Params["RecordingSid"];
    ////            var status = Request.Params["CallStatus"];
    ////            var duration = int.Parse(Request.Params["RecordingDuration"]);
    ////            var url = Request.Params["RecordingUrl"];

    ////            // reading the transcibed result
    ////            response.Say("Recoding Completed. Waiting for transcription, \n");

    ////            //ErrorLog.LogError(new Exception(), $"URL = {url}");
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            ErrorLog.LogError(e, "Error within ivr/RecordingCompleted");
    ////            response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    ////        }
    ////        return TwiML(response);
    ////    }


    ////    //https://www.twilio.com/docs/voice/tutorials/ivr-screening-csharp-mvc
    ////    [HttpPost]
    ////    public ActionResult TranscribeCallback(string CallSid, string TranscriptionText, string RecordingUrl)
    ////    {
    ////        var response = new VoiceResponse();
    ////        try
    ////        {
    ////            // get the transcribed result - https://www.twilio.com/docs/voice/twiml/record#transcribe
    ////            response.Say($"Transcribed message is,\n {TranscriptionText}");
    ////            ErrorLog.LogError(new Exception(), $"TranscriptionText = {TranscriptionText}, CallSid = {CallSid}, RecordingUrl = {RecordingUrl}");

    ////            // done
    ////            response.Say("Good Bye", Say.VoiceEnum.Alice, null, Say.LanguageEnum.EnGb);
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            ErrorLog.LogError(e, "Error within ivr/HandleTranscribedVrn");
    ////            response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    ////        }

    ////        response.Say("Goodbye");
    ////        response.Hangup();
    ////        return TwiML(response);
    ////    }

    ////    private VoiceResponse RejectCall()
    ////    {
    ////        var response = new VoiceResponse();
    ////        var reject = new Reject("busy");
    ////        //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
    ////        response.Append(reject);
    ////        return response;
    ////    }
    ////}


    // Transcribe working 
    //////// http://localhost:49843/elmah.axd
    //////// https://www.twilio.com/console/voice/calls/logs?startDate=2019-02-01+00%3A00%3A00&endDate=2019-02-28+23%3A59%3A59
    //////// https://www.twilio.com/console/voice/recordings/recording-logs
    //////public class IvrController : TwilioController
    //////{
    //////    public static string NgRokPath { get; } = " https://5b2491ec.ngrok.io/Ivr";

    //////    // GET: IVR
    //////    public ActionResult Index()
    //////    {
    //////        return View();
    //////    }

    //////    [HttpPost]
    //////    public ActionResult Welcome()
    //////    {
    //////        var response = new VoiceResponse();
    //////        try
    //////        {
    //////            //response.Say("Please say \n", Say.VoiceEnum.Alice, 1, Say.LanguageEnum.EnGb);
    //////            // record and transcribe users voice 		
    //////            //response.Record(
    //////            //    timeout: 30,
    //////            //    maxLength: 30,
    //////            //    playBeep: true,
    //////            //    action: new Uri(NgRokPath+ "/RecordingCompleted"),
    //////            //    transcribe: true,
    //////            //    transcribeCallback: new Uri(NgRokPath + "/TranscribeCallback"),
    //////            //    finishOnKey: "*");

    //////            var gatherOptionsList = new List<Gather.InputEnum>
    //////            {
    //////                Gather.InputEnum.Speech,
    //////                //Gather.InputEnum.Dtmf
    //////            };
    //////            var gather = new Gather(
    //////                input: gatherOptionsList,
    //////                timeout: 60,
    //////                finishOnKey: "*",
    //////                action: Url.ActionUri("OnGatherComplete", "Ivr")
    //////                );
    //////            gather.Say("Please say \n", Say.VoiceEnum.Alice, 1, Say.LanguageEnum.EnGb);
    //////            response.Append(gather);

    //////            //response.Say("I did not receive a recording");
    //////        }
    //////        catch (Exception e)
    //////        {
    //////            ErrorLog.LogError(e, "Error within ivr/Welcome");
    //////            response = RejectCall();
    //////        }
    //////        return TwiML(response);
    //////    }

    //////    [HttpPost]
    //////    public TwiMLResult OnGatherComplete(string SpeechResult, double Confidence)
    //////    {
    //////        var response = new VoiceResponse();
    //////        try
    //////        {
    //////            var identifyingConfidence = Math.Round(Confidence * 100, 2);
    //////            var transcript = $"You said {SpeechResult} with Confidence {identifyingConfidence}.\n Good Bye";
    //////            var say = new Say(transcript);
    //////            ErrorLog.LogError(new Exception(), transcript);
    //////            response.Append(say);
    //////        }
    //////        catch (Exception e)
    //////        {
    //////            ErrorLog.LogError(e, "Error within ivr/OnGatherComplete");
    //////            response = RejectCall();
    //////        }
    //////        return TwiML(response);
    //////    }

    //////    //[HttpPost]
    //////    //public TwiMLResult RecordingCompleted()
    //////    //{
    //////    //    var response = new VoiceResponse();
    //////    //    try
    //////    //    {
    //////    //        var recordingSid = Request.Params["RecordingSid"];
    //////    //        var status = Request.Params["CallStatus"];
    //////    //        var duration = int.Parse(Request.Params["RecordingDuration"]);
    //////    //        var url = Request.Params["RecordingUrl"];

    //////    //        // reading the transcibed result
    //////    //        response.Say("Recoding Completed. Waiting for transcription, \n");

    //////    //        //ErrorLog.LogError(new Exception(), $"URL = {url}");
    //////    //    }
    //////    //    catch (Exception e)
    //////    //    {
    //////    //        ErrorLog.LogError(e, "Error within ivr/RecordingCompleted");
    //////    //        response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    //////    //    }
    //////    //    return TwiML(response);
    //////    //}


    //////    ////https://www.twilio.com/docs/voice/tutorials/ivr-screening-csharp-mvc
    //////    //[HttpPost]
    //////    //public ActionResult TranscribeCallback(string CallSid, string TranscriptionText, string RecordingUrl) 
    //////    //{
    //////    //    var response = new VoiceResponse();
    //////    //    try
    //////    //    {
    //////    //        // get the transcribed result - https://www.twilio.com/docs/voice/twiml/record#transcribe
    //////    //        response.Say($"Transcribed message is,\n {TranscriptionText}");
    //////    //        ErrorLog.LogError(new Exception(), $"TranscriptionText = {TranscriptionText}, CallSid = {CallSid}, RecordingUrl = {RecordingUrl}");

    //////    //        // done
    //////    //        response.Say("Good Bye", Say.VoiceEnum.Alice, null, Say.LanguageEnum.EnGb);
    //////    //    }
    //////    //    catch (Exception e)
    //////    //    {
    //////    //        ErrorLog.LogError(e, "Error within ivr/HandleTranscribedVrn");
    //////    //        response.Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    //////    //    }

    //////    //    response.Say("Goodbye");
    //////    //    response.Hangup();
    //////    //    return TwiML(response);
    //////    //}

    //////    private VoiceResponse RejectCall()
    //////    {
    //////        var response = new VoiceResponse();
    //////        var reject = new Reject("busy");
    //////        //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
    //////        response.Append(reject);
    //////        return response;
    //////    }
    //////}
    

  

    ////namespace ivr_webhook.Controllers
    ////{
    ////    // http://localhost:49843/elmah.axd
    ////    // https://www.twilio.com/console/voice/calls/logs?startDate=2019-02-01+00%3A00%3A00&endDate=2019-02-28+23%3A59%3A59
    ////    // https://www.twilio.com/console/voice/recordings/recording-logs
    ////    public class IvrController : TwilioController
    ////    {
    ////        public static string NgRokPath { get; } = " https://5b2491ec.ngrok.io/Ivr";

    ////        // GET: IVR
    ////        public ActionResult Index()
    ////        {
    ////            return View();
    ////        }

    ////        [HttpPost]
    ////        public ActionResult Welcome()
    ////        {
    ////            var response = new VoiceResponse();
    ////            try
    ////            {
    ////                var gatherOptionsList = new List<Gather.InputEnum>
    ////            {
    ////                Gather.InputEnum.Speech,    // Speech input
    ////                //Gather.InputEnum.Dtmf
    ////            };

    ////                var gather = new Gather(
    ////                    input: gatherOptionsList,
    ////                    hints: "A, B, C, D,E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, one, two, three, four, five, size, seven, eight, nine, zero",   // 0,1,2,3,4,5,6,7,8,9
    ////                    speechTimeout: "Auto",
    ////                    finishOnKey: "*",
    ////                    action: Url.ActionUri("OnGatherComplete", "Ivr")    // will be called automaticaly when transcription is ready 
    ////                    );
    ////                //gather.Say("Please say \n", Say.VoiceEnum.Alice, 1, Say.LanguageEnum.EnGb);
    ////                gather.Say(ConversationHelper.InputVrn, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    ////                response.Append(gather);

    ////                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    ////                response.Append(say);
    ////            }
    ////            catch (Exception e)
    ////            {
    ////                ErrorLog.LogError(e, "Error within ivr/Welcome");
    ////                response = RejectCall();
    ////            }
    ////            return TwiML(response);
    ////        }

    ////        [HttpPost]
    ////        public TwiMLResult OnGatherComplete(string speechResult, double confidence)
    ////        {
    ////            var response = new VoiceResponse();
    ////            try
    ////            {
    ////                var identifyingConfidence = Math.Round(confidence * 100, 2);
    ////                var transcript = $"You said, \n {speechResult}, \n \n with Confidence {identifyingConfidence}.\n";
    ////                transcript += "again " + transcript + ", Good Bye";
    ////                var say = new Say(transcript, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
    ////                ErrorLog.LogError(new Exception(), transcript);
    ////                response.Append(say);
    ////            }
    ////            catch (Exception e)
    ////            {
    ////                ErrorLog.LogError(e, "Error within ivr/OnGatherComplete");
    ////                response = RejectCall();
    ////            }
    ////            return TwiML(response);
    ////        }



    ////        private VoiceResponse RejectCall()
    ////        {
    ////            var response = new VoiceResponse();
    ////            var reject = new Reject("busy");
    ////            //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Reject reason=\"busy\" /></Response>");
    ////            response.Append(reject);
    ////            return response;
    ////        }
    ////    }
    ////}

}