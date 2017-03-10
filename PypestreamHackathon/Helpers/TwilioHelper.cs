using System;
using System.Threading.Tasks;
using Twilio;

namespace PypestreamHackathon.Helpers
{
    public class TwilioHelper
    {
        public static void sendSMS(string messageTo, string messageText)
        {
            string ALLOWED_PHONENUMBER = "+19176339276";
            // Your Account SID from twilio.com/console
            var accountSid = "ACa195db395adcbff6b7043fdfc2a61946";
            // Your Auth Token from twilio.com/console
            var authToken = "207e77a8fdb04f93fe2b9c8673bb6e5e";

            var client = new TwilioRestClient(accountSid, authToken);
            client.SendMessage(ALLOWED_PHONENUMBER, messageTo, messageText);
        }
    }
}