using System.Collections.Generic;

namespace MandalayJdeIntegrationCore.Models.Jde
{
    internal class JdeCsvLine
    {
        public JdeField[] Column { get; set; }

        internal List<string> assignmentErrors = new List<string>();

        public JdeCsvLine(int fieldCount)
        {
            Column = new JdeField[fieldCount + 1];
            for (int i = 0; i < fieldCount + 1; i++)
            {
                Column[i] = new JdeField() { FieldType = JdeFieldType.NotUsed, Length = 0, Decimal = 0, Position = i };
            }
        }

        public override string ToString()
        {
            List<string> stringValues = new List<string>();
            for (int i = 1; i < Column.Length; i++)  // skipping zero item because filed[1] is the first column not fileds[0]
            {
                stringValues.Add(Column[i].QuotedValue);
            }

            // but when there are no errors we generate the CSV line
            return (string.Join(',', stringValues));
        }

        public List<string> GetErrors(string txnLogId)
        {
            List<string> rangeErrors = new List<string>();
            rangeErrors.AddRange(assignmentErrors);
            for (int i = 1; i < Column.Length; i++)  // skipping zero item because filed[1] is the first column not fileds[0]
            {
                string error = Column[i].Validate(txnLogId);
                if (error != null)
                {
                    rangeErrors.Add(error);
                }
            }
            return rangeErrors;
        }
    }
}
