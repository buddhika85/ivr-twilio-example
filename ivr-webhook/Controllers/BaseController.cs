using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ivr_webhook.Helpers;
using Twilio.AspNet.Mvc;

namespace ivr_webhook.Controllers
{
    public class BaseController : TwilioController
    {
        public void LogMessage(string message)
        {
            ErrorLog.LogError(new Exception(), message);
        }
        
    }
}