
using ServiceBase.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBase.Notification.Twilio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new TwilioOptions
            {
                Sid = "ACe688195668d5f7ca2f2283d274a384c5",
                Token = "4014740a51d129040ee29776afc7d8eb",
                From = "+491735199341"
            };

            var sender = new TwilioSmsSender(options,
                new NullLogger<TwilioSmsSender>());

            sender.SendSmsAsync("+491735199341", "Hallo Welt!");

            Console.ReadKey(); 
        }
    }
}
