﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BO;

namespace BL
{
    /// <summary>
    /// Customer-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            if (location.Latitude < -1 || location.Latitude > 1 || location.Longitude < 0 || location.Longitude > 2) //limited coordinate field
            {
                throw new IllegalArgumentException("The given latitude and/or longitude is out of our coordinate field range.");
            }
            if (phone.Length != 9)
            {
                throw new IllegalArgumentException("The phone number is invalid.");
            }

            try
            {
                lock (dal)
                {
                    dal.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
                }
                Customer customer = new(customerID, name, phone, location, new List<PackageForCustomer>(), new List<PackageForCustomer>());
                return customer;
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(int customerID, string name = "", string phone = "")
        {
            if (phone != "" && phone.Length != 9)
            {
                throw new IllegalArgumentException("The phone number is invalid.");
            }

            try
            {
                lock (dal)
                {
                    if (name != "")
                    {
                        dal.UpdateCustomerName(customerID, name);
                    }
                    if (phone != "")
                    {
                        dal.UpdateCustomerPhone(customerID, phone);
                    }
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomerPassword(int customerID, string password)
        {
            if (password.Length < 6)
            {
                throw new IllegalArgumentException("The password is not secure.");
            }

            try
            {
                lock (dal)
                {
                    dal.UpdateCustomerPassword(customerID, password);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveCustomer(int customerID)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Package> packages = dal.FindPackages(p => p.SenderID == customerID || p.ReceiverID == customerID);
                    if (packages.Count() > 0)
                    {
                        throw new UnableToRemoveException("The customer is in the midst of a transaction.");
                    }

                    dal.RemoveCustomer(customerID);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int customerID)
        {
            try
            {
                lock (dal)
                {
                    DO.Customer dalCustomer = dal.GetCustomer(customerID);

                    //create two lists of PackageForCustomers
                    IEnumerable<DO.Package> dalPackagesToSend = dal.FindPackages(p => p.SenderID == customerID);
                    IEnumerable<DO.Package> dalPackagesToReceive = dal.FindPackages(p => p.ReceiverID == customerID);

                    //create collection of PackageForCustomers this customer is sending
                    IEnumerable<PackageForCustomer> packagesToSend = from DO.Package dalPackage in dalPackagesToSend
                                                                     let receiver = dal.GetCustomer(dalPackage.ReceiverID)
                                                                     let receiverForPackage = new CustomerForPackage(receiver.ID, receiver.Name)
                                                                     select new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage);

                    //create collection of PackageForCustomers this customer is receiving
                    IEnumerable<PackageForCustomer> packagesToReceive = from DO.Package dalPackage in dalPackagesToReceive
                                                                        let sender = dal.GetCustomer(dalPackage.SenderID)
                                                                        let senderForPackage = new CustomerForPackage(sender.ID, sender.Name)
                                                                        select new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage);

                    Customer customer = new(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, new Location(dalCustomer.Latitude, dalCustomer.Longitude), packagesToSend, packagesToReceive);
                    return customer;
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetCustomerPassword(int customerID)
        {
            try
            {
                lock (dal)
                {
                    return dal.GetCustomerPassword(customerID);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerToList> GetCustomersList()
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Customer> dalCustomers = dal.GetCustomersList();
                    IEnumerable<CustomerToList> customerToLists = from DO.Customer dalCustomer in dalCustomers
                                                                  let numDeliveredPackagesSent = dal.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered != null).Count()
                                                                  let numUndeliveredPackagesSent = dal.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered == null).Count()
                                                                  let numPackagesReceived = dal.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered != null).Count()
                                                                  let numPackagesExpected = dal.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered == null).Count()
                                                                  select new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected);
                    return customerToLists.OrderBy(c => c.ID);
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<CustomerToList> FindCustomers(Predicate<DO.Customer> predicate)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Customer> dalCustomers = dal.FindCustomers(predicate);
                    IEnumerable<CustomerToList> customersToLists = from DO.Customer dalCustomer in dalCustomers
                                                                   let numDeliveredPackagesSent = dal.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered != null).Count()
                                                                   let numUndeliveredPackagesSent = dal.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered == null).Count()
                                                                   let numPackagesReceived = dal.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered != null).Count()
                                                                   let numPackagesExpected = dal.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered == null).Count()
                                                                   select new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected);
                    return customersToLists;
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion
    }
}
