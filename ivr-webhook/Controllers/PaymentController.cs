using ivr_webhook.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Twilio.AspNet.Mvc;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace ivr_webhook.Controllers
{
    public class PaymentController : BaseController
    {

        #region card number
        /// <summary>
        /// inform payment information to the caller
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPaymentInfo()
        {
            var response = new VoiceResponse();
            try
            {
                var hourlyRate = ApiHelper.GetParkingHourlyRate(Session["RequestedParkingLocation"] as string);
                Session["HourlyRate"] = hourlyRate;
                Log4NetLogger.Info($"Hourly Rate - {Session["HourlyRate"]}");

                var parkingCost = Math.Round(hourlyRate * ((int) Session["HoursRequested"]), 2);
                Session["ParkingCost"] = parkingCost;
                Log4NetLogger.Info($"Parking Cost - {Session["ParkingCost"]}");

                var validUntilTime = DateTime.Now.AddHours((int)Session["HoursConfirmed"]).ToString("h:mm tt");
                Session["ParkUntil"] = validUntilTime;
                Log4NetLogger.Info($"Park Until- {Session["ParkUntil"]}");
                var say = new Say(
                    string.Format(ConversationHelper.ParkCostAndParkUntil, parkingCost, ConversationHelper.CurrencyText,
                        validUntilTime), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);

                var isRegistered = Session["IsRegisteredUser"] as string == "yes";
                if (isRegistered)
                {
                    var userDefaultCardEnding = ApiHelper.GetUserDefaultCardEnding(Session["IncomingCallerNumber"] as string);
                    Session["UserDefaultCardEnding"] = userDefaultCardEnding;
                    if (string.IsNullOrWhiteSpace(userDefaultCardEnding))
                    {
                        Session["UseDefaultCard"] = "no";
                        // get card info
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                    }
                    else
                    {
                        // use default card?
                        response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("UseDafaultCardOrNot", "Payment"));
                    }
                }
                else
                {
                    // get card info
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                }

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/GetPaymentInfo", e);
                throw;
            }
        }

        /// <summary>
        /// Get card number as no default card
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCardNumber()
        {
            try
            {
                var response = new VoiceResponse();
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                var gather = new Gather(
                    input: gatherOptionsList,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 8,
                    method: HttpMethod.Get,
                    numDigits: 17,
                    finishOnKey: "*",
                    action: Url.ActionUri("OnCardNumberEnetered", "Payment")
                );
                gather.Say(ConversationHelper.GetCardNumber, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/GetCardNumber", e);
                throw;
            }
        }

        /// <summary>
        /// Validate and confirm the card number
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult OnCardNumberEnetered(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                // validate card number
                var isValid = ApiHelper.ValidateCardNumber(digits);
                if (isValid)
                {
                    Session["CardNumberEntered"] = digits.Trim();
                    Session["CardNumberConfirmed"] = null;
                    var gatherOptionsList = new List<Gather.InputEnum>
                    {
                        Gather.InputEnum.Dtmf
                    };
                    var gather = new Gather(
                        input: gatherOptionsList,
                        language: ConversationHelper.GatherLanguage,
                        numDigits: 1,
                        method: HttpMethod.Get,
                        timeout: 5,
                        action: Url.ActionUri("OnCardNumberConfirmed", "Payment")
                    );
                    var cardNumberSay = new Say(string.Format(ConversationHelper.CardNumberConfirm, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    cardNumberSay.SsmlProsody(rate: "slow");
                    gather.Append(cardNumberSay);
                    response.Append(gather);

                    // nothing received
                    var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    var uri = GetRedirectUri("Payment", "OnCardNumberEnetered", new Dictionary<string, string> { { "digits", digits } } ); //Url.ActionUri($"OnCardNumberEnetered?digits={digits}", "Payment")
                    response.Redirect(method: HttpMethod.Get, url: uri);
                }
                else
                {
                    // invalid card number
                    Log4NetLogger.Warn($"Invalid Card Number Entry {digits}, buy caller {Session["IncomingCallerNumber"] as string}");
                    var say = new Say(string.Format(ConversationHelper.CardNumberInvalid, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                }
                
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/OnCardNumberEnetered", e);
                throw;
            }
        }

        /// <summary>
        /// Card number confirmed
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult OnCardNumberConfirmed(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (digits == "*")
                {
                    Session["CardNumberConfirmed"] = Session["CardNumberEntered"];
                    Log4NetLogger.Info("Card Number Confirmed - " + Session["CardNumberConfirmed"]);
                  
                    return RedirectToAction("GetCardExpiryInfo", "Payment");
                }
                else
                {
                    Session["CardNumberConfirmed"] = null;
                    Session["CardNumberEntered"] = null;
                    // get card number again
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/OnCardNumberConfirmed", e);
                throw;
            }
        }

        /// <summary>
        /// Use default card?
        /// </summary>
        /// <returns></returns>
        public ActionResult UseDafaultCardOrNot()
        {
            try
            {
                var response = new VoiceResponse();
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                var gather = new Gather(
                    input: gatherOptionsList,
                    language: ConversationHelper.GatherLanguage,
                    numDigits: 1,
                    method: HttpMethod.Get,
                    timeout: 5,
                    action: Url.ActionUri("DafaultCardConfirmed", "Payment")
                );
                gather.Say(
                    string.Format(ConversationHelper.UseDafaultCardOrNot,
                        Utilities.AddSpaceBetweenChars(Session["UserDefaultCardEnding"] as string)),
                    ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("UseDafaultCardOrNot", "Payment"));
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/UseDafaultCardOrNot", e);
                throw;
            }
        }

        /// <summary>
        /// Use deafult card, and confrimation
        /// </summary>
        /// <returns></returns>
        public ActionResult DafaultCardConfirmed(string digits)
        {
            var response = new VoiceResponse();
            try
            {
                if (digits == "*")
                {
                    Session["UseDefaultCard"] = "yes";
                    Log4NetLogger.Info("Default card usage Confirmed - " + Session["UserDefaultCardEnding"]);

                    return RedirectToAction("GetCardExpiryInfo", "Payment");
                }
                else
                {
                    Session["UseDefaultCard"] = "no";
                    Log4NetLogger.Info("Default card will not be used - " + Session["UserDefaultCardEnding"]);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardNumber", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/DafaultCardConfirmed", e);
                throw;
            }
        }

        #endregion  card number

        #region card expiry

        /// <summary>
        /// Get card expiry info
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCardExpiryInfo()
        {
            try
            {
                var response = new VoiceResponse();
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                var gather = new Gather(
                    input: gatherOptionsList,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    numDigits: 5,
                    method: HttpMethod.Get,
                    finishOnKey: "*",
                    action: Url.ActionUri("CardExpiryDateEntered", "Payment")
                );
                gather.Say(ConversationHelper.GetCardExpDate, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardExpiryInfo", "Payment"));
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/GetCardExpiryInfo", e);
                throw;
            }
        }

        /// <summary>
        /// Confirm card expiry date
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult CardExpiryDateEntered(string digits)
        {
            try
            {
                var isValidExpiryDate = ApiHelper.ValidateCardExpiryDate(digits);
                if (isValidExpiryDate)
                {
                    Session["CardExpiryDateEntered"] = digits;
                    var response = new VoiceResponse();
                    var gatherOptionsList = new List<Gather.InputEnum>
                    {
                        Gather.InputEnum.Dtmf
                    };
                    var gather = new Gather(
                        input: gatherOptionsList,
                        language: ConversationHelper.GatherLanguage,
                        timeout: 5,
                        numDigits: 1,
                        method: HttpMethod.Get,
                        action: Url.ActionUri("CardExpiryDateConfirmed", "Payment")
                    );
                    gather.Say(string.Format(ConversationHelper.CardExpiryDateConfirm, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    response.Append(gather);

                    // nothing received
                    var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    var uri = GetRedirectUri("Payment", "CardExpiryDateEntered", new Dictionary<string, string> { { "digits", digits } } ); //Url.ActionUri($"CardExpiryDateEntered?digits={digits}", "Payment")
                    response.Redirect(method: HttpMethod.Get, url: uri);
                    return TwiML(response);
                }
                else
                {
                    // invalid card expiry date
                    Log4NetLogger.Warn($"Invalid Card Expiry Date {digits}, buy caller {Session["IncomingCallerNumber"] as string}");
                    var response = new VoiceResponse();
                    var say = new Say(string.Format(ConversationHelper.CardExpiryInvalid, digits), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardExpiryInfo", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/CardExpiryDateEntered", e);
                throw;
            }
        }


        /// <summary>
        /// card expiry date confirmed
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult CardExpiryDateConfirmed(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (digits == "*")
                {
                    Session["CardExpiryDateConfirmed"] = Session["CardExpiryDateEntered"];
                    Log4NetLogger.Info("Card expiry date Confirmed - " + Session["CardExpiryDateConfirmed"]);

                    return RedirectToAction("GetCvvNumber", "Payment");
                }
                else
                {
                    Session["CardExpiryDateEntered"] = null;
                    Session["CardExpiryDateConfirmed"] = null;
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCardExpiryInfo", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/CardExpiryDateConfirmed", e);
                throw;
            }
        }

        #endregion card expiry

        #region cvv number

        /// <summary>
        /// Get cvv number
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCvvNumber()
        {
            try
            {
                var response = new VoiceResponse();
                var gatherOptionsList = new List<Gather.InputEnum>
                {
                    Gather.InputEnum.Dtmf
                };
                var gather = new Gather(
                    input: gatherOptionsList,
                    language: ConversationHelper.GatherLanguage,
                    timeout: 5,
                    numDigits: 4,
                    method: HttpMethod.Get,
                    finishOnKey: "*",
                    action: Url.ActionUri("OnCvvNumberEntered", "Payment")
                );
                gather.Say(ConversationHelper.GetCvvNumber, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                response.Append(gather);

                // nothing received
                var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                response.Append(say);
                response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCvvNumber", "Payment"));
                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/GetCvvNumber", e);
                throw;
            }
        }

        public ActionResult OnCvvNumberEntered(string digits)
        {
            try
            {
                var isValidCvvNumber = ApiHelper.ValidateCvvNumber(digits);
                if (isValidCvvNumber)
                {
                    Session["CvvNumberEntered"] = digits;
                    var response = new VoiceResponse();
                    var gatherOptionsList = new List<Gather.InputEnum>
                    {
                        Gather.InputEnum.Dtmf
                    };
                    var gather = new Gather(
                        input: gatherOptionsList,
                        language: ConversationHelper.GatherLanguage,
                        timeout: 5,
                        numDigits: 1,
                        method: HttpMethod.Get,
                        action: Url.ActionUri("OnCvvNumberConfirmed", "Payment")
                    );
                    gather.Say(string.Format(ConversationHelper.CvvNumberConfirm, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    response.Append(gather);

                    // nothing received
                    var say = new Say(ConversationHelper.NothingReceived, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    var uri = GetRedirectUri("Payment", "OnCvvNumberEntered", new Dictionary<string, string> { { "digits", digits } });
                    response.Redirect(method: HttpMethod.Get, url: uri);
                    return TwiML(response);
                }
                else
                {
                    // invalid cvv number
                    Session["CvvNumberEntered"] = null;
                    Log4NetLogger.Warn($"Invalid CVV Number {digits}, buy caller {Session["IncomingCallerNumber"] as string}");
                    var response = new VoiceResponse();
                    var say = new Say(string.Format(ConversationHelper.CvvNumberInvalid, Utilities.AddSpaceBetweenChars(digits)), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCvvNumber", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/OnCvvNumberEntered", e);
                throw;
            }
        }

        public ActionResult OnCvvNumberConfirmed(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (digits == "*")
                {
                    Session["CvvNumberConfirmed"] = Session["CvvNumberEntered"];
                    Log4NetLogger.Info("CVV number Confirmed - " + Session["CvvNumberConfirmed"]);

                    var say = new Say(ConversationHelper.ProcessingPayment, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    return RedirectToAction("ProcessPayment", "Payment");
                }
                else
                {
                    Session["CvvNumberConfirmed"] = null;
                    Session["CvvNumberEntered"] = null;
                    response.Redirect(method: HttpMethod.Get, url: Url.ActionUri("GetCvvNumber", "Payment"));
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/OnCvvNumberConfirmed", e);
                throw;
            }
        }


        #endregion cvv number


        /// <summary>
        /// Process the payment
        /// </summary>
        /// <returns></returns>
        public ActionResult ProcessPayment()
        {
            try
            {
                var response = new VoiceResponse();
                string callSid = Session["CallSid"] as string;
                string incomingCallerNumber = Session["IncomingCallerNumber"] as string;
                string isRegisteredUser = Session["IsRegisteredUser"] as string;
                string requestedParkingLocation = Session["RequestedParkingLocation"] as string;
                string currentVrn = Session["CurrentVrn"] as string;
                int hoursConfirmed = (int) Session["HoursConfirmed"];
                double hourlyRate =(double)Session["HourlyRate"];
                double parkingCost = (double)Session["ParkingCost"];
                string useDefaultCard = Session["UseDefaultCard"] as string;
                int? userDefaultCardEnding = Session["UserDefaultCardEnding"] != null ? (int?) int.Parse((string)Session["UserDefaultCardEnding"]) : null;
                string cardNumberConfirmed = Session["CardNumberConfirmed"] as string;
                int cardExpiryDateConfirmed = int.Parse((string)Session["CardExpiryDateConfirmed"]);
                int cvvNumberConfirmed = int.Parse((string)Session["CvvNumberConfirmed"]);
                int? retryPaymentCount = Session["RetryPaymentCount"] != null ? (int?)int.Parse((string)Session["RetryPaymentCount"]) : null;

                var paymentSuccess = ApiHelper.ProcessPayment(callSid,
                                        incomingCallerNumber,
                                        isRegisteredUser,
                                        requestedParkingLocation,
                                        currentVrn,
                                        hoursConfirmed,
                                        hourlyRate,
                                        parkingCost,
                                        useDefaultCard,
                                        userDefaultCardEnding,
                                        cardNumberConfirmed,
                                        cardExpiryDateConfirmed,
                                        cvvNumberConfirmed,
                                        retryPaymentCount);
                LogSummary();       // log summary
                if (paymentSuccess)
                {
                    Log4NetLogger.Info("Payment Success");
                    var say = new Say(string.Format(ConversationHelper.PaymentComplete, Session["ParkUntil"] as string), ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                }
                else
                {
                    Log4NetLogger.Warn("Payment Unsuccessful");
                    Session["RetryPaymentCount"] = 0;
                    var gatherOptionsList = new List<Gather.InputEnum>
                    {
                        Gather.InputEnum.Dtmf
                    };
                    var gather = new Gather(
                        input: gatherOptionsList,
                        language: ConversationHelper.GatherLanguage,
                        timeout: 5,
                        method: HttpMethod.Get,
                        action: Url.ActionUri("RetryPayment", "Payment")
                    );
                    gather.Say(ConversationHelper.PaymentUnsuccessful, ConversationHelper.SpeakVoice, null, ConversationHelper.SpeakLanguage);
                    response.Append(gather);
                }

                return TwiML(response);
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/ProcessPayment", e);
                throw;
            }
        }

        /// <summary>
        /// Retry payment after payment fails
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public ActionResult RetryPayment(string digits)
        {
            try
            {
                var response = new VoiceResponse();
                if (digits == "*")
                {
                    if (Session["RetryPaymentCount"] != null && (int)Session["RetryPaymentCount"] != 0)
                    {
                        Session["RetryPaymentCount"] = (int)Session["RetryPaymentCount"] + 1;
                    }
                    else
                    {
                        Session["RetryPaymentCount"] = 0;
                    }

                    Log4NetLogger.Warn($"Retry Payment Count = {(int)Session["RetryPaymentCount"]} - By Caller = {Session["IncomingCallerNumber"] as string}");
                    return RedirectToAction("ProcessPayment", "Payment");
                }
                else
                {

                    var say = new Say(ConversationHelper.NotRetryPayment, ConversationHelper.SpeakVoice, 1, ConversationHelper.SpeakLanguage);
                    response.Append(say);
                    response.Hangup();
                    return TwiML(response);
                }
            }
            catch (Exception e)
            {
                Log4NetLogger.Error("Exception in PaymentController/RetryPayment", e);
                throw;
            }
        }
    }
}