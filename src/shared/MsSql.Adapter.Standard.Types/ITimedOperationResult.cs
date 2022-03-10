using System;

namespace MsSql.Adapter.Standard.Types
{
    public interface ITimedOperationResult : IOperationResult
    {
        TimeSpan Duration { get; set; }
    }
}