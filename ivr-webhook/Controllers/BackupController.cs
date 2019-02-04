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
}