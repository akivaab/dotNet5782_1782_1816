using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
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
    }
}

