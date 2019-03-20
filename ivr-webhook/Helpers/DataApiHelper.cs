using System;
using System.Linq;

namespace ivr_webhook.Helpers
{
    /// <summary>
    /// Performs conditional checks in the IVR flow
    /// </summary>
    public class DataApiHelper
    {
        public bool IsRegisteredUser(string incomingPhoneNumber)
        {
            try
            {
                // TO DO - add business logic

                return false;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/IsRegisteredUser", e);
                throw;
            }
        }

        /// <summary>
        /// Returns empty string if a new user/unregistered
        /// </summary>
        /// <param name="incomingPhoneNumber"></param>
        /// <returns></returns>
        public string GetCallerNameByPhoneNumber(string incomingPhoneNumber)
        {
            try
            {
                // TO DO - add business logic

                return "John";
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/GetCallerNameByPhoneNumber", e);
                throw;
            }
        }

        /// <summary>
        /// Returns null if the user does not have a parking history
        /// </summary>
        /// <param name="incomingPhoneNumber"></param>
        /// <returns></returns>
        public string GetLastparkingLocation(string incomingPhoneNumber)
        {
            try
            {
                // TO DO - add business logic

                return "1234";
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/GetLastparkingLocation", e);
                throw;
            }
        }

        /// <summary>
        /// Validates the existance of the parking location
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public bool IsParkingLocationValid(string locationCode)
        {
            try
            {
                // TO DO - add business logic

                return true;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/IsParkingLocationValid", e);
                throw;
            }
        }


        /// <summary>
        /// Gets the default VRN
        /// </summary>
        /// <param name="incomingPhoneNumber"></param>
        /// <returns></returns>
        public string GetDefaultCarVrn(string incomingPhoneNumber)
        {
            try
            {
                // TO DO - add business logic

                return "ABCD123";
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/GetDefaultCarVrn", e);
                throw;
            }
        }

        /// <summary>
        /// Returns some sample VRNs to be used as hints for voice recogintion
        /// </summary>
        /// <returns></returns>
        public string GetVrnHints()
        {
            return
                "L R 5 9 U X P, N C 5 8 E R U, R X 5 1 N N U, E J O 4 C O U, O E 0 3 0 X M, E X 6 2 H H C, E U 1 4 T N K, J 1 U X Y, G V 1 4 A U O, Y E 5 8 K J A, G M 1 6 5 0 Y F, M 9 A U T, Y G 6 1 Y G E, F L I 8 X U D," +
                "E T 6 5 P V N, G 4 3 7 A J M, G D 6 2 O T X, K N 5 3 V T D, F L 6 5 D X V, G L 6 4 E W Z, K M 6 6 T X F, A F 6 4 K H Y, B T 6 3 K 0, L F 1 4 W T X, A F 6 4 B H A, R J 1 4 O W C, K U 1 7 C Y X, G L 6 4, R J 1 4 O W O, M H 1 6 F B F," + 
                "Y F 0 8 V F Y, V U 1 0 P X E,  C 0 6 D F G, E X 6 5 Z D Z, B T 6 3 K O H, H G 1 2 V G T, F 6 4 B H A, D P 1 4 Y L U, L G 6 0 E E M, E O 1 0 Z T H, I 0 Z T H, A X 1 4 E U C, A F 6 6 W J G, B J 6 4 U D E, Y K 0 3 O P C, S 5 0 1 T P P, A F 1 7 X U T," + 
                "A R 0 7 K H Y, 0 1 0 Z T H, B D 1 0 C M Z, B 4 5 6 L K N, B K 5 6 L K N, C T R 0 E N, A F 1 7 Y P H, K G 0 2 O F A, F E 1 5 X G U, F I 7 X U T, I 7 X U T A F, A F I 7 X U T, I 7 X U T, Y K O 3 0, A K 6 4 U L U, L S 6 2 O M O, P K 5 5 F Z F, Y H 6 1 X F P," + 
                "D H 1 6 M X C, S 1 0 0 A H P, Y X 1 2 W D A, K T 0 3 G S Y, Y B 0 7 C X X, R 5 5 5 B A X, K N 6 4 Z F O, K S 6 6 G R F, K S 0 5 O C E, V A 1 4 F U P, K A 0 3 K K Z, S D 1 1 T Z T, A X 0 8 A U C, A X 0 8 A U, S B 5 6 H R J, B T 6 0 Z K K, F H 6 3 J U J, A E 1 6 Y N Z," +
                "W J 5 7 W H G, Y M 6 6 X S W, V N 1 2 A V R, K X 0 3 K N C, B P 0 2 T G F, F X 6 3 Y J N, N V 5 9 D D N, K U I 7, L I G 1 3 5 6, V N 6 0 C F E, F M 1 0 D D E, L B 5 5 O X E, 6 4 B H A, F L 1 4 D V T, A J 0 2 K N C, D P 1 4 Y L, K S 0 9 O P H, Y 9 0 3 J R O, A K 1 3 B X H," +
                "A F 1 7 X V L, A F 1 7 X U X, L S 5 7 W T T, A M 5 2 L N D, K V 5 4 U J U, Y E 1 3 W S Y, A K 1 4 U V D, D P 6 5 S Z T, R K I 6 A W, K F 1 5 Z P K, M W 0 7 X A F, K M 6 2 E O Y";


        }

