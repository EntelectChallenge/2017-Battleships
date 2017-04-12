using System;

namespace GameEngine.Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) :
            base(message)
        {
        }

        public InvalidCommandException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}