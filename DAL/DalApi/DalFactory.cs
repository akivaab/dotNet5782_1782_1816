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
        public static IDal GetDal(string instance)
        {
            switch (instance)
            {
                case "DalObject":
                    return DalObject.DalObject.instance;
                case "DalXml":
                    return DalXml.DalXml.instance;
                default:
                    throw new IllegalArgumentException("The given string does not match any instance.");
            }
        }
    }
}
