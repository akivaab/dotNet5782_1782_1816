using System;

namespace DalObject
{
    /// <summary>
    /// Initialization and Singleton implementation of the Data Layer.
    /// </summary>
    sealed partial class DalObject : DalApi.IDal
    {
        public bool DataCleanupRequired { get; }

        #region Fields
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a DalObject.
        /// </summary>
        private static readonly Lazy<DalObject> lazyDalObject = new Lazy<DalObject>(() => new DalObject());

        /// <summary>
        /// Instance of the DalObject that is first instantiated when the getter is called.
        /// </summary>
        internal static DalObject instance { get { return lazyDalObject.Value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// A private constructor to maintain Singleton design pattern.
        /// </summary>
        private DalObject() 
        {
            DataCleanupRequired = true;
        }
        #endregion
    }
}
