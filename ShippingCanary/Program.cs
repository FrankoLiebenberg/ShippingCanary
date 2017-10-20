using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ShippingCanary
{
    class Program
    {
        private static string _previousVersion;
        private static int _interval = 5;

        static void Main(string[] args)
        {
            bool valid = false;
            string url = "";


            Console.ForegroundColor = ConsoleColor.Green;

            while (!valid)
            {
                Console.WriteLine("Enter the URL");
                url = Console.ReadLine();
                Console.Clear();

                valid = Uri.IsWellFormedUriString(url, UriKind.Absolute);

            }


            int timeOut = (int)TimeSpan.FromMinutes(_interval).TotalMilliseconds;

            ManualResetEvent duration = new ManualResetEvent(false);

            Task.Run(() =>
            {
                Thread.Sleep((int)TimeSpan.FromHours(24).TotalMilliseconds);
                duration.Set();
            });

            Check(url);

            Console.WriteLine("Running checks every " + _interval + " minutes");

            while (!duration.WaitOne(timeOut))
            {

                bool changed = Check(url);

                if (changed)
                {
                    Console.WriteLine(DateTime.Now + " - Change Detected");
                }

            }


        }

        static bool Check(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        return Different(content.ReadAsStringAsync().Result);

                    }
                }
            }
        }

        static bool Different(string input)
        {
            if (_previousVersion != input)
            {
                _previousVersion = input;

                return true;

            }

            return false;
        }

    }
}
