using System;

namespace Domain.Exceptions
{
    public class LocationOutOfBoundsException : Exception
    {
        public LocationOutOfBoundsException(string msg): base(msg)
        {
            
        }
    }
}