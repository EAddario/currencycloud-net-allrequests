using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyCloud;
using CurrencyCloud.Entity;
using CurrencyCloud.Exception;
using CurrencyCloud.Environment;

namespace AllRequests
{
    public static class QuickTest
    {
        public static async Task MainAsync(string loginId, string apiKey)
        {
            var client = new Client();
            var isAuthenticated = false;
            Retry.Enabled = false;

            try
            {
                Console.WriteLine("Login ID: {0} | API Key {1}", loginId, apiKey);
                var token = await client.InitializeAsync(ApiServer.Demo, loginId, apiKey);
                Console.WriteLine("Token: {0}", token);
                isAuthenticated = true;

                /* ToDo: Add API calls here */
            }
            catch (ApiException e)
            {
                if (e is AuthenticationException)
                {
                    isAuthenticated = false;
                }

                Console.WriteLine("ApiException -> " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("System Exception");
                Console.WriteLine("Message: {0}", e.Message);
                Console.WriteLine("Source: {0}", e.Source);
                Console.WriteLine("Method: {0}", e.TargetSite);
                Console.WriteLine("Stack Trace: {0}", e.StackTrace);
            }
            finally
            {
                if (isAuthenticated)
                {
                    Console.WriteLine(Environment.NewLine + "Close Session");
                    await client.CloseAsync();
                }
            }
        }

        static string RandomChars(int num)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[num];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
