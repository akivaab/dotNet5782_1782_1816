using System;

namespace DalObject
{
    /// <summary>
    /// Initialization and Singleton implementation of the Data Layer.
    /// </summary>
    sealed partial class DalObject : DalApi.IDal
    {
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a DalObject.
        /// </summary>
        private static readonly Lazy<DalObject> lazyDalObject = new Lazy<DalObject>(() => new DalObject());

        /// <summary>
        /// Instance of the DalObject that is first instantiated when the getter is called.
        /// </summary>
        internal static DalObject instance { get { return lazyDalObject.Value; } }

        /// <summary>
        /// A constructor that adds initial values to the entity lists, private to maintain Singleton design pattern.
        /// </summary>
        private DalObject()
        {
            DataSource.Initialize();
        }
    }
}
