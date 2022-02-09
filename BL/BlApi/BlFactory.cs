using System;
using BO;

namespace BlApi
{
    /// <summary>
    /// Factory to instantiate a BL object.
    /// </summary>
    public static class BlFactory
    {
        /// <summary>
        /// Get a BL object.
        /// </summary>
        /// <returns>An IBL object.</returns>
        /// <exception cref="InstanceInitializationException">The BL instance was not created properly.</exception>
        public static IBL GetBl()
        {
            try { return BL.BL.instance; }
            catch (Exception) { throw new InstanceInitializationException("Failed to instantiate BL."); }
        }
    }
}
