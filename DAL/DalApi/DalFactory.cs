using System;
using DO;

namespace DalApi
{
    /// <summary>
    /// Factory to instantiate an IDal-implementing object.
    /// </summary>
    public static class DalFactory
    {
        /// <summary>
        /// Get a IDal-implementing object.
        /// </summary>
        /// <param name="instance">String representing which instantiation to choose.</param>
        /// <returns>An IDal object.</returns>
        /// <exception cref="IllegalArgumentException">The string does not match any instance.</exception>
        /// <exception cref="InstanceInitializationException">Either the DalObject or DalXml instance was not created properly.</exception>
        public static IDal GetDal(string instance)
        {
            switch (instance)
            {
                case "DalObject":
                    try { return DalObject.DalObject.instance; }
                    catch (Exception) { throw new InstanceInitializationException("Failed to instantiate DalObject."); }
                case "DalXml":
                    try { return DalXml.DalXml.instance; }
                    catch (Exception) { throw new InstanceInitializationException("Failed to instantiate DalXml."); }
                default:
                    throw new IllegalArgumentException("The given string does not match any instance.");
            }
        }
    }
}
