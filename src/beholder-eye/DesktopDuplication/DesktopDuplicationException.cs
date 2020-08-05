namespace beholder_eye
{
    using System;

    public class DesktopDuplicationException : Exception
    {
        public DesktopDuplicationException()
        {
        }

        public DesktopDuplicationException(string message)
            : base(message)
        {
        }

        public DesktopDuplicationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
