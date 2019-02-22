using System.Net;
using System.Runtime.CompilerServices;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Helpers
{
    public static class ConversationHelper
    {
        public static Say.LanguageEnum SpeakLanguage = Say.LanguageEnum.EnGb;
        public static Say.VoiceEnum SpeakVoice = Say.VoiceEnum.PollyMatthew;

        public static Gather.LanguageEnum GatherLanguage = Gather.LanguageEnum.EnGb;

        public static string WelcomeMessage = "Welcome.";//"Welcome to Advanced Parking Systems.\n";

        public static string InputVrn = "Please say your vehicle registration number. \nAnd press the star key when finished.";
        // "VRN beep \n star";

        public static string AutheticationError = "Sorry, Twilio authentication failed";
        public static string NothingReceived = "I did not receive a response";

        public static string Bye = "Good Bye";

        public static string ConfirmedVrnQuestion = "You said, \n {0} as V R N, \nTo continue press star on your keypad, \nOr to retry press any other key";
        public static string ConfirmedVrnMessage = "You confirmed your VRN which is {0}";

        public static string ConfirmedVMakerQuestion = "You said, \n {0} as Maker, \nTo continue press star on your keypad, \nOr to retry press any other key";
        public static string ConfirmedVMakerMessage = "You confirmed maker which is {0}";

        public static string ConfirmedVColorQuestion = "You said, \n {0} as Color, \nTo continue press star on your keypad, \nOr to retry press any other key";

        public static string Summary = "Summary. \n \nV R N is, {0}. \n Maker is, {1}.\n, Color is, {2}. \nGoodBye";


        //

        public static string CallbackNonePrivate = "Hello. \nPlease call back from a non private phone. \nGood Bye.";
        public static string GoodBye = "GoodBye";

        public static string WelcomeNewUser = "Hello, Welcome Guest.";
        public static string WelcomeRegisteredUser = "Hello, Welcome Back {0}.";

        public static string GetLocationCode = "Enter Location Code via Keypad. \nAnd press the star key when finished.";
        public static string ParkAtSameLocationCode = "To Park at previouse location which is, {0}, press star on your keypad, \nOr, to enter a new location code, press any other key";
        public static string InputUnidentified = "Sorry, We did not recognize your input";
        public static string ParkingLocationCodeInvalid = "Sorry, We did not recognize the parking location code, {0}";
        public static string ConfirmedParkingLocationCodeQuestion = "You requested to Park At, \n {0}, \nParking Location Code. \nTo continue press star on your keypad, \nOr to re enter press any other key";
        public static string FlowSummary = "Summary. \n \nCaller is, {0}. \n Location Code is, {1}. \nV R N is, {2}. \n Number of hours confirmed is, {3}. \nGoodBye";

        public static string UseDefaultVrn = "To use default VRN, which is, {0}, press star on your keypad, \nOr, to re enter a new VRN, press any other key";

        public static string SayVrn = "Please say the V R N clearly, \nstating each letter, and, number. \n\nAnd press the star key when finished.";
        public static string SayVrnAgain = "Please say the V R N again, \nstating each letter, and, number. \n\nAnd press the star key when finished.";

        public static string DidNotRecoginizeVrn = "Sorry. \nI did not recognize your VRN input.";

        public static string EnterParkTime =  "Enter the Number of Hours to Park, via Keypad. \nAnd press the star key when finished.";
        public static string MaxTimeReached = "Sorry. \nYou cannot park at the location, {0}, \n at this time. \nGoodBye";
        public static string MaxHoursYouCanStay = "You can stay up to, {0} hours at location code\n, {1}.\n";
        public static string ErrorInGettingParkingTime = "Sorry, Error occured when allocating your car parking space.\n";
        public static string InvalidHourRequest = "You have enetered {0} hours which is invalid.\n";
        public static string TooManyHourRequest = "You have enetered, {0} hours, which exceeds the maximum limit.\n";
        public static string ConfirmedParkingHours = "You requested a parking space for , {0} hours. \nTo confirm and continue press star on your keypad, \nOr to re enter press any other key";

        public static string ParkCostAndParkUntil = "Your parking permit cost is, \n{0}, {1}. \nAnd You can park until, \n{2}.";
        public static string CurrencyText = "Pounds";
        public static string UseDafaultCardOrNot = "To pay using your default card, which ends with, \n {0}, \npress star on your keypad, \nOr, to pay with a different card press any other key";
        public static string GetCardNumber = "Please enter your card number ignoring spaces, via Keypad. \nAnd press the star key when finished.";
        public static string CardNumberConfirm = "You provided, \n{0}, \nas your card number. \nTo confirm and continue press star on your keypad, \nOr to re enter press any other key";
        public static string CardNumberInvalid = "Sorry, The card number you provided which is, \n{0}, \nis invalid.";
        public static string GetCardExpDate = "Enter 4 digit card expiry date, via Keypad. \nAnd press the star key when finished.";
        public static string CardExpiryDateConfirm = "You entered , {0} , \n as the card expiry date. \nTo confirm and continue, press the star on your keypad, \nOr to re enter press any other key";
        public static string CardExpiryInvalid = "Sorry, The card expiry date you provided which is, \n{0}, \nis invalid.";
        public static string GetCvvNumber = "Please enter 3 digit CVV number, via Keypad. \nAnd press the star key when finished.";
        public static string CvvNumberConfirm = "You provided, \n{0}, as the CVV number. \nTo confirm and continue press star on your keypad, \nOr to re enter press any other key";
        public static string ProcessingPayment = "Please wait a few seconds as we are process the payment with your confirmed card details.";
        public static string PaymentComplete = "Thank you. \nPayment successful. \nYou can park until, {0}. \nGood Bye";
        public static string PaymentUnsuccessful = "Sorry, Your card payment unsuccessful. \nTo retry, press the star on your keypad, \nOr to end the call press any other key .";
        public static string NotRetryPayment = "You opted not to retry the unsuccessful payment.\n Good Bye.";
    }
}