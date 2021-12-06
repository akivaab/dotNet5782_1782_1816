using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class IllegalArgumentException : Exception
        {
            public IllegalArgumentException() : base() { }
        }
        public class EmptyListException : Exception
        {
            public EmptyListException() : base() { }
        }
        public class NonUniqueIdException : Exception
        {
            public NonUniqueIdException() : base() { }
        }
        public class UndefinedObjectException : Exception
        {
            public UndefinedObjectException() : base() { }
        }
        public class UnableToChargeException : Exception
        {
            public UnableToChargeException() : base() { }
        }
        public class UnableToReleaseException : Exception
        {
            public UnableToReleaseException() : base() { }
        }
        public class UnableToAssignException : Exception
        {
            public UnableToAssignException() : base() { }
        }
        public class UnableToCollectException : Exception
        {
            public UnableToCollectException() : base() { }
        }
        public class UnableToDeliverException : Exception
        {
            public UnableToDeliverException() : base() { }
        }
    }
}

