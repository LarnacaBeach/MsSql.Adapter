﻿namespace MsSql.Adapter.Standard.Types
{
    public interface IOperationResult
    {
        int StatusCode { get; set; }
        string? StatusMessage { get; set; }

        bool Fail();
    }

    public interface IOperationResult<T> : IOperationResult
    {
        T? Data { get; set; }
    }
}