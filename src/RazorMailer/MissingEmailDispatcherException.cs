using System;

namespace RazorMailer
{
    public class MissingEmailDispatcherException : Exception
    {
        public MissingEmailDispatcherException(string message) : base(message)
        {
        }
    }
}