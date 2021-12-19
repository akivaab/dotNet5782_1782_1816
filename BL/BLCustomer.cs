using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            try
            {
                DalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            }
            catch (DalApi.DO.IllegalArgumentException)
            {
                throw new IllegalArgumentException();
            }
            catch (DalApi.DO.NonUniqueIdException)
            {
                throw new NonUniqueIdException();
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
            catch (DalApi.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public Customer DisplayCustomer(int customerID)
        {
            try
            {
                DalApi.DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);

                //create two lists of PackageForCustomers
                List<DalApi.DO.Package> dalPackagesToSend = (List<DalApi.DO.Package>)DalObject.FindPackages(p => p.SenderID == customerID);
                List<DalApi.DO.Package> dalPackagesToReceive = (List<DalApi.DO.Package>)DalObject.FindPackages(p => p.ReceiverID == customerID);

                //create list of PackageForCustomers this customer is sending
                List<PackageForCustomer> packagesToSend = new();
                foreach (DalApi.DO.Package dalPackage in dalPackagesToSend)
                {
                    DalApi.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                    CustomerForPackage receiverForPackage = new(receiver.ID, receiver.Name);
                    packagesToSend.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage));
                }

                //create list of PackageForCustomers this customer is receiving
                List<PackageForCustomer> packagesToReceive = new();
                foreach (DalApi.DO.Package dalPackage in dalPackagesToReceive)
                {
                    DalApi.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                    CustomerForPackage senderForPackage = new(sender.ID, sender.Name);
                    packagesToReceive.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage));
                }

                Customer customer = new(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, new Location(dalCustomer.Latitude, dalCustomer.Longitude), packagesToSend, packagesToReceive);
                return customer;
            }
            catch (DalApi.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public List<CustomerToList> DisplayAllCustomers()
        {
            List<DalApi.DO.Customer> dalCustomers = new(DalObject.DisplayCustomersList());
            List<CustomerToList> customerToLists = new();
                
            foreach (DalApi.DO.Customer dalCustomer in dalCustomers)
            {
                int numDeliveredPackagesSent = ((List<DalApi.DO.Package>)DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered != null)).Count;
                int numUndeliveredPackagesSent = ((List<DalApi.DO.Package>)DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered == null)).Count;
                int numPackagesReceived = ((List<DalApi.DO.Package>)DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered != null)).Count;
                int numPackagesExpected = ((List<DalApi.DO.Package>)DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered == null)).Count;
                customerToLists.Add(new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected));
            }
            return customerToLists;
        }
    }
}
