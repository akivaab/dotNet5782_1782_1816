using System;

namespace IDAL
{
    namespace DO
    {
        public class IllegalArgumentException : Exception
        {
            public IllegalArgumentException() : base() {}
        }
        public class NonUniqueIdException : Exception
        {
            public NonUniqueIdException() : base() {}
        }

        public class UndefinedObjectException : Exception
        {
            public UndefinedObjectException() : base() {}
        }
    }
}
