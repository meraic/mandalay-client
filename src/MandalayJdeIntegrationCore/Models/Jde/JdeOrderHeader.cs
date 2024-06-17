using MandalayClient.Common.Models.Json.Transactions;

namespace MandalayJdeIntegrationCore.Models.Jde
{
    internal class JdeOrderHeader : JdeCsvLine
    {
        public JdeOrderHeader(Transaction transaction) : base(165)   // Order header has 165 fileds
        {
            InitColumns();
        }

        private void InitColumns()
        {
            // set the fields that are actually get given values.
            Column[1] = new JdeField() 
            { 
                Position = 1, FieldId = "SZEDTY", Description = "Type Record", FieldType = JdeFieldType.Character, Length = 1, Decimal = 0, WasteMan = "1", 
            };
            Column[1].Value = "1";

            Column[3] = new JdeField() 
            { 
                Position = 3, FieldId = "SZEKCO", Description = "Company-Key (EDI -Document Key Co)", FieldType = JdeFieldType.String, Length = 5, Decimal = 0, WasteMan = "Map to JDE company number" 
            };

            Column[4] = new JdeField() 
            { 
                Position = 4, FieldId = "SZEDOC", Description = "EDI -Document Number", FieldType = JdeFieldType.Numeric, Length = 9, Decimal = 0, WasteMan = "Assign Next Number" 
            };

            Column[5] = new JdeField() 
            { 
                Position = 5, FieldId = "SZEDCT", Description = "EDI -Document Type", FieldType = JdeFieldType.String, Length = 2, Decimal = 0, WasteMan = "X2" 
            };
            Column[5].Value = "X2";

            Column[7] = new JdeField() 
            { 
                Position = 7, FieldId = "SYEDST", Description = "EDI -Transaction SetNumber", FieldType = JdeFieldType.String, Length = 6, Decimal = 0, WasteMan = "850" 
            };
            Column[7].Value = "850";

            Column[9] = new JdeField() 
            { 
                Position = 9, FieldId = "SYEDDT", Description = "EDI -Transmission Date", FieldType = JdeFieldType.Date, Length = 8, Decimal = 0, WasteMan = "Set to system date" 
            };

            Column[10] = new JdeField() 
            { 
                Position = 10, FieldId = "SYEDER", Description = "EDI -Send/Receive Indicato", FieldType = JdeFieldType.Character, Length = 1, Decimal = 0, WasteMan = "R" 
            };
            Column[10].Value = "R";

            Column[18] = new JdeField() 
            { 
                Position = 18, FieldId = "SYTPUR", Description = "Transaction Set Purpose", FieldType = JdeFieldType.String, Length = 2, Decimal = 0, WasteMan = "02" 
            };
            Column[18].Value = "02";

            Column[23] = new JdeField() 
            { 
                Position = 23, FieldId = "SYMCU", Description = "Business Unit", FieldType = JdeFieldType.String, Length = 12, Decimal = 0, WasteMan = "Map WasteMan site to JDE branch" 
            };

            Column[32] = new JdeField() 
            { 
                Position = 32, FieldId = "SYSHAN", Description = "Address Number -Ship To", FieldType = JdeFieldType.NumericOrBlank, Length = 8, Decimal = 0, WasteMan = "Map WasteMan short name to JDE customer number" 
            };

            Column[35] = new JdeField() 
            { 
                Position = 35, FieldId = "SYTRDJ", Description = "Date -Order/Transaction", FieldType = JdeFieldType.Date, Length = 8, Decimal = 0, WasteMan = "Date([Data].Date_Out) Format: DD/MM/YY " 
            };

            Column[43] = new JdeField() 
            { 
                Position = 43, FieldId = "SYVR01", Description = "Reference", FieldType = JdeFieldType.String, Length = 25, Decimal = 0, WasteMan = "Customer Order Number" 
            };

            Column[44] = new JdeField() 
            { 
                Position = 44, FieldId = "SYVR02", Description = "Reference 2", FieldType = JdeFieldType.String, Length = 25, Decimal = 0, WasteMan = "Weighbridge docket numbere.g.HEL29747WW" 
            };
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
