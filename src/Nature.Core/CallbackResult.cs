using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nature.Core
{
    public class CallbackResult
    {
        public static readonly CallbackResult Success = new CallbackResult();

        public CallbackResult()
        {
        }

        public CallbackResult(params string[] errors)
        {
            this.Errors = errors;
        }

        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccess => Errors == null || !Errors.Any();
    }

    public class CallbackResult<T> : CallbackResult
    {
        public CallbackResult(T result)
        {
            Result = result;
        }

        public CallbackResult(params string[] errors)
            : base(errors)
        {
        }

        public T Result { get; private set; }
    }

}
