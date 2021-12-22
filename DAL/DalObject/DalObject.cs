using System;

namespace DalObject
{
    sealed partial class DalObject : DalApi.IDal
    {
        private static readonly Lazy<DalObject> lazyDalObject = new Lazy<DalObject>(() => new DalObject());

        internal static DalObject Instance { get { return lazyDalObject.Value; } }

        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        private DalObject()
        {
            DataSource.Initialize();
        }
    }
}
