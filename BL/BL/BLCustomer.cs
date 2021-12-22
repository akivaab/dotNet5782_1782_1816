using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Customer-related functionality of the Business Layer.
    /// </summary>
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
                IEnumerable<DO.Package> dalPackagesToSend = DalObject.FindPackages(p => p.SenderID == customerID);
                IEnumerable<DO.Package> dalPackagesToReceive = DalObject.FindPackages(p => p.ReceiverID == customerID);

                //create collection of PackageForCustomers this customer is sending
                IEnumerable<PackageForCustomer> packagesToSend = from DO.Package dalPackage in dalPackagesToSend
                                                                 let receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID)
                                                                 let receiverForPackage = new CustomerForPackage(receiver.ID, receiver.Name)
                                                                 select new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage);

                //create collection of PackageForCustomers this customer is receiving
                IEnumerable<PackageForCustomer> packagesToReceive = from DO.Package dalPackage in dalPackagesToReceive
                                                                    let sender = DalObject.DisplayCustomer(dalPackage.SenderID)
                                                                    let senderForPackage = new CustomerForPackage(sender.ID, sender.Name)
                                                                    select new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage);
                
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
            IEnumerable<DO.Customer> dalCustomers = DalObject.DisplayCustomersList();
            IEnumerable<CustomerToList> customerToLists = from DO.Customer dalCustomer in dalCustomers
                                                          let numDeliveredPackagesSent = DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered != null).Count()
                                                          let numUndeliveredPackagesSent = DalObject.FindPackages(p => p.SenderID == dalCustomer.ID && p.Delivered == null).Count()
                                                          let numPackagesReceived = DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered != null).Count()
                                                          let numPackagesExpected = DalObject.FindPackages(p => p.ReceiverID == dalCustomer.ID && p.Delivered == null).Count()
                                                          select new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected);
            return customerToLists;
        }
    }
}
