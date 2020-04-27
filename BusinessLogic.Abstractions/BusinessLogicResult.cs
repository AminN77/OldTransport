using System;
using System.Collections.Generic;
using BusinessLogic.Abstractions.Message;

namespace BusinessLogic.Abstractions
{
    public class BusinessLogicResult : IBusinessLogicResult
    {
        public bool Succeeded { get; }
        public IList<IPresentationMessage> Messages { get; }
        public Exception Exception { get; }

        public BusinessLogicResult(bool succeeded, IEnumerable<IBusinessLogicMessage> messages = null, Exception exception = null)
        {
            Succeeded = succeeded;
            Exception = exception;
            Messages = new List<IPresentationMessage>();
            if (messages == null) return;
            foreach (var message in messages)
            {
                Messages.Add(message);
            }
        }
    }

    public class BusinessLogicResult<TResult> : BusinessLogicResult, IBusinessLogicResult<TResult>
    {
        public TResult Result { get; }
        public int PageCount { get; }

        public BusinessLogicResult(bool succeeded, TResult result, IEnumerable<IBusinessLogicMessage> messages = null, Exception exception = null, int pageCount = 0)
            : base(succeeded, messages, exception)
        {
            Result = result;
            PageCount = pageCount;
        }
    }
}
