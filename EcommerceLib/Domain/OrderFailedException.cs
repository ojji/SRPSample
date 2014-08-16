using System;

namespace EcommerceLib.Domain
{
    public class OrderFailedException : Exception
    {
        public OrderFailedException()
        {
        }

        public OrderFailedException(string message)
            : base(message)
        {
        }

        public OrderFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}