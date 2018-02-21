using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class fetchSMS
    {
        public dynamic fetchSms(int messageId)
        {
            // Specify your login credentials
            string username = "georgekomen";
            string apiKey = "e36225a6e5630d73cfd37d66cdbf042a171161d5415d811e43ed96321f9cb556";
            
            // Create a new instance of our awesome gateway class
            AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);
            // Any gateway errors will be captured by our custom Exception class below,
            // so wrap the call in a try-catch block   
            try
            {
                // Thats it, hit send and we'll take care of the rest

                dynamic results = gateway.fetchMessages(messageId);
                return results;
            }
            catch (AfricasTalkingGatewayException e)
            {
                return "Encountered an error: " + e.Message;
            }

        }
    }
}