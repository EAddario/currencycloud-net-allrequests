using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyCloud;
using CurrencyCloud.Entity;
using CurrencyCloud.Exception;
using CurrencyCloud.Environment;

namespace AllRequests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //MainAsync(args[0], args[1]).Wait();
            QuickTest.MainAsync(args[0], args[1]).Wait();
        }

        static async Task MainAsync(string loginId, string apiKey)
        {
            var client = new Client();
            var isAuthenticated = false;

            try
            {
                Console.WriteLine("Login ID: {0} | API Key {1}", loginId, apiKey);
                var token = await client.InitializeAsync(ApiServer.Demo, loginId, apiKey);
                Console.WriteLine("Token: {0}", token);
                isAuthenticated = true;

                var reverse = true;
                Account mainAccount = null;
                Account subAccount = null;
                Beneficiary beneficiary = null;
                Conversion conversion = null;
                Payment payment = null;
                Settlement settlement = null;

                #region Accounts API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Current Account:");
                    var currentAccount = await client.GetCurrentAccountAsync();
                    Console.WriteLine(currentAccount.ToJSON());

                    mainAccount = currentAccount;

                    Console.WriteLine(Environment.NewLine + "Find Accounts:");
                    var findAccounts = await client.FindAccountsAsync();
                    Console.WriteLine(findAccounts.ToJSON());
                    Console.WriteLine(Environment.NewLine + "Find Accounts Loop:");
                    foreach (var element in findAccounts.Accounts)
                        Console.WriteLine(element.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrieve Account:");
                    var retrieveAccount = await client.GetAccountAsync(mainAccount.Id);
                    Console.WriteLine(retrieveAccount.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Create Account:");
                    var createAccount = await client.CreateAccountAsync(new Account
                    {
                        AccountName = "Currencycloud Development",
                        LegalEntityType = "individual",
                        Street = "12 Steward St",
                        City = "London",
                        PostalCode = "E1 6FQ",
                        Country = "GB",
                        ApiTrading = true,
                        OnlineTrading = true,
                        PhoneTrading = true
                    });
                    Console.WriteLine(createAccount.ToJSON());

                    subAccount = createAccount;

                    Console.WriteLine(Environment.NewLine + "Update Account:");
                    var updateAccount = await client.UpdateAccountAsync(new Account
                    {
                        Id = subAccount.Id,
                        YourReference = "CCY-" + new Random().Next(1000, 10000) + "-" + RandomChars(4),
                        IdentificationType = "passport",
                        IdentificationValue = RandomChars(10)
                    });
                    Console.WriteLine(updateAccount.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Balances API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Balances:");
                    var findBalances = await client.FindBalancesAsync();
                    Console.WriteLine(findBalances.ToJSON());

                    if (findBalances.Balances[0] != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Retrieve Balance:");
                        var retrieveBalance = await client.GetBalanceAsync(findBalances.Balances[0].Currency);
                        Console.WriteLine(retrieveBalance.ToJSON());
                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Beneficiary API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Beneficiaries:");
                    var findBeneficiaries = await client.FindBeneficiariesAsync();
                    Console.WriteLine(findBeneficiaries.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrieve Beneficiary:");
                    var retrieveBeneficiary = await client.GetBeneficiaryAsync(findBeneficiaries.Beneficiaries[0].Id);
                    Console.WriteLine(retrieveBeneficiary.ToJSON());

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

                    beneficiary = createBeneficiary;

                    beneficiary = new Beneficiary
                    {
                        Id = createBeneficiary.Id,
                        BeneficiaryFirstName = "Tamara",
                        BeneficiaryLastName = "Carlton",
                        Email = "development@currencycloud.com",
                    };

                    Console.WriteLine(Environment.NewLine + "Update Beneficiary:");
                    var updateBeneficiary = await client.UpdateBeneficiaryAsync(beneficiary);
                    Console.WriteLine(updateBeneficiary.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Contacts API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Contacts:");
                    var findContacts = await client.FindContactsAsync();
                    Console.WriteLine(findContacts.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Current Contact:");
                    var currentContact = await client.GetCurrentContactAsync();
                    Console.WriteLine(currentContact.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Get Contact:");
                    var getContact = await client.GetContactAsync(currentContact.Id);
                    Console.WriteLine(getContact.ToJSON());

                    if (subAccount != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Create Contact:");
                        var createContact = await client.CreateContactAsync(new Contact
                        {
                            AccountId = subAccount.Id,
                            FirstName = "Currencycloud",
                            LastName = "Development",
                            EmailAddress = "development." + RandomChars(6) + "@currencycloud.com",
                            PhoneNumber = "+44 20 3326 8173",
                            DateOfBirth = new DateTime(1968, 03, 23)
                        });
                        Console.WriteLine(createContact.ToJSON());

                        Console.WriteLine(Environment.NewLine + "Update Contact:");
                        var updateContact = await client.UpdateContactAsync(new Contact
                        {
                            Id = createContact.Id,
                            YourReference = "CCY-CTC-" + new Random().Next(100, 1000),
                            Status = "enabled",
                            Locale = "en-GB",
                            Timezone = "Europe/London",
                            DateOfBirth = new DateTime(1968, 03, 23)
                        });
                        Console.WriteLine(updateContact.ToJSON());
                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Conversions API

                try
                {
                    var findConversions = await client.FindConversionsAsync();
                    Console.WriteLine(Environment.NewLine + "Find Conversions: {0}", findConversions.ToJSON());

                    var retrieveConversion = await client.GetConversionAsync(findConversions.Conversions[0].Id);
                    Console.WriteLine(Environment.NewLine + "Retrieve Conversion: {0}", retrieveConversion.ToJSON());

                    var createConversion = await client.CreateConversionAsync(new Conversion
                    (
                        "EUR",
                        "GBP",
                        "buy",
                        (decimal) new Random().Next(1000000, 1500000) / 100,
                        true
                    ));
                    Console.WriteLine(Environment.NewLine + "Create Conversion: {0}", createConversion.ToJSON());

                    var daysToMonday = (int) DayOfWeek.Monday <= (int) DateTime.Now.DayOfWeek
                        ? (int) DayOfWeek.Monday - (int) DateTime.Now.DayOfWeek + 7
                        : (int) DayOfWeek.Monday - (int) DateTime.Now.DayOfWeek;

                    var quoteDateChange = await client.QuoteDateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 2)
                    });
                    Console.WriteLine(Environment.NewLine + "Quote Conversion Date Change: {0}", quoteDateChange.ToJSON());

                    var dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 2)
                    });
                    Console.WriteLine(Environment.NewLine + "First Conversion Date Change: {0}", dateChange.ToJSON());

                    dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 3)
                    });
                    Console.WriteLine(Environment.NewLine + "Second Conversion Date Change: {0}", dateChange.ToJSON());

                    dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 4)
                    });
                    Console.WriteLine(Environment.NewLine + "Third Conversion Date Change: {0}", dateChange.ToJSON());

                    /* ToDo: Deprecate? */
