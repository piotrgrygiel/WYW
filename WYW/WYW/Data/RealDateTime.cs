using System;

namespace WYW.Data
{
    public class RealDateTime : IDateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
