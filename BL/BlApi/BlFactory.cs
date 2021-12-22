using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>An IBL object</returns>
        public static IBL GetBl()
        {
            return BL.BL.instance;
        }
    }
}
