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

                beneficiary = new Beneficiary
                {
                    BankCountry = "IT",
                    Currency = "EUR",
                    AccountNumber = "1234567890",
                    Iban = "IT1200012030200359100100",
                    BicSwift = "IBSPITNA020",
                    PaymentTypes = new [] {"regular", "priority"},
                    BeneficiaryEntityType = "individual",
                    BeneficiaryAddress = new List<string> {"Via dei Tribunali, 38, 80138"},
                    BeneficiaryCity = "Napoli",
                    BeneficiaryCountry = "IT",
                    BeneficiaryFirstName = "Dame Tamara",
                    BeneficiaryLastName = "Carlton"
                };

                Console.WriteLine(Environment.NewLine + "Validate Beneficiary:");
                var validateBeneficiary = await client.ValidateBeneficiaryAsync(beneficiary);
                Console.WriteLine(validateBeneficiary.ToJSON());

                beneficiary.BankAccountHolderName = "Dame Tamara Carlton";
                beneficiary.Name = "Fulcrum Fund";

                Console.WriteLine(Environment.NewLine + "Create Beneficiary:");
                var createBeneficiary = await client.CreateBeneficiaryAsync(beneficiary);
                Console.WriteLine(createBeneficiary.ToJSON());

                Console.WriteLine(Environment.NewLine + "Delete Beneficiary:");
                var deleteBeneficiary = await client.DeleteBeneficiaryAsync(createBeneficiary.Id);
                Console.WriteLine(deleteBeneficiary.ToJSON());
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