        /// <summary>
        /// Checks if the parking time is limited 
        /// </summary>
        /// <param name="parkingLocationCode"></param>
        /// <returns></returns>
        public bool IsLimitedParkTime(string parkingLocationCode)
        {
            try
            {
                // TO DO - add business logic

                return true;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/IsLimitedParkTime", e);
                throw;
            }
        }

        /// <summary>
        /// Default max parking time
        /// </summary>
        /// <returns></returns>
        public int GetDefaultMaxParkingHours()
        {
            try
            {
                // TO DO - add business logic

                return 8;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/GetDefaultMaxParkingHours", e);
                throw;
            }
        }

        /// <summary>
        /// Returns available parking time for car park
        /// </summary>
        /// <param name="parkingLocationCode"></param>
        /// <returns></returns>
        public int GetAvailableParkingTimeForPark(string parkingLocationCode)
        {
            try
            {
                // TO DO - add business logic

                return 6;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/GetAvailableParkingTimeForPark", e);
                throw;
            }
        }

        /// <summary>
        /// Is max time reached at the current parking location?
        /// </summary>
        /// <param name="parkingLocationCode"></param>
        /// <returns></returns>
        public bool IsMaxTimeReached(string parkingLocationCode)
        {
            try
            {
                // TO DO - add business logic

                return false;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/IsMaxTimeReached", e);
                throw;
            }
        }

        /// <summary>
        /// Returns parking hourly rate
        /// </summary>
        /// <param name="parkingLocationCode"></param>
        /// <returns></returns>
        public double GetParkingHourlyRate(string parkingLocationCode)
        {
            try
            {
                // TO DO - add business logic

                return 12;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/IsMaxTimeReached", e);
                throw;
            }
        }

        /// <summary>
        /// Returns users last 4 digits of the default card
        /// </summary>
        /// <param name="incomingPhoneNumber"></param>
        /// <returns></returns>
        public string GetUserDefaultCardEnding(string incomingPhoneNumber)
        {
            try
            {
                // TO DO - add business logic

                return "1234";
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/UserDefaultCardEnding", e);
                throw;
            }
        }

        /// <summary>
        /// validates a card number entered by the user
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool ValidateCardNumber(string digits)
        {
            try
            {
                // TO DO - add business logic

                digits = digits.Trim();
                //return digits.Count() == 16;
                return true;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/ValidateCardNumber", e);
                throw;
            }
        }

        /// <summary>
        /// Validate card expiry date
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool ValidateCardExpiryDate(string digits)
        {
            try
            {
                // TO DO - add business logic

                return true;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/ValidateCardExpiryDate", e);
                throw;
            }
        }

        /// <summary>
        /// Vaidate 3 digit cvv number
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public bool ValidateCvvNumber(string digits)
        {
            try
            {
                // TO DO - add business logic

                digits = digits.Trim();
                return digits.Count() == 3;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/ValidateCvvNumber", e);
                throw;
            }
        }

        /// <summary>
        /// Process payment
        /// </summary>
        /// <param name="callSid"></param>
        /// <param name="incomingCallerNumber"></param>
        /// <param name="isRegisteredUser"></param>
        /// <param name="requestedParkingLocation"></param>
        /// <param name="currentVrn"></param>
        /// <param name="hoursConfirmed"></param>
        /// <param name="hourlyRate"></param>
        /// <param name="parkingCost"></param>
        /// <param name="useDefaultCard"></param>
        /// <param name="userDefaultCardEnding"></param>
        /// <param name="cardNumberConfirmed"></param>
        /// <param name="cardExpiryDateConfirmed"></param>
        /// <param name="cvvNumberConfirmed"></param>
        /// <param name="retryPaymentCount"></param>
        /// <returns></returns>
        public bool ProcessPayment(string callSid, string incomingCallerNumber, string isRegisteredUser, string requestedParkingLocation, 
            string currentVrn, int hoursConfirmed, double hourlyRate, double parkingCost
            ,
            string useDefaultCard, int? userDefaultCardEnding, string cardNumberConfirmed, int cardExpiryDateConfirmed,
            int cvvNumberConfirmed, int? retryPaymentCount
            )
        {
            try
            {
                // TO DO - add business logic

                Log4NetLogger.Info("\n\nProcessing the payment\n" +
                                   $"callSid - {callSid}\n" +
                                   $"incomingCallerNumber - {incomingCallerNumber}\n" +
                                   $"isRegisteredUser - {isRegisteredUser}\n" +
                                   $"requestedParkingLocation - {requestedParkingLocation}\n" +
                                   $"currentVrn - {currentVrn}\n" +
                                   $"hoursConfirmed - {hoursConfirmed}\n" +
                                   $"hourlyRate - {hourlyRate}\n" +
                                   $"parkingCost - {parkingCost}\n" +
                                   $"useDefaultCard - {useDefaultCard}\n" +
                                   $"userDefaultCardEnding - {userDefaultCardEnding}\n" +
                                   $"cardNumberConfirmed - {cardNumberConfirmed}\n" +
                                   $"cardExpiryDateConfirmed - {cardExpiryDateConfirmed} \n" +
                                   $"cvvNumberConfirmed - {cvvNumberConfirmed} \n" +
                                   $"retryPaymentCount - {retryPaymentCount}\n" +
                                   $"End Payment Information, Stating payment process\n\n");

                return true;
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in DataApiHelper/ProcessPayment", e);
                throw;
            }
        }
    }
}