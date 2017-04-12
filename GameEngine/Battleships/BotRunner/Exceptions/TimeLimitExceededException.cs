using System;

namespace BotRunner.Exceptions
{
    public class TimeLimitExceededException : Exception
    {
        public TimeLimitExceededException(string message) :
            base(message)
        {

        }
    }
}