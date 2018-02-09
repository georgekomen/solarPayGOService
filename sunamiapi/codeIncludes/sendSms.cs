using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class sendSms
    {
        public void sendSmsThroughGateway(string SmsNumber, string Message, string customer_id)
        {
            // Specify your login credentials
            string username = "georgekomen";
            string apiKey = "e36225a6e5630d73cfd37d66cdbf042a171161d5415d811e43ed96321f9cb556";

            // Specify the numbers that you want to send to in a comma-separated list
            // Please ensure you include the country code (+254 for Kenya in this case)
            string recipients = SmsNumber;
            // And of course we want our recipients to know what we really do
            string message = Message;
            string from = "SUNAMISOLAR";
            // Create a new instance of our awesome gateway class
            AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);
            // Any gateway errors will be captured by our custom Exception class below,
            // so wrap the call in a try-catch block   
            try
            {
                // Thats it, hit send and we'll take care of the rest

                dynamic results = gateway.sendMessage(recipients, message, customer_id, from);

                foreach (dynamic result in results)
                {
                    Console.Write((string)result["number"] + ",");
                    Console.Write((string)result["status"] + ","); // status is either "Success" or "error message"
                    Console.Write((string)result["messageId"] + ",");
                    Console.WriteLine((string)result["cost"]);
                }
            }
            catch (AfricasTalkingGatewayException e)
            {
                Console.WriteLine("Encountered an error: " + e.Message);
            }

        }
    }
}