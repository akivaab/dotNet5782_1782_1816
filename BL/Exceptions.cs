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
        public class UnableToCharge : Exception
        {
            public UnableToCharge() : base() { }
        }

        public class UnableToRelease : Exception
        {
            public UnableToRelease() : base() { }
        }
    }
}

