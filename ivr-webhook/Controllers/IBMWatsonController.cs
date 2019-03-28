using ivr_webhook.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using IBM.WatsonDeveloperCloud.SpeechToText.v1;
using IBM.WatsonDeveloperCloud.Util;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace ivr_webhook.Controllers
{
    //https://www.twilio.com/docs/add-ons/tutorials/how-to-use-recordings-add-ons-in-csharp
    //https://stackoverflow.com/questions/7929013/making-a-curl-call-in-c-sharp
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

       

        [HttpPost]
        public ActionResult Recorded(BodyData bodyData)
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
                        keywords: new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" }, 
                        keywordsThreshold: 0.5f, model: "en-US_NarrowbandModel");


                if (results?.Results[0]?.Alternatives[0]?.Confidence != null)
                {
                    Log4NetLogger.Info($"IBM Confidence : {results.Results[0].Alternatives[0].Confidence} \n IBM Transcript : {results.Results[0].Alternatives[0].Transcript}");
                    response.Say($"You said, \n {results.Results[0].Alternatives[0].Transcript}. \n And identifying confidence {results.Results[0].Alternatives[0].Confidence * 100} percent");
                }
                else
                {
                    Log4NetLogger.Error("Results not found");
                    response.Say("Results not found.");
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




        //[HttpPost]
        //public ActionResult TranscriptionGetter(HttpResponseClasses.BodyData bodyData)
        //{
        //    try
        //    {
        //        Log4NetLogger.Info("Inside - TranscriptionGetter");
        //        var data = bodyData.AddOns;
        //        var addOns = JsonConvert.DeserializeObject<HttpResponseClasses.RootObject>(data);
        //        if (addOns.results.ibm_watson_speechtotext.ToString() == "")
        //        {
        //            return Content("Add Watson Speech to Text add-on in your Twilio console");
        //        }

        //        var payloadUrl = addOns.results.ibm_watson_speechtotext.payload[0].url;
        //        var accountSid = ConfigurationManager.AppSettings["twillioSid"];
        //        var apiKey = ConfigurationManager.AppSettings["twillioApiKey"];

        //        var reqCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(accountSid + ":" + apiKey));
        //        var request = (HttpWebRequest)WebRequest.Create(payloadUrl);
        //        request.Headers.Add("Authorization", "Basic " + reqCredentials);
        //        var response = (HttpWebResponse)request.GetResponse();
        //        Stream stream = response.GetResponseStream();
        //        StreamReader streamreader = new StreamReader(stream);
        //        var responseBody = streamreader.ReadToEnd();

        //        var results = JsonConvert.DeserializeObject<WatsonResponse.RootObject>(responseBody).results[0].results;
        //        var transcripts = string.Join("",
        //            results.ConvertAll<string>(item => item.alternatives[0].transcript).ToArray()
        //        );

        //        Log4NetLogger.Info($"IBM Transcription- {transcripts}");

        //        var responselast = new VoiceResponse();
        //        responselast.Say(transcripts);
        //        return TwiML(responselast);
        //    }
        //    catch (Exception e)
        //    {
        //        Log4NetLogger.Error("Exception in IvrFlowController/TranscriptionGetter", e);
        //        return TwiML(RejectCall());
        //    }
        //}
    }
}