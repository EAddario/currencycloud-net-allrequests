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

                var reverse = false;
                Account mainAccount = null;
                Account subAccount = null;
                Beneficiary beneficiary = null;
                Conversion conversion = null;
                Payment payment = null;
                Settlement settlement = null;

                Console.WriteLine(Environment.NewLine + "Beneficiary Required Details:");
                var beneficiaryDetails = await client.GetBeneficiaryRequiredDetailsAsync("MXN", "MX");
                Console.WriteLine(beneficiaryDetails.ToJSON());

                Console.WriteLine(Environment.NewLine + "Payer Required Details:");
                var payerDetails = await client.GetPayerRequiredDetailsAsync("MX");
                Console.WriteLine(payerDetails.ToJSON());

//                Console.WriteLine(Environment.NewLine + "Conversion Dates:");
//                var conversionDates = await client.GetConversionDatesAsync("GBPJPY");
//                Console.WriteLine(conversionDates.ToJSON());
//
//                Console.WriteLine(Environment.NewLine + "Available Currencies:");
//                var availableCurrencies = await client.GetAvailableCurrenciesAsync();
//                Console.WriteLine(availableCurrencies.ToJSON());
//
//                Console.WriteLine(Environment.NewLine + "Payment Dates:");
//                var paymentDates = await client.GetPaymentDatesAsync("JPY");
//                Console.WriteLine(paymentDates.ToJSON());
//
//                Console.WriteLine(Environment.NewLine + "Settlement Accounts:");
//                var settlementAccounts = await client.GetSettlementAccountsAsync();
//                Console.WriteLine(settlementAccounts.ToJSON());
//
//                Console.WriteLine(Environment.NewLine + "Purpose Codes:");
//                var purposeCodes = await client.GetPaymentPurposeCodes("INR", "IN");
//                Console.WriteLine(purposeCodes.ToJSON());
//
//                Console.WriteLine(Environment.NewLine + "Payer Required Details:");
//                var payerDetails = await client.GetPayerRequiredDetailsAsync("GB");
//                Console.WriteLine(payerDetails.ToJSON());
            }
            catch (ApiException e)
            {
                if(e is AuthenticationException)
                {
                    isAuthenticated = false;
                }

                Console.WriteLine("ApiException -> " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("System Exception");
                Console.WriteLine("Message: {0}", e.Message);
                Console.WriteLine("Source: {0}",e.Source);
                Console.WriteLine("Method: {0}",e.TargetSite);
                Console.WriteLine("Stack Trace: {0}",e.StackTrace);
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
    }
}