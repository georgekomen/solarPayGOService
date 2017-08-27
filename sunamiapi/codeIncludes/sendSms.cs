using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sunamiapi.codeIncludes
{
    public class sendSms
    {
        public void sendSmsThroughGateway(string SmsNumber, string Message)
        {
            string username = "georgekomen";
            string apiKey = "e36225a6e5630d73cfd37d66cdbf042a171161d5415d811e43ed96321f9cb556";
            AfricasTalkingGateway gateway = new AfricasTalkingGateway(username, apiKey);
            try
            {
                dynamic results = gateway.sendMessage(SmsNumber, Message);
            }
            catch (AfricasTalkingGatewayException e)
            {
                //Console.WriteLine("Encountered an error: " + e.Message);
            }

        }
    }
}