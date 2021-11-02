using System;

namespace IDAL
{
    namespace DO
    {
        public class ExceededLimitException : Exception
        {
            public ExceededLimitException() : base() { }
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
