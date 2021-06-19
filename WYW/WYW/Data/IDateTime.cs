using System;

namespace WYW.Data
{
    public interface IDateTime
    {
        DateTimeOffset UtcNow();
    }
}
