using System;

namespace MandalayClient.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CreateableAttribute : Attribute
    {
        public bool Createable { get; private set; }

        public CreateableAttribute(bool createable)
        {
            Createable = createable;
        }
    }
}
