﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DalXml
{
    /// <summary>
    /// Customer-related functionality of the Data Layer.
    /// </summary>
    partial class DalXml : DalApi.IDal
    {
        #region Add Methods
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            if (customers.Exists(customer => customer.ID == id && customer.Active))
            {
                throw new NonUniqueIdException("The given customer ID is not unique.");
            }

            Customer customer = new();
            customer.ID = id;
            customer.Name = name;
            customer.Phone = phone;
            customer.Latitude = latitude;
            customer.Longitude = longitude;
            customer.Active = true;
            customer.Password = id.ToString();
            customers.Add(customer);

            saveListToXMLSerializer<Customer>(customers, customerXmlPath);
        }
        #endregion

        #region Update Methods
        public void UpdateCustomerName(int customerID, string name)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);
            
            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            Customer customer = customers[customerIndex];
            customer.Name = name;
            customers[customerIndex] = customer;

            saveListToXMLSerializer<Customer>(customers, customerXmlPath);
        }

        public void UpdateCustomerPhone(int customerID, string phone)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            Customer customer = customers[customerIndex];
            customer.Phone = phone;
            customers[customerIndex] = customer;

            saveListToXMLSerializer<Customer>(customers, customerXmlPath);
        }

        public void UpdateCustomerPassword(int customerID, string password)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            Customer customer = customers[customerIndex];
            customer.Password = password;
            customers[customerIndex] = customer;

            saveListToXMLSerializer<Customer>(customers, customerXmlPath);
        }
        #endregion

        #region Remove Methods
        public void RemoveCustomer(int customerID)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID");
            }

            Customer customer = customers[customerIndex];
            customer.Active = false;
            customers[customerIndex] = customer;

            saveListToXMLSerializer<Customer>(customers, customerXmlPath);

        }
        #endregion

        #region Getter Methods
        public Customer GetCustomer(int customerID)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            return customers[customerIndex];
        }

        public IEnumerable<Customer> GetCustomersList()
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            return from customer in customers
                   where customer.Active
                   select customer;
        }

        public string GetCustomerPassword(int customerID)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);
            
            int customerIndex = customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            return customers[customerIndex].Password;
        }
        #endregion

        #region Find Methods
        public IEnumerable<Customer> FindCustomers(Predicate<Customer> predicate)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);

            return from customer in customers
                   where predicate(customer) && customer.Active
                   select customer;
        }
        #endregion
    }
}
