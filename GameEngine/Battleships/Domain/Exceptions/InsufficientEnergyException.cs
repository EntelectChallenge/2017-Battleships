using System;

namespace Domain.Exceptions
{
    public class InsufficientEnergyException : Exception
    {
        public InsufficientEnergyException(string message) : base(message)
        {
            
        }

        public InsufficientEnergyException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}