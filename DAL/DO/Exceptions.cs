using System;

namespace DO
{
    /// <summary>
    /// An exception thrown when an illegal argument is passed.
    /// </summary>
    public class IllegalArgumentException : Exception
    {
        /// <summary>
        /// IllegalArgumentException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public IllegalArgumentException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when an entity is given an ID that is already assigned to another entity.
    /// </summary>
    public class NonUniqueIdException : Exception
    {
        /// <summary>
        /// NonUniqueIdException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public NonUniqueIdException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when an object or entity referenced does not exist.
    /// </summary>
    public class UndefinedObjectException : Exception
    {
        /// <summary>
        /// UndefinedObjectException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UndefinedObjectException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when there is a problem creating, saving, or loading XML files.
    /// </summary>
    public class XMLFileLoadCreateException : Exception
    {
        /// <summary>
        /// XMLFileLoadCreateException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public XMLFileLoadCreateException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when a class instance failed to be initialized properly.
    /// </summary>
    public class InstanceInitializationException : Exception
    {
        /// <summary>
        /// InstanceInitializationException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public InstanceInitializationException(string s) : base(s) { }
    }
}
