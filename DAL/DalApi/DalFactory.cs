using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="instance">string representing which instantiation to choose.</param>
        /// <returns>An IDal object.</returns>
        public static IDal GetDal(string instance)
        {
            switch (instance)
            {
                case "DalObject":
                    return DalObject.DalObject.instance;
                //case "DalXml":
                //    return DalXml.DalXml.instance;
                default:
                    throw new DO.IllegalArgumentException("The given string does not match any instance.");
            }
        }
    }
}
