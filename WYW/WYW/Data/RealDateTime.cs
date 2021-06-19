using System;

namespace WYW.Data
{
    public class RealDateTime : IDateTime
    {
        public DateTimeOffset UtcNow()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}
