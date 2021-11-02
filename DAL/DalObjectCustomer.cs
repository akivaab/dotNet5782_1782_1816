﻿using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDal.IDal
    {
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (DataSource.Customers.Count >= 100)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Customers.FindIndex(customer => customer.ID == id) != -1)
            {
                throw new NonUniqueIdException();
            }
            Customer customer = new();
            customer.ID = id;
            customer.Name = name;
            customer.Phone = phone;
            customer.Latitude = latitude;
            customer.Longitude = longitude;
            DataSource.Customers.Add(customer);
        }

        public Customer DisplayCustomer(int customerID)
        {
            int customerIndex = DataSource.Customers.FindIndex(customer => customer.ID == customerID);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            return DataSource.Customers[customerIndex];
        }

        public IEnumerable<Customer> DisplayCustomersList()
        {
            List<Customer> customers = new();
            for (int i = 0; i < DataSource.Customers.Count; i++)
            {
                customers.Add(DataSource.Customers[i]);
            }
            return customers;
        }
    }
}
