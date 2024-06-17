namespace MandalayJdeIntegrationCore.Models.Jde
{
    internal class JdeOrderDetail : JdeCsvLine
    {
        public JdeOrderDetail() : base(252)   // Order header has 252 fileds
        {
            InitColumns();
        }

        private void InitColumns()
        {
            Column[1] = new JdeField() 
            { 
                Position = 1,
                FieldId = "SZEDTY", Description = "Type Record", FieldType = JdeFieldType.Character, Length = 1, Decimal = 0, WasteMan = "1" 
            };
            Column[1].Value = "2";

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

            Column[6] = new JdeField() 
            { 
                Position = 6, FieldId = "SZEDLN", Description = "EDI -Line Number", FieldType = JdeFieldType.Numeric, Length = 7, Decimal = 3, WasteMan = "1.000, 2.000" 
            };
            Column[7] = new JdeField() 
            { 
                Position = 7, FieldId = "SYEDST", Description = "EDI -Transaction SetNumber", FieldType = JdeFieldType.String, Length = 6, Decimal = 0, WasteMan = "850" 
            };
            Column[7].Value = "850";

            Column[10] = new JdeField() 
            { 
                Position = 10, FieldId = "SYEDER", Description = "EDI -Send/Receive Indicato", FieldType = JdeFieldType.Character, Length = 1, Decimal = 0, WasteMan = "R" 
            };
            Column[10].Value = "R";

            Column[20] = new JdeField() 
            { 
                Position = 20, FieldId = "SZMCU", Description = "Business Unit", FieldType = JdeFieldType.String, Length = 12, Decimal = 0, WasteMan = "Branch/Site" 
            };

            Column[30] = new JdeField() 
            { 
                Position = 30, FieldId = "SZDMCT", Description = "Agreement Number -Distribution", FieldType = JdeFieldType.String, Length = 12, Decimal = 0, WasteMan = "Approval Numbere.g.B8012" 
            };

            Column[33] = new JdeField() 
            { 
                Position = 33, FieldId = "SZSHAN", Description = "Address Number -Ship To", FieldType = JdeFieldType.NumericOrBlank, Length = 8, Decimal = 0, WasteMan = "WasteMan short name mapped to JDE customer number" 
            };

            Column[36] = new JdeField() 
            { 
                Position = 36, FieldId = "SZTRDJ", Description = "Date -Order/Transaction", FieldType = JdeFieldType.Date, Length = 8, Decimal = 0, WasteMan = "Service Date" 
            };

            Column[50] = new JdeField() 
            { 
                Position = 50, FieldId = "SZLITM", Description = "2nd Item Number", FieldType = JdeFieldType.String, Length = 25, Decimal = 0, WasteMan = "Service product" 
            };

            Column[81] = new JdeField() 
            { 
                Position = 81, FieldId = "SZUOM", Description = "Unit of Measure as Input", FieldType = JdeFieldType.String, Length = 2, Decimal = 0, WasteMan = "Code for method" 
            };

            Column[82] = new JdeField() 
            { 
                Position = 82, FieldId = "SZUORG", Description = "Units -Order/Transaction Quantity", FieldType = JdeFieldType.Numeric, Length = 15, Decimal = 2, WasteMan = "1.52" 
            };

            Column[92] = new JdeField() 
            { 
                Position = 92, FieldId = "SZUPRC", Description = "Amount – Price per Unit", FieldType = JdeFieldType.Numeric, Length = 15, Decimal = 4, WasteMan = "99.77" 
            };

            Column[93] = new JdeField() 
            { 
                Position = 93, FieldId = "SZAEXP", Description = "Amount -Extended Price", FieldType = JdeFieldType.Numeric, Length = 15, Decimal = 2, WasteMan = "115.6" 
            };

            Column[95] = new JdeField() 
            { 
                Position = 95, FieldId = "SZPROV", Description = "Price Override Code", FieldType = JdeFieldType.Character, Length = 1, Decimal = 0, WasteMan = "1" 
            };
            Column[95].Value = "1";

            Column[145] = new JdeField() 
            { 
                Position = 145, FieldId = "SZCNID", Description = "Container I.D.", FieldType = JdeFieldType.String, Length = 20, Decimal = 0, WasteMan = "Vehicle registratione.g.SUY197/06" 
            };

            Column[146] = new JdeField() 
            { 
                Position = 146, FieldId = "SZFRTH", Description = "Freight Handling Code", FieldType = JdeFieldType.String, Length = 3, Decimal = 0, WasteMan = "Flag to indicate rate should not printN = do not print rateS = surcharge line" 
            };

            Column[159] = new JdeField() 
            { 
                Position = 159, FieldId = "SZUOM4", Description = "Unit of Measure -Pricing", FieldType = JdeFieldType.String, Length = 2, Decimal = 0, WasteMan = "Pricing UoM" 
            };

            Column[160] = new JdeField() 
            { 
                Position = 160, FieldId = "SZITWT", Description = "Unit Weight", FieldType = JdeFieldType.Numeric, Length = 15, Decimal = 4, WasteMan = "Total Weight" 
            };

            Column[161] = new JdeField() 
            { 
                Position = 161, FieldId = "SZWTUM", Description = "Weight Unit of Measure", FieldType = JdeFieldType.String, Length = 2, Decimal = 0, WasteMan = "Weight Unit of Measure" 
            };

            Column[218] = new JdeField() 
            { 
                Position = 218, FieldId = "SZURAB", Description = "User Reserved Number", FieldType = JdeFieldType.Numeric, Length = 8, Decimal = 0, WasteMan = "1300Time of service" 
            };
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
