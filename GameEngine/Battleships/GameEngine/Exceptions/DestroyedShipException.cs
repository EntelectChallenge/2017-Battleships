using System;

namespace GameEngine.Exceptions
{
    public class DestroyedShipException : Exception
    {
        public DestroyedShipException(string message) :
            base(message)
        {
        }

        public DestroyedShipException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}