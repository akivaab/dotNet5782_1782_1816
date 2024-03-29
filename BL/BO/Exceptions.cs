﻿using System;

namespace BO
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

        /// <summary>
        /// IllegalArgumentException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        /// <param name="e">A matching exception thrown from the data layer.</param>
        public IllegalArgumentException(string s, DO.IllegalArgumentException e) : base(s, e) { }
    }

    /// <summary>
    /// An exception thrown when a list/collection is empty but should not be.
    /// </summary>
    public class EmptyListException : Exception
    {
        /// <summary>
        /// EmptyListException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public EmptyListException(string s) : base(s) { }
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

        /// <summary>
        /// NonUniqueIdException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        /// <param name="e">A matching exception thrown from the data layer.</param>
        public NonUniqueIdException(string s, DO.NonUniqueIdException e) : base(s, e) { }
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

        /// <summary>
        /// UndefinedObjectException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        /// <param name="e">A matching exception thrown from the data layer.</param>
        public UndefinedObjectException(string s, DO.UndefinedObjectException e) : base(s, e) { }
    }

    /// <summary>
    /// An exception thrown when a drone cannot be sent to charge.
    /// </summary>
    public class UnableToChargeException : Exception
    {
        /// <summary>
        /// UnableToChargeException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToChargeException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when a drone cannot be released from charging.
    /// </summary>
    public class UnableToReleaseException : Exception
    {
        /// <summary>
        /// UnableToReleaseException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToReleaseException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when a package cannot be assigned to a drone.
    /// </summary>
    public class UnableToAssignException : Exception
    {
        /// <summary>
        /// UnableToAssignException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToAssignException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when a package cannot be collected by a drone.
    /// </summary>
    public class UnableToCollectException : Exception
    {
        /// <summary>
        /// UnableToCollectException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToCollectException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when a package cannot be delivered by a drone.
    /// </summary>
    public class UnableToDeliverException : Exception
    {
        /// <summary>
        /// UnableToDeliverException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToDeliverException(string s) : base(s) { }
    }

    /// <summary>
    /// An exception thrown when an entity cannot be removed.
    /// </summary>
    public class UnableToRemoveException : Exception
    {
        /// <summary>
        /// UnableToRemoveException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public UnableToRemoveException(string s) : base(s) { }
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
        /// <param name="e">A matching exception thrown from the data layer.</param>
        public XMLFileLoadCreateException(string s, DO.XMLFileLoadCreateException e): base(s, e) { }
    }

    /// <summary>
    /// An exception throw when some error occured while making a LINQ query.
    /// </summary>
    public class LinqQueryException : Exception
    {
        /// <summary>
        /// LinqQueryException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        public LinqQueryException(string s) : base(s) { }
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

        /// <summary>
        /// InstanceInitializationException constructor.
        /// </summary>
        /// <param name="s">A message describing the exception.</param>
        /// <param name="e">A matching exception thrown from the data layer.</param>
        public InstanceInitializationException(string s, DO.InstanceInitializationException e) : base(s, e) { }
    }
}
