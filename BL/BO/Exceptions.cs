using System;

namespace BO
{
    public class IllegalArgumentException : Exception
    {
        public IllegalArgumentException(string s) : base(s) { }
    }
    public class EmptyListException : Exception
    {
        public EmptyListException(string s) : base(s) { }
    }
    public class NonUniqueIdException : Exception
    {
        public NonUniqueIdException(string s) : base(s) { }
    }
    public class UndefinedObjectException : Exception
    {
        public UndefinedObjectException(string s) : base(s) { }
    }
    public class UnableToChargeException : Exception
    {
        public UnableToChargeException(string s) : base(s) { }
    }
    public class UnableToReleaseException : Exception
    {
        public UnableToReleaseException(string s) : base(s) { }
    }
    public class UnableToAssignException : Exception
    {
        public UnableToAssignException(string s) : base(s) { }
    }
    public class UnableToCollectException : Exception
    {
        public UnableToCollectException(string s) : base(s) { }
    }
    public class UnableToDeliverException : Exception
    {
        public UnableToDeliverException(string s) : base(s) { }
    }
}
