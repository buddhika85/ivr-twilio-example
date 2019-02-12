using System.Runtime.CompilerServices;
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
        public static string NothingReceived = "I did not receive a response";

        public static string Bye = "Good Bye";

        public static string ConfirmedVrnQuestion = "You said, \n {0}, \nTo continue press star on your keypad, \nOr to retry press any other key";
        public static string ConfirmedVrnMessage = "You confirmed your VRN which is {0}";

        public static string ConfirmedVMakerQuestion = "You said, \n {0} as Maker, \nTo continue press star on your keypad, \nOr to retry press any other key";
        public static string ConfirmedVMakerMessage = "You confirmed maker which is {0}";

        public static string ConfirmedVColorQuestion = "You said, \n {0} as Color, \nTo continue press star on your keypad, \nOr to retry press any other key";

        public static string Summary = "Summary. \n \nV R N is, {0}. \n Maker is, {1}.\n, Color is, {2}. \nGoodBye";

    }
}