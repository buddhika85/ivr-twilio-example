using System;
using System.ComponentModel.DataAnnotations;

namespace ivr_webhook.Models
{
    /// <summary>
    /// Ref - https://www.twilio.com/docs/voice/twiml/record#transcribe
    /// </summary>
    public class TranscribedResult
    {
        public string TranscriptionSid { get; set; }
        public string TranscriptionText { get; set; }
        public string TranscriptionStatus { get; set; }
        public string TranscriptionUrl { get; set; }
        public string RecordingSid { get; set; }
        public string RecordingUrl { get; set; }
        public string CallSid { get; set; }
        public string AccountSid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CallStatus { get; set; }
        public string ApiVersion { get; set; }
        public string Direction { get; set; }
        public string ForwardedFrom { get; set; }
    }
}