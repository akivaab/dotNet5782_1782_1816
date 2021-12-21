using System;
using System.Collections.Generic;
using BO;

namespace BL
{
    partial class BL : BlApi.IBL
    {
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            try
            {
                DalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            }
            catch (DO.IllegalArgumentException e)
            {
                throw new IllegalArgumentException(e.Message, e);
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            Customer customer = new(customerID, name, phone, location, new List<PackageForCustomer>(), new List<PackageForCustomer>());
            return customer;
        }
        public void UpdateCustomer(int customerID, string name = "", string phone = "")
        {
            try
            {
                if (name != "")
                {
                    DalObject.UpdateCustomerName(customerID, name);
                }
                if (phone != "")
                {
                    DalObject.UpdateCustomerPhone(customerID, phone);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        public Customer DisplayCustomer(int customerID)
        {
            try
            {
                DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);

                //create two lists of PackageForCustomers
                List<DO.Package> dalPackagesToSend = (List<DO.Package>)DalObject.FindPackages(p => p.SenderID == customerID);
                List<DO.Package> dalPackagesToReceive = (List<DO.Package>)DalObject.FindPackages(p => p.ReceiverID == customerID);

                //create list of PackageForCustomers this customer is sending
                List<PackageForCustomer> packagesToSend = new();
                foreach (DO.Package dalPackage in dalPackagesToSend)
                {
                    DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                    CustomerForPackage receiverForPackage = new(receiver.ID, receiver.Name);
                    packagesToSend.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage));
                }

                //create list of PackageForCustomers this customer is receiving
                List<PackageForCustomer> packagesToReceive = new();
                foreach (DO.Package dalPackage in dalPackagesToReceive)
                {
                    DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                    CustomerForPackage senderForPackage = new(sender.ID, sender.Name);
                    packagesToReceive.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage));
                }

                Customer customer = new(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, new Location(dalCustomer.Latitude, dalCustomer.Longitude), packagesToSend, packagesToReceive);
                return customer;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        public IEnumerable<CustomerToList> DisplayAllCustomers()
        {
            List<DO.Customer> dalCustomers = new(DalObject.DisplayCustomersList());
            List<CustomerToList> customerToLists = new();
                
            foreach (DO.Customer dalCustomer in dalCustomers)
            {
                int numDeliveredPackagesSent = ((List<DO.Package>)DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered != null)).Count;
                int numUndeliveredPackagesSent = ((List<DO.Package>)DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered == null)).Count;
                int numPackagesReceived = ((List<DO.Package>)DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered != null)).Count;
                int numPackagesExpected = ((List<DO.Package>)DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered == null)).Count;
                customerToLists.Add(new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected));
            }
            return customerToLists;
        }
    }
}
