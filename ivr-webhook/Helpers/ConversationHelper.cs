using Twilio.TwiML.Voice;

namespace ivr_webhook.Helpers
{
    public static class ConversationHelper
    {
        public static Say.LanguageEnum SpeakLanguage = Say.LanguageEnum.EnGb;
        public static Say.VoiceEnum SpeakVoice = Say.VoiceEnum.Alice;

        public static string WelcomeMessage = "welcome \n";//"Welcome to Advanced Parking Systems.\n";

        public static string InputVrn = "VRN beep \n star";
            //"To start, please tell us your vehicle registration number after the beep.\n And Press the star key when finished.";
        public static string AutheticationError = "Sorry, Twilio authentication failed";
        public static string NothingReceived = "I did not receive a recording";

        public static string Bye = "Good Bye";
    }
}