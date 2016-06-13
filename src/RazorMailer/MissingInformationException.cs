using System;

namespace RazorMailer
{
    public class MissingInformationException : Exception
    {
        public MissingInformationException(string message) : base(message)
        {
        }
    }
}