using ivr_webhook.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace ivr_webhook.Controllers
{
    public class IBMWatsonController : BaseController
    {
        [HttpPost]
        public ActionResult Index()
        {
            try
            {
                var response = new VoiceResponse();
                response.Say("VRN Please");
                response.Record(maxLength: 10, action: Url.ActionUri("Recorded", "IBMWatson"));
                response.Hangup();

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IBMWatsonController/Index", e);
                return TwiML(RejectCall());
            }
        }

        public class BodyData
        {
            public string RecordingUrl { get; set; }
        }

        [HttpPost]
        public ActionResult Recorded(BodyData bodyData)
        {
            try
            {
                string recordingUrl = bodyData.RecordingUrl;
                Log4NetLogger.Debug($"IBM - {recordingUrl}");

                var response = new VoiceResponse();
                response.Say("Thanks for howling... take a listen to what you howled.");
                response.Play(new Uri(recordingUrl));
                response.Say("Goodbye.");

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/Recorded", e);
                return TwiML(RejectCall());
            }
        }


        [HttpPost]
        public ActionResult TranscriptionGetter(HttpResponseClasses.BodyData bodyData)
        {
            try
            {
                Log4NetLogger.Info("Inside - TranscriptionGetter");
                var data = bodyData.AddOns;
                var addOns = JsonConvert.DeserializeObject<HttpResponseClasses.RootObject>(data);
                if (addOns.results.ibm_watson_speechtotext.ToString() == "")
                {
                    return Content("Add Watson Speech to Text add-on in your Twilio console");
                }

                var payloadUrl = addOns.results.ibm_watson_speechtotext.payload[0].url;
                var accountSid = "ACdfe342ddbecf65b044a7e098180a75e3"; //ConfigurationManager.AppSettings["TwilioAccountSid"];
                var apiKey = "b8fd816074dfd4f4b6a63c1289606cd3"; //ConfigurationManager.AppSettings["TwilioAuthToken"];

                var reqCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(accountSid + ":" + apiKey));
                var request = (HttpWebRequest)WebRequest.Create(payloadUrl);
                request.Headers.Add("Authorization", "Basic " + reqCredentials);
                var response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamreader = new StreamReader(stream);
                var responseBody = streamreader.ReadToEnd();

                var results = JsonConvert.DeserializeObject<WatsonResponse.RootObject>(responseBody).results[0].results;
                var transcripts = string.Join("",
                    results.ConvertAll<string>(item => item.alternatives[0].transcript).ToArray()
                );

                Log4NetLogger.Info($"IBM Transcription- {transcripts}");

                var responselast = new VoiceResponse();
                responselast.Say(transcripts);
                return TwiML(responselast);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in IvrFlowController/TranscriptionGetter", e);
                return TwiML(RejectCall());
            }
        }
    }
}