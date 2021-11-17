using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            Customer customer = new(customerID, name, phone, location, new List<PackageForCustomer>(), new List<PackageForCustomer>());
            DalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            return customer;
        }
        public void UpdateCustomer(int customerID, string name = "", string phone = "")
        {
            List<IDAL.DO.Customer> dalCustomerList = (List<IDAL.DO.Customer>)DalObject.DisplayCustomersList();
            int customerIndex = dalCustomerList.FindIndex(c => c.ID == customerID);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            IDAL.DO.Customer customer = dalCustomerList[customerIndex];
            dalCustomerList.RemoveAt(customerIndex);
            if (name != "")
            {
                customer.Name = name;
            }
            if (phone != "")
            {
                customer.Phone = phone;
            }
            dalCustomerList.Insert(customerIndex, customer);
        }
        public Customer DisplayCustomer(int customerID)
        {
            IDAL.DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<IDAL.DO.Package> dalPackagesToSend = dalPackages.FindAll(p => p.SenderID == customerID);
            List<IDAL.DO.Package> dalPackagesToReceive = dalPackages.FindAll(p => p.ReceiverID == customerID);
            List<PackageForCustomer> packagesToSend = new();
            List<PackageForCustomer> packagesToReceive = new();
            foreach (IDAL.DO.Package dalPackage in dalPackagesToSend)
            {
                IDAL.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                CustomerForPackage receiverForPackage = new(receiver.ID, receiver.Name);
                packagesToSend.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage));
            }
            foreach (IDAL.DO.Package dalPackage in dalPackagesToReceive)
            {
                IDAL.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                CustomerForPackage senderForPackage = new(sender.ID, sender.Name);
                packagesToReceive.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage));
            }
            Customer customer = new(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, new Location(dalCustomer.Latitude, dalCustomer.Longitude), packagesToSend, packagesToReceive);
            return customer;
        }
        public List<CustomerToList> DisplayAllCustomers()
        {
            List<IDAL.DO.Customer> dalCustomers = new(DalObject.DisplayCustomersList());
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<CustomerToList> customerToLists = new();
            int numDeliveredPackagesSent = 0;
            int numUndeliveredPackagesSent = 0;
            int numPackagesReceived = 0;
            int numPackagesExpected = 0;
            foreach (IDAL.DO.Customer dalCustomer in dalCustomers)
            {
                numDeliveredPackagesSent = dalPackages.FindAll(p => p.SenderID == dalCustomer.ID && p.Delivered != DateTime.MinValue).Count;
                numUndeliveredPackagesSent = dalPackages.FindAll(p => p.SenderID == dalCustomer.ID && p.Collected != DateTime.MinValue && p.Delivered == DateTime.MinValue).Count;
                numPackagesReceived = dalPackages.FindAll(p => p.ReceiverID == dalCustomer.ID && p.Delivered != DateTime.MinValue).Count;
                numPackagesExpected = dalPackages.FindAll(p => p.ReceiverID == dalCustomer.ID && p.Delivered == DateTime.MinValue).Count;
                customerToLists.Add(new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected));
            }
            return customerToLists;
        }
    }
}
