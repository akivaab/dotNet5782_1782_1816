using System;

namespace IDAL
{
    namespace DO
    {
        class ExceededLimitException : Exception
        {
            public ExceededLimitException() : base() { }
        }

        class NonUniqueIdException : Exception
        {
            public NonUniqueIdException() : base() {}
        }

        class UndefinedObjectException : Exception
        {
            public UndefinedObjectException() : base() {}
        }
    }
}
