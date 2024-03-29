﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalObject
{
    /// <summary>
    /// Customer-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (DataSource.customers.Exists(customer => customer.ID == id && customer.Active))
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
            DataSource.customers.Add(customer);
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerName(int customerID, string name)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }
            
            Customer customer = DataSource.customers[customerIndex];
            customer.Name = name;
            DataSource.customers[customerIndex] = customer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerPhone(int customerID, string phone)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }
            
            Customer customer = DataSource.customers[customerIndex];
            customer.Phone = phone;
            DataSource.customers[customerIndex] = customer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerPassword(int customerID, string password)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);

            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            Customer customer = DataSource.customers[customerIndex];
            customer.Password = password;
            DataSource.customers[customerIndex] = customer;
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveCustomer(int customerID)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID");
            }

            Customer customer = DataSource.customers[customerIndex];
            customer.Active = false;
            DataSource.customers[customerIndex] = customer;
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int customerID)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);
            
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }
            
            return DataSource.customers[customerIndex];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetCustomersList()
        {
            return from customer in DataSource.customers
                   where customer.Active
                   select customer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetCustomerPassword(int customerID)
        {
            int customerIndex = DataSource.customers.FindIndex(customer => customer.ID == customerID && customer.Active);

            if (customerIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            return DataSource.customers[customerIndex].Password;
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> FindCustomers(Predicate<Customer> predicate)
        {
            return from customer in DataSource.customers
                   where predicate(customer) && customer.Active
                   select customer;
        }
        #endregion
    }
}
