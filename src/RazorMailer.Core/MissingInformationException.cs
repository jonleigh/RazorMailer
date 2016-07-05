using System;

namespace RazorMailer.Core
{
    public class MissingInformationException : Exception
    {
        public MissingInformationException(string message) : base(message)
        {
        }
    }
}