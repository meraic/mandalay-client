using MandalayClient.Common.Models.Json.Transactions;
using MandalayJdeIntegrationCore.Models.Jde;
using MandalayJdeIntegrationCore.Models.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MandalayJdeIntegrationCore.Helper
{
    // This whole pirce needs to be replaced at some point as at the moment I am using the POC code written by David 
    public static class MandalaysToJdeCsvHelper
    {
        public static JdeCsvAndErrors MapToCsv(this IEnumerable<Transaction> mandalayTransactions,
            string site, string tenant, DateTime tenantLocalTime,
            IDictionary<string, Site> siteMappings,
            IDictionary<string, Tenant> tenantMappings,
            IDictionary<string, UnitOfMeasure> uomMappings)
        {
            var result = new JdeCsvAndErrors();

            var uniqueTransactionLogIds = mandalayTransactions.Select(x => new { SiteId = x.SiteId, TransactionLogId = x.TransactionLogId }).Distinct();

            foreach (var unique in uniqueTransactionLogIds)
            {
                if (!unique.TransactionLogId.HasValue)
                {
                    throw new Exception("Recieved a transaction without a value for transaction_log_id");
                }
                if (string.IsNullOrEmpty(unique.SiteId))
                {
                    throw new Exception("Recieved a transaction without a value for site_id");
                }

                // get the transactions for that uniq site and log_id
                var groupOfTransactions = mandalayTransactions.Where(x => x.TransactionLogId == unique.TransactionLogId && x.SiteId == unique.SiteId).ToArray();
                Site currentSite = null;
                if (currentSite == null || currentSite.MandalaySiteId != unique.SiteId)
                {
                    currentSite = siteMappings.FirstOrDefault(x => x.Value?.MandalaySiteId == unique.SiteId).Value;
                    if (currentSite == null)
                    {
                        result.Errors.Add($"A transaction with Site \"{unique.SiteId}\" was recieved from Mandalay but there was no entry for this site in the Site table.");
                        continue;
                    }
                }
                // Map the transaction
                var jdeCsvLines = MapGroupOfTransactions(groupOfTransactions, currentSite, uomMappings, tenantLocalTime, result.Errors);
                // and append it
                result.CsvLines.AddRange(jdeCsvLines);
            }

            return result;
        }

        private static List<string> MapGroupOfTransactions(Transaction[] groupOfTransactions, Site currentSite, IDictionary<string, UnitOfMeasure> uomMappings, DateTime tenantLocalDateTime, List<string> errors)
        {
            var translatedLines = new List<string>();
            var transactionsWithCharges = groupOfTransactions.Where(x => x.TotalExcludingTax.HasValue && x.TotalExcludingTax.Value != 0).ToList();
            if (transactionsWithCharges.Count == 0)
            {
                return translatedLines;  // no transactions with $amt so nothing to generate.
            }

            translatedLines.Add(buildOrderHeader(groupOfTransactions[0], currentSite, uomMappings, tenantLocalDateTime, errors));

            int detailLineNumber = 1;
            foreach (var transaction in transactionsWithCharges)
            {
                if (currentSite.JdeSplitEpaLevy && transaction.EpaLevy.HasValue && transaction.EpaLevy != 0)
                {
                    string detailLine1 = buildOrderDetail(transaction, currentSite, uomMappings, tenantLocalDateTime, errors, detailLineNumber++, OrderDetailType.EpaSplitRecord1);
                    translatedLines.Add(detailLine1);
                    string detailLine2 = buildOrderDetail(transaction, currentSite, uomMappings, tenantLocalDateTime, errors, detailLineNumber++, OrderDetailType.EpaSplitRecord2);
                    translatedLines.Add(detailLine2);
                }
                else
                {
                    string detailLine = buildOrderDetail(transaction, currentSite, uomMappings, tenantLocalDateTime, errors, detailLineNumber++, OrderDetailType.NotEpaSplit);
                    translatedLines.Add(detailLine);
                }
            }
            return translatedLines;
        }

        private static string buildOrderHeader(Transaction transaction, Site site, IDictionary<string, UnitOfMeasure> uomMapping, DateTime localDateTime, List<string> errors)
        {
            var orderHeader = new JdeOrderHeader(transaction);

            //3
            orderHeader.Column[3].Value = site.JdeCompanyNumber;

            //4
            if (transaction.TransactionLogId.HasValue)
            {
                orderHeader.Column[4].Value = transaction.TransactionLogId.Value.ToString();
            }
            else
            {
                errors.Add($"A transaction with no value for transaction_log_id encountered. ticket={transaction.TicketNumber}");
            }

            //9
            orderHeader.Column[9].Value = localDateTime.ToString("dd/MM/yy");

            //23
            orderHeader.Column[23].Value = site.JdeBusinessUnit;

            //32
            var accoutNumber = LeadingDigits(transaction.ClientAccountNumber);
            orderHeader.Column[32].Value = accoutNumber;

            //35
            System.DateTime dateOut;
            if (DateTime.TryParse(transaction.DateOut, out dateOut))
            {
                orderHeader.Column[35].Value = dateOut.ToString("dd/MM/yy");
            }
            else
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: Date_out has value = {transaction.DateOut} which cannot be parsed into a DateTime.  Expected date in YYYY-MM-DDTHH:MM:SS format");
            }

            //43
            orderHeader.Column[43].Value = transaction.ClientOrderNumber;

            //44
            orderHeader.Column[44].Value = transaction.TicketNumber;

            errors.AddRange(orderHeader.GetErrors(transaction.TransactionLogId.Value.ToString()));
            return orderHeader.ToString();
        }

        private static string buildOrderDetail(Transaction transaction, Site site, IDictionary<string, UnitOfMeasure> uomMapping, DateTime tenantLocalDateTime, List<string> errors, int detailLineNumber, OrderDetailType orderDetailType)
        {
            // line1  Only 23 fields are given values out of 165 total fields
            var orderDetail = new JdeOrderDetail();

            //3
            orderDetail.Column[3].Value = site.JdeCompanyNumber;

            //4
            if (transaction.TransactionLogId.HasValue)
            {
                orderDetail.Column[4].Value = transaction.TransactionLogId.Value.ToString();
            }
            else
            {
                errors.Add($"A transaction with no value for transaction_log_id encountered. ticket={transaction.TicketNumber}");
            }

            //6
            orderDetail.Column[6].Value = $"{detailLineNumber}.000";

            //20
            orderDetail.Column[20].Value = site.JdeBusinessUnit;

            //30
            orderDetail.Column[30].Value = "";

            //33
            orderDetail.Column[33].Value = LeadingDigits(transaction.ClientOrderNumber);


            //36
            System.DateTime dateOut;
            if (DateTime.TryParse(transaction.DateOut, out dateOut))
            {
                orderDetail.Column[36].Value = dateOut.ToString("dd/MM/yy");
            }
            else
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: Date_out has value = {transaction.DateOut} which cannot be parsed into a DateTime.  Expected date in dd/MM/yy format");
            }

            //50
            switch (orderDetailType)
            {
                case OrderDetailType.NotEpaSplit:
                    //50
                    orderDetail.Column[50].Value = LeadingDigits(transaction.ProductCode);
                    break;
                case OrderDetailType.EpaSplitRecord1:
                    //50
                    orderDetail.Column[50].Value = LeadingDigits(transaction.ProductCode);
                    break;
                case OrderDetailType.EpaSplitRecord2:
                    //50
                    orderDetail.Column[50].Value = "DummyEpaLevyProduct";
                    break;
            }

            //81
            var translatedUnitOfMeasure = uomMapping.FirstOrDefault(x => x.Key == transaction.StockUnit).Value;
            if (translatedUnitOfMeasure == null)
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: Mandalay stock_unit = {transaction.StockUnit} but no matching entry found in the UnitOfMeasure table.");
            }
            else
            {
                orderDetail.Column[81].Value = translatedUnitOfMeasure.JdeUnitOfMeasure;
            }


            //82
            switch (orderDetailType)
            {
                case OrderDetailType.NotEpaSplit:
                    //82
                    if (transaction.ChargedQuantity.HasValue)
                    {
                        orderDetail.Column[82].Value = transaction.ChargedQuantity.Value.ToString();
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: charged_qty has no value.  Expected decimal value.");
                    }
                    break;
                case OrderDetailType.EpaSplitRecord1:
                    //82
                    if (transaction.ChargedQuantity.HasValue)
                    {
                        orderDetail.Column[82].Value = transaction.ChargedQuantity.Value.ToString();
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: charged_qty has no value.  Expected decimal value.");
                    }
                    break;
                case OrderDetailType.EpaSplitRecord2:
                    //82
                    if (transaction.Quantity.HasValue)
                    {
                        orderDetail.Column[82].Value = transaction.Quantity.Value.ToString();
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: quantity has no value.  Expected decimal value.");
                    }
                    break;
            }

            //92
            switch (orderDetailType)
            {
                case OrderDetailType.NotEpaSplit:
                    //92
                    if (transaction.TotalExcludingTax.HasValue && transaction.ChargedQuantity.HasValue && transaction.ChargedQuantity.Value != 0)
                    {
                        var calulated = transaction.TotalExcludingTax.Value / transaction.ChargedQuantity.Value;
                        orderDetail.Column[92].Value = calulated.ToString("0.0000");
                    }
                    else
                    {
                        if (!transaction.TotalExcludingTax.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: total_ex_tax has no value.  Expected decimal value.");
                        }
                        if (!transaction.ChargedQuantity.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: charged_qty has no value.  Expected decimal value.");
                        }
                        if (transaction.ChargedQuantity.HasValue && transaction.ChargedQuantity == 0)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId} charged_qty = 0.  Expected not = 0.");
                        }
                    }
                    break;
                case OrderDetailType.EpaSplitRecord1:
                    //92
                    if (transaction.TotalExcludingTax.HasValue && transaction.ChargedQuantity.HasValue && transaction.ChargedQuantity.Value != 0)
                    {
                        var calulated = transaction.TotalExcludingTax.Value / transaction.ChargedQuantity.Value;
                        orderDetail.Column[92].Value = calulated.ToString("0.0000");
                    }
                    else
                    {
                        if (!transaction.TotalExcludingTax.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: total_ex_tax has no value.  Expected decimal value.");
                        }
                        if (!transaction.ChargedQuantity.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: charged_qty has no value.  Expected decimal value.");
                        }
                        if (transaction.ChargedQuantity.HasValue && transaction.ChargedQuantity == 0)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId} charged_qty = 0.  Expected not = 0.");
                        }
                    }

                    break;
                case OrderDetailType.EpaSplitRecord2:
                    //92
                    if (transaction.EpaLevy.HasValue && transaction.Quantity.HasValue && transaction.Quantity.Value != 0)
                    {
                        var calulated = transaction.EpaLevy.Value / transaction.Quantity.Value;
                        orderDetail.Column[92].Value = calulated.ToString("0.0000");
                    }
                    else
                    {
                        if (!transaction.EpaLevy.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: epa_levy has no value.  Expected decimal value.");
                        }
                        if (!transaction.Quantity.HasValue)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId}: quanity has no value.  Expected decimal value.");
                        }
                        if (transaction.Quantity.HasValue && transaction.Quantity == 0)
                        {
                            errors.Add($"On transaction_log_id={transaction.TransactionLogId} quantity = 0.  Expected not = 0.");
                        }
                    }
                    break;
            }

            //93
            switch (orderDetailType)
            {
                case OrderDetailType.NotEpaSplit:
                    //93
                    if (transaction.TotalExcludingTax.HasValue)
                    {
                        orderDetail.Column[93].Value = transaction.TotalExcludingTax.Value.ToString("0.00");
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: total_ex_tax has no value.  Expected decimal value.");
                    }
                    break;
                case OrderDetailType.EpaSplitRecord1:
                    //93
                    if (transaction.PriceExcludingTax.HasValue)
                    {
                        orderDetail.Column[93].Value = transaction.PriceExcludingTax.Value.ToString("0.00");
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: price_ex_tax has no value.  Expected decimal value.");
                    }
                    break;
                case OrderDetailType.EpaSplitRecord2:
                    //93
                    if (transaction.EpaLevy.HasValue)
                    {
                        orderDetail.Column[93].Value = transaction.EpaLevy.Value.ToString("0.00");
                    }
                    else
                    {
                        errors.Add($"On transaction_log_id={transaction.TransactionLogId}: total_ex_tax has no value.  Expected decimal value.");
                    }
                    break;
            }

            //95
            orderDetail.Column[95].Value = "1";

            //145
            orderDetail.Column[145].Value = transaction.LicensePlate;

            //146
            orderDetail.Column[146].Value = ""; // default value for 146
            switch (orderDetailType)
            {
                case OrderDetailType.NotEpaSplit:
                    if (transaction.ChargedQuantity.HasValue && transaction.Quantity.HasValue)
                    {
                        if (transaction.ChargedQuantity.Value > transaction.Quantity.Value)
                        {
                            orderDetail.Column[146].Value = "N";
                        }
                    }
                    break;
                case OrderDetailType.EpaSplitRecord1:
                    orderDetail.Column[146].Value = "";
                    break;
                case OrderDetailType.EpaSplitRecord2:
                    orderDetail.Column[146].Value = "S";
                    break;
            }

            //159
            if (translatedUnitOfMeasure == null)
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: Mandalay stock_unit = {transaction.StockUnit} but no matching entry found in the UnitOfMeasure table.");
            }
            else
            {
                orderDetail.Column[159].Value = translatedUnitOfMeasure.JdeUnitOfMeasure;
            }

            //160
            if (transaction.NettWeightTonnes.HasValue)
            {
                orderDetail.Column[160].Value = transaction.NettWeightTonnes.Value.ToString("0.0000");
            }
            else
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: expected nett_weight_tonnes to have a value");
            }

            //161
            orderDetail.Column[161].Value = "T";

            //218
            System.DateTime dateOut2;
            if (DateTime.TryParse(transaction.DateIn, out dateOut2))
            {
                orderDetail.Column[218].Value = dateOut.ToString("HHmm");
            }
            else
            {
                errors.Add($"On transaction_log_id={transaction.TransactionLogId}: Date_out has value = {transaction.DateOut} which cannot be parsed into a DateTime.  Expected date in YYYY-MM-DDTHH:MM:SS format");
            }

            errors.AddRange(orderDetail.GetErrors(transaction.TransactionLogId.Value.ToString()));
            return orderDetail.ToString();

        }

        private static string LeadingDigits(string s)
        {
            var pattern = "^\\d*";
            Regex rg = new Regex(pattern);
            MatchCollection matches = rg.Matches(s);
            string returnValue;
            if (matches.Count > 0)
            {
                returnValue = "";
            }
            else
            {
                returnValue = matches[0].Value;
            }

            return returnValue;
        }

        private static DateTime GetTenancyLocalTime()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            var localTime = System.TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return localTime;
        }
    }
}
