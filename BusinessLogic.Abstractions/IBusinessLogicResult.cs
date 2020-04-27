using System;
using System.Collections.Generic;
using BusinessLogic.Abstractions.Message;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicResult
    {
        bool Succeeded { get; }
        IList<IPresentationMessage> Messages { get; }
        Exception Exception { get; }
    }

    public interface IBusinessLogicResult<out TResult> : IBusinessLogicResult
    {
        TResult Result { get; }
        int PageCount { get; }
    }
}
