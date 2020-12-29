using System;

namespace BusinessLogic.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            
        }

        public NotFoundException(string message) : base(message) 
        {
            
        }
    }
}