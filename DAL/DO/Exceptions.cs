using System;

namespace DO
{
    public class IllegalArgumentException : Exception
    {
        public IllegalArgumentException(string s) : base(s) { }
    }
    public class NonUniqueIdException : Exception
    {
        public NonUniqueIdException(string s) : base(s) { }
    }
    public class UndefinedObjectException : Exception
    {
        public UndefinedObjectException(string s) : base(s) { }
    }
    public class UndefinedStringException : Exception
    {
        public UndefinedStringException(string s) : base(s) { }
    }
}
