using MandalayJdeIntegrationCore.Extensions;
using System;
using System.Globalization;

namespace MandalayJdeIntegrationCore.Models.Jde
{
    public class JdeField
    {
        private string fieldValue;

        public int Position { get; set; }
        public string FieldId { get; set; }
        public string Description { get; set; }
        public JdeFieldType FieldType { get; set; }
        public int Length { get; set; }
        public int Decimal { get; set; }
        public string WasteMan { get; set; }

        public string Value
        {
            get 
            { 
                return fieldValue; 
            }
            set 
            { 
                if (this.FieldType == JdeFieldType.NotUsed) 
                    throw new ApplicationException("Attempt to set a Value in a JdeField that should not have a value");
                fieldValue = value; 
            }
        }

        public string QuotedValue
        {
            get
            {
                if (this.FieldType == JdeFieldType.NotUsed || Value.IsNullOrEmpty())
                {
                    return "\"\"";  
                }

                return @"""" + Value.Replace(@"""", @"""""") + @"""";
            }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                throw new Exception($"Value of {FieldId} used but it is null");
            }
            return Value;
        }

        internal string Validate(string transaction_log_id)
        {
            // Test that the value seems correct given type length Dec
            if (this.Length > 0)
            {
                if (this.Value != null)
                {
                    int actualLength = this.Value.Length;

                    if (actualLength > this.Length)
                    {
                        return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Max Length {this.Length} but value \"{this.Value}\" is longer.";
                    }
                }
            }

            switch (this.FieldType)
            {
                case JdeFieldType.Character:
                    break;
                case JdeFieldType.String:
                    break;
                case JdeFieldType.Numeric:
                    decimal testValue;
                    if (!decimal.TryParse(this.Value, out testValue))
                    {
                        return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Numeric but value \"{this.Value}\" which cannot be converted to decimal.";
                    }
                    // check decimal places
                    int idxPoint = this.Value.IndexOf('.');
                    if (idxPoint > 0)
                    {
                        var decimals = this.Value.Substring(idxPoint + 1).Length;
                        if (decimals > this.Decimal)
                        {
                            return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Numeric with {this.Decimal} decimal places but it has {decimals} decimal places.";
                        }
                    }
                    if (this.Length > 0)
                    {
                        int actualLength = this.Value.Length;
                        if (actualLength > this.Length)
                        {
                            return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Max Length {this.Length} but value \"{Value}\" is longer.";
                        }
                    }
                    break;
                case JdeFieldType.NumericOrBlank:
                    if (!string.IsNullOrEmpty(this.Value))
                    {
                        decimal testValue2;
                        if (!decimal.TryParse(this.Value, out testValue2))
                        {
                            return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Numeric but value \"{this.Value}\" which cannot be converted to decimal.";
                        }
                        // check decimal places
                        int idxPoint2 = this.Value.IndexOf('.');
                        if (idxPoint2 > 0)
                        {
                            var decimals = this.Value.Substring(idxPoint2 + 1).Length;
                            if (decimals > this.Decimal)
                            {
                                return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Numeric with {this.Decimal} decimal places but it has {decimals} decimal places.";
                            }
                        }
                        if (this.Length > 0)
                        {
                            int actualLength = this.Value.Length;
                            if (actualLength > this.Length)
                            {
                                return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Max Length {this.Length} but value \"{this.Value}\" is longer.";
                            }
                        }
                    }
                    break;
                case JdeFieldType.Date:
                    try
                    {
                        DateTime dt = DateTime.ParseExact(this.Value, "d/M/yy", CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be Date with d/M/yy but has value {this.Value}";
                    }
                    break;
                case JdeFieldType.NotUsed:
                    if (!string.IsNullOrEmpty(this.Value))
                    {
                        return $"On transaction_log_id={transaction_log_id}: Column {this.Position} should be have no value but has {this.Value}";
                    }
                    break;
            }

            return null;
        }
    }
}
