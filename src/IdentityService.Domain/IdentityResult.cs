using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.Domain
{
    public class IdentityResult
    {
        public static readonly IdentityResult Success = new IdentityResult();

        public IdentityResult()
        {
        }

        public IdentityResult(params string[] errors)
        {
            this.Errors = errors;
        }

        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccess => Errors == null || !Errors.Any();
    }

    public class IdentityResult<T> : IdentityResult
    {
        public IdentityResult(T result)
        {
            Result = result;
        }

        public IdentityResult(params string[] errors)
            : base(errors)
        {
        }

        public T Result { get; private set; }
    }
}