//                    var dateChangeDetail = await client.DateChangeDetailsConversionAsync(createConversion);
//                    Console.WriteLine(Environment.NewLine + "Date Change Details: {0}", dateChangeDetail.ToJSON());

                    var splitPreview = await client.PreviewSplitConversionAsync(new Conversion
                    {
                        Id = createConversion.Id,
                        Amount = new Random().Next(500000, 999999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Preview Conversion Split: {0}", splitPreview.ToJSON());

                    var splitConversion = await client.SplitConversionAsync(new Conversion
                    {
                        Id = createConversion.Id,
                        Amount = new Random().Next(750000, 999999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Conversion Split: {0}", splitConversion.ToJSON());

                    var parentConversion = splitConversion.ParentConversion;
                    var childConversion = splitConversion.ChildConversion;

                    var splitChildConversion = await client.SplitConversionAsync(new Conversion
                    {
                        Id = childConversion.Id,
                        Amount = new Random().Next(500000, 749999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Child Conversion Split: {0}", splitChildConversion.ToJSON());

                    var splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = parentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Original Conversion Split History: {0}", splitHistory.ToJSON());

                    splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = childConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Parent Conversion Split History: {0}", splitHistory.ToJSON());

                    splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = splitChildConversion.ChildConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Child/Child Conversion Split History: {0}", splitHistory.ToJSON());

                    var profitAndLoss = await client.FindConversionProfitAndLossesAsync();
                    Console.WriteLine(Environment.NewLine + "Total Profit and Losses: {0}", profitAndLoss.ToJSON());

                    profitAndLoss = await client.FindConversionProfitAndLossesAsync(new ConversionProfitAndLossFindParameters
                    {
                        ConversionId = parentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Conversion Profit and Losses: {0}", profitAndLoss.ToJSON());

                    var quoteCancel = await client.QuoteCancelConversionAsync(new ConversionCancellation
                    {
                        ConversionId = createConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Quote Conversion Cancellation: {0}", quoteCancel.ToJSON());

                    var cancelConversion = await client.CancelConversionsAsync(new ConversionCancellation
                    {
                        ConversionId = splitChildConversion.ChildConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Cancel Split Child Conversion: {0}", cancelConversion.ToJSON());

                    cancelConversion = await client.CancelConversionsAsync(new ConversionCancellation
                    {
                        ConversionId = splitChildConversion.ParentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Cancel Split Parent Conversion: {0}", cancelConversion.ToJSON());

                    cancelConversion = await client.CancelConversionsAsync(new ConversionCancellation
                    {
                        ConversionId = parentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Cancel Parent Conversion: {0}", cancelConversion.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Ibans API

                try
                {
                    /* ToDo: Deprecate? */
                    Console.WriteLine(Environment.NewLine + "Find Ibans:");
                    var findIbans = await client.FindIbansAsync();
                    Console.WriteLine(findIbans.ToJSON());

                    /* ToDo: Deprecate? */
                    Console.WriteLine(Environment.NewLine + "Find Sub-Account Ibans:");
                    var findSubAccountIbans = await client.FindSubAccountsIbansAsync(new IbanFindParameters());
                    Console.WriteLine(findSubAccountIbans.ToJSON());

                    /* ToDo: Deprecate? */
                    Console.WriteLine(Environment.NewLine + "Retrieve Sub-Account Ibans:");
                    var retrieveSubAccountIbans = await client.GetSubAccountsIbansAsync(subAccount.Id);
                    Console.WriteLine(retrieveSubAccountIbans.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Payments API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Payments:");
                    var findPayments = await client.FindPaymentsAsync();
                    Console.WriteLine(findPayments.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrieve Payment:");
                    var retrievePayment = await client.GetPaymentAsync(findPayments.Payments[0].Id);
                    Console.WriteLine(retrievePayment.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrieve Payment Submission:");
                    var retrievePaymentSubmission = await client.GetPaymentSubmissionAsync(retrievePayment.Id);
                    Console.WriteLine(retrievePaymentSubmission.ToJSON());

                    if (beneficiary != null && conversion != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Create Payments:");
                        payment = await client.CreatePaymentAsync(new Payment
                        {
                            BeneficiaryId = beneficiary.Id,
                            Currency = conversion.BuyCurrency,
                            ConversionId = conversion.Id,
                            Amount = conversion.ClientBuyAmount,
                            Reason = "Investment",
                            Reference = "CCY-PMT-" + new Random().Next(100, 1000),
                            PaymentType = "regular",
                            UltimateBeneficiaryName = beneficiary.BankAccountHolderName,
                            UniqueRequestId = Guid.NewGuid().ToString()
                        });
                        Console.WriteLine(payment.ToJSON());

                        Console.WriteLine(Environment.NewLine + "Update Payment:");
                        var updatePayment = await client.UpdatePaymentAsync(new Payment
                        {
                            Id = payment.Id,
                            Reference = "CCY-PMT-" + new Random().Next(100, 1000)
                        });
                        Console.WriteLine(updatePayment.ToJSON());
                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e);
                }

                #endregion

                #region Rates API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Rates:");
                    var findRates = await client.FindRatesAsync("GBPUSD,GBPCAD,GBPEUR,GBPJPY,FOOBAR", true);
                    Console.WriteLine(findRates.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Detailed Rates:");
                    var detailedRates = await client.GetRateAsync(new DetailedRates
                    {
                        BuyCurrency = "USD",
                        SellCurrency = "GBP",
                        FixedSide = "buy",
                        Amount = (decimal) new Random().Next(100000, 1000000) / 100
                    });
                    Console.WriteLine(detailedRates.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Reference API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Beneficiary Required Details:");
                    var beneficiaryDetails = await client.GetBeneficiaryRequiredDetailsAsync();
                    Console.WriteLine(beneficiaryDetails.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Conversion Dates:");
                    var conversionDates = await client.GetConversionDatesAsync("GBPJPY");
                    Console.WriteLine(conversionDates.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Available Currencies:");
                    var availableCurrencies = await client.GetAvailableCurrenciesAsync();
                    Console.WriteLine(availableCurrencies.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Payment Dates:");
                    var paymentDates = await client.GetPaymentDatesAsync("JPY");
                    Console.WriteLine(paymentDates.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Settlement Accounts:");
                    var settlementAccounts = await client.GetSettlementAccountsAsync();
                    Console.WriteLine(settlementAccounts.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Purpose Codes:");
                    var purposeCodes = await client.GetPaymentPurposeCodes("INR", "IN");
                    Console.WriteLine(purposeCodes.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Payer Required Details:");
                    var payerDetails = await client.GetPayerRequiredDetailsAsync("GB");
                    Console.WriteLine(payerDetails.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Settlements API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Settlements:");
                    var findSettlements = await client.FindSettlementsAsync();
                    Console.WriteLine(findSettlements.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Create Settlement:");
                    settlement = await client.CreateSettlementAsync();
                    Console.WriteLine(settlement.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrive Settlement:");
                    var retrieveSettlement = await client.GetSettlementAsync(settlement.Id);
                    Console.WriteLine(retrieveSettlement.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Add Conversion to Settlement:");
                    var addConversionSettlement = await client.AddConversionToSettlementAsync(settlement.Id, conversion.Id);
                    Console.WriteLine(addConversionSettlement.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Release Settlement:");
                    var releaseSettlement = await client.ReleaseSettlementAsync(settlement.Id);
                    Console.WriteLine(releaseSettlement.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Unrelease Settlement:");
                    var unreleaseSettlement = await client.UnreleaseSettlementAsync(settlement.Id);
                    Console.WriteLine(unreleaseSettlement.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Remove Conversion from Settlement:");
                    var removeConversionSettlement = await client.RemoveConversionFromSettlementAsync(settlement.Id, conversion.Id);
                    Console.WriteLine(removeConversionSettlement.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Transactions API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Transactions:");
                    var findTransactions = await client.FindTransactionsAsync();
                    Console.WriteLine(findTransactions.ToJSON());

                    Console.WriteLine(Environment.NewLine + "Retrieve Transaction:");
                    var retrieveTransaction = await client.GetTransactionAsync(findTransactions.Transactions[0].Id);
                    Console.WriteLine(retrieveTransaction.ToJSON());
                }
                catch (ApiException e)
                {
                    Console.WriteLine(e.Message);
                }

                #endregion

                #region Transfers API

                try
                {
                    Console.WriteLine(Environment.NewLine + "Find Transfers:");
                    var findTransfers = await client.FindTransfersAsync();
                    Console.WriteLine(findTransfers.ToJSON());

                    if (findTransfers != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Retrieve Transfer:");
                        var retrieveTransfer = await client.GetTransferAsync(findTransfers.Transfers[0].Id);
                        Console.WriteLine(retrieveTransfer.ToJSON());
                    }

                    if (mainAccount != null && subAccount != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Create Transfer:");
                        var createTransfer = await client.CreateTransferAsync(new Transfer
                        {
                            SourceAccountId = subAccount.Id,
                            DestinationAccountId = mainAccount.Id,
                            Currency = "GBP",
                            Amount = (decimal) new Random().Next(10000, 100000) / 100,
                            Reason = "Funding"
                        });
                        Console.WriteLine(createTransfer.ToJSON());
                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine(e.Message);
                }

                #endregion

                #region Virtual Accounts API

                try
                {
                    /* ToDo: Deprecate? */
//                    Console.WriteLine(Environment.NewLine + "Find Virtual Accounts:");
//                    var findVANs = await client.FindVirtualAccountsAsync(new FindParameters());
//                    Console.WriteLine(findVANs.ToJSON());

                    /* ToDo: Deprecate? */
//                    Console.WriteLine(Environment.NewLine + "Find Sub-Account Virtual Accounts:");
//                    var findSubAccountVANs = await client.FindSubAccountsVirtualAccountsAsync(new FindParameters());
//                    Console.WriteLine(findSubAccountVANs.ToJSON());

//                    if (findSubAccountVANs != null)
//                    {
//                        /* ToDo: Deprecate? */
//                        Console.WriteLine(Environment.NewLine + "Retrieve Sub-Account Virtual Accounts:");
//                        var retrieveSubAccountVANs = await client.GetSubAccountVirtualAccountsAsync(findSubAccountVANs.VirtualAccounts[0].Id);
//                        Console.WriteLine(retrieveSubAccountVANs.ToJSON());
//                    }
                }
                catch (ApiException e)
                {
                    Console.WriteLine("ApiException -> " + e.Message);
                }

                #endregion

                #region Delete Objects

                if (reverse)
                {
                    if (settlement != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Delete Settlement:");
                        var deleteSettlement = await client.DeleteSettlementAsync(settlement.Id);
                        Console.WriteLine(deleteSettlement.ToJSON());
                    }

                    if (payment != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Delete Payment:");
                        var deletePayment = await client.DeletePaymentAsync(payment.Id);
                        Console.WriteLine(deletePayment.ToJSON());
                    }

                    if (beneficiary != null)
                    {
                        Console.WriteLine(Environment.NewLine + "Delete Beneficiary:");
                        var deleteBeneficiary = await client.DeleteBeneficiaryAsync(beneficiary.Id);
                        Console.WriteLine(deleteBeneficiary.ToJSON());
                    }
                }

                #endregion
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