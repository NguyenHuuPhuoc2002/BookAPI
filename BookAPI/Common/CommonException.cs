
using System.Globalization;


namespace Common.Exceptions
{
    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
    public class DuplicateException : Exception
    {
        public DuplicateException() : base() { }

        public DuplicateException(string message) : base(message) { }

        public DuplicateException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
    public class PasswordException : Exception
    {
        public PasswordException() : base() { }

        public PasswordException(string message) : base(message) { }

        public PasswordException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

}
