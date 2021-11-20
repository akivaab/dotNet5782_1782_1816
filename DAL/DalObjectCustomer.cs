using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDAL.IDal
    {
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (latitude < -1 || latitude > 1 || longitude < 0 || longitude > 2) //limited coordinate field
            {
                throw new IllegalArgumentException();
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

        public void UpdateCustomerName(int customerID, string name)
        {
            int customerIndex = DataSource.Customers.FindIndex(customer => customer.ID == customerID);
            
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            
            Customer customer = DataSource.Customers[customerIndex];
            customer.Name = name;
            DataSource.Customers[customerIndex] = customer;
        }

        public void UpdateCustomerPhone(int customerID, string phone)
        {
            int customerIndex = DataSource.Customers.FindIndex(customer => customer.ID == customerID);
            
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            
            Customer customer = DataSource.Customers[customerIndex];
            customer.Phone = phone;
            DataSource.Customers[customerIndex] = customer;
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
