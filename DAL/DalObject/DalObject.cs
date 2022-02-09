using System;

namespace DalObject
{
    /// <summary>
    /// Initialization and Singleton implementation of the Data Layer.
    /// </summary>
    sealed partial class DalObject : DalApi.IDal
    {
        #region Fields and Properties
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a DalObject.
        /// </summary>
        private static readonly Lazy<DalObject> lazyDalObject = new Lazy<DalObject>(() => new DalObject());

        /// <summary>
        /// Instance of the DalObject that is first instantiated when the getter is called.
        /// </summary>
        internal static DalObject instance => lazyDalObject.Value;

        public bool DataCleanupRequired { get; init; } = true;
        #endregion

        #region Constructor
        /// <summary>
        /// A private constructor to maintain Singleton design pattern.
        /// </summary>
        private DalObject() { }
        #endregion
    }
}
