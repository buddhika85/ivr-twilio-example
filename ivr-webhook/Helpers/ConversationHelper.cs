using Twilio.TwiML.Voice;

namespace ivr_webhook.Helpers
{
    public static class ConversationHelper
    {
        public static Say.LanguageEnum SpeakLanguage = Say.LanguageEnum.EnGb;
        public static Say.VoiceEnum SpeakVoice = Say.VoiceEnum.PollyMatthew;

        public static Gather.LanguageEnum GatherLanguage = Gather.LanguageEnum.EnGb;

        public static string WelcomeMessage = "welcome \n";//"Welcome to Advanced Parking Systems.\n";

        public static string InputVrn = "Please say your vehicle registration number. \nAnd press the star key when finished.";
        // "VRN beep \n star";

        public static string AutheticationError = "Sorry, Twilio authentication failed";
        public static string NothingReceived = "I did not receive a recording";

        public static string Bye = "Good Bye";
    }
}