using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public override string ToString()
            {
                return $"Customer Name: {Name}, ID: {ID}, Phone Number: {Phone}, Location: {Latitude}, {Longitude}";
            }
        }
    }
}

