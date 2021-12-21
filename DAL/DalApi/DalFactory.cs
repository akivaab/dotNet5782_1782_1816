using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public static class DalFactory
    {
        public static IDal GetDal(string instance)
        {
            switch (instance)
            {
                case "DalObject":
                    return DalObject.DalObject.Instance;
                //case "DalXml":
                //    return DalXml.DalXml.Instance;
                default:
                    throw new DO.UndefinedStringException();
            }
        }
    }
}
