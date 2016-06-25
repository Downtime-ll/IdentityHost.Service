using System;

namespace Nature.Core
{
    [Serializable]
    public class NatureException : ApplicationException
    {
        public NatureException(string message)
            : base(message) {
        }

        public NatureException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
