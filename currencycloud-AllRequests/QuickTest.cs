using System;
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

                Conversion conversion = null;

                #region Conversions API

                try
                {
                    /**
                     * Find Conversions
                     */
                    var findConversions = await client.FindConversionsAsync();
                    Console.WriteLine(Environment.NewLine + "Find Conversions: {0}", findConversions.ToJSON());
                    foreach (var element in findConversions.Conversions)
                        Console.WriteLine(Environment.NewLine + "Find Conversions Loop: {0}", element.ToJSON());

                    /**
                     * Retrieve Conversion
                     */
                    var retrieveConversion = await client.GetConversionAsync(findConversions.Conversions[0].Id);
                    Console.WriteLine(Environment.NewLine + "Retrieve Conversion: {0}", retrieveConversion.ToJSON());

                    /**
                     * Create Conversion
                     */
                    var createConversion = await client.CreateConversionAsync(new Conversion
                    (
                        "EUR",
                        "GBP",
                        "buy",
                        new Random().Next(1000000, 1500000) / 100,
                        true
                     ));
                    Console.WriteLine(Environment.NewLine + "Create Conversion: {0}", createConversion.ToJSON());

                    var daysToMonday = (int)DayOfWeek.Monday <= (int)DateTime.Now.DayOfWeek ?
                        (int)DayOfWeek.Monday - (int)DateTime.Now.DayOfWeek + 7 :
                        (int)DayOfWeek.Monday - (int)DateTime.Now.DayOfWeek;

                    /**
                     * Quote Date Change
                     */
                    var quoteDateChange = await client.QuoteDateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 2)
                    });
                    Console.WriteLine(Environment.NewLine + "Quote Conversion Date Change: {0}", quoteDateChange.ToJSON());

                    /**
                     * Date Change - Next Monday + 1
                     */
                    var dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 2 )
                    });
                    Console.WriteLine(Environment.NewLine + "First Conversion Date Change: {0}", dateChange.ToJSON());

                    /**
                     * Date Change - Next Monday + 2
                     */
                    dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 3)
                    });
                    Console.WriteLine(Environment.NewLine + "Second Conversion Date Change: {0}", dateChange.ToJSON());

                    /**
                     * Date Change - Next Monday + 3
                     */
                    dateChange = await client.DateChangeConversionAsync(new ConversionDateChange
                    {
                        ConversionId = createConversion.Id,
                        NewSettlementDate = DateTime.Now.AddDays(daysToMonday + 4)
                    });
                    Console.WriteLine(Environment.NewLine + "Third Conversion Date Change: {0}", dateChange.ToJSON());

                    /**
                     * Date Change Detail
                     */
                    var dateChangeDetail = await client.DateChangeDetailsConversionAsync(createConversion);
                    Console.WriteLine(Environment.NewLine + "Date Change Details: {0}", dateChangeDetail.ToJSON());

                    /**
                     * Split Preview
                     */
                    var splitPreview = await client.PreviewSplitConversionAsync(new Conversion
                    {
                        Id = createConversion.Id,
                        Amount = new Random().Next(500000, 999999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Preview Conversion Split: {0}", splitPreview.ToJSON());

                    /**
                     * Split Conversion
                     */
                    var splitConversion = await client.SplitConversionAsync(new Conversion
                    {
                        Id = createConversion.Id,
                        Amount = new Random().Next(750000, 999999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Conversion Split: {0}", splitConversion.ToJSON());

                    var parentConversion = splitConversion.ParentConversion;
                    var childConversion = splitConversion.ChildConversion;

                    /**
                     * Split Child Conversion
                     */
                    var splitChildConversion = await client.SplitConversionAsync(new Conversion
                    {
                        Id = childConversion.Id,
                        Amount = new Random().Next(500000, 749999) / 100
                    });
                    Console.WriteLine(Environment.NewLine + "Child Conversion Split: {0}", splitChildConversion.ToJSON());

                    /**
                     * Split History - Conversion
                     */
                    var splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = parentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Original Conversion Split History: {0}", splitHistory.ToJSON());

                    /**
                     * Split History - Parent
                     */
                    splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = childConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Parent Conversion Split History: {0}", splitHistory.ToJSON());

                    /**
                     * Split History - Child/Child
                     */
                    splitHistory = await client.SplitHistoryConversionAsync(new Conversion
                    {
                        Id = splitChildConversion.ChildConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Child/Child Conversion Split History: {0}", splitHistory.ToJSON());

                    /**
                     * General Profit and Loss
                     */
                    var profitAndLoss = await client.FindConversionProfitAndLossesAsync();
                    Console.WriteLine(Environment.NewLine + "Total Profit and Losses: {0}", profitAndLoss.ToJSON());

                    /**
                     * Conversion Profit and Loss
                     */
                    profitAndLoss = await client.FindConversionProfitAndLossesAsync(new ConversionProfitAndLossFindParameters
                    {
                        ConversionId = parentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Conversion Profit and Losses: {0}", profitAndLoss.ToJSON());

                    /**
                     * Quote Cancel Conversion
                     */
                    var quoteCancel = await client.QuoteCancelConversionAsync(new ConversionCancellation
                    {
                        ConversionId = createConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Quote Conversion Cancellation: {0}", quoteCancel.ToJSON());

                    /**
                     * Cancel Child/Child Conversion
                     */
                    var cancelConversion = await client.CancelConversionsAsync(new ConversionCancellation
                    {
                        ConversionId = splitChildConversion.ChildConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Cancel Split Child Conversion: {0}", cancelConversion.ToJSON());

                    /**
                     * Cancel Child/Parent Conversion
                     */
                    cancelConversion = await client.CancelConversionsAsync(new ConversionCancellation
                    {
                        ConversionId = splitChildConversion.ParentConversion.Id
                    });
                    Console.WriteLine(Environment.NewLine + "Cancel Split Parent Conversion: {0}", cancelConversion.ToJSON());

                    /**
                     * Cancel Parent Conversion
                     */
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