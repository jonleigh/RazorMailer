using System;

namespace RazorMailer.Core
{
    public class MissingEmailDispatcherException : Exception
    {
        public MissingEmailDispatcherException(string message) : base(message)
        {
        }
    }
}