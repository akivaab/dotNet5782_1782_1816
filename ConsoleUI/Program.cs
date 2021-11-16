using System;
using IDAL.DO;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            IDAL.IDal dal = new DalObject.DalObject();
            int option;
            do
            {
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Adding options");
                Console.WriteLine("2. Updating options");
                Console.WriteLine("3. Display options");
                Console.WriteLine("4. List Display options");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");
                int.TryParse(Console.ReadLine(), out option);
                switch (option)
                {
                    case 1:
                        AddingOptions(dal);
                        break;
                    case 2:
                        UpdatingOptions(dal);
                        break;
                    case 3:
                        DisplayingOptions(dal);
                        break;
                    case 4:
                        ListDisplayingOptions(dal);
                        break;
                    case 5:
                        Console.WriteLine("Bye");
                        break;
                    default:
                        break;
                }
            } while (option != 5);
        }

        public static void AddingOptions(IDAL.IDal dal)
        {
            Console.WriteLine("Adding Options:");
            Console.WriteLine("1. Add new base station");
            Console.WriteLine("2. Add new drone");
            Console.WriteLine("3. Add new customer");
            Console.WriteLine("4. Add new package");
            Console.Write("Choose an adding option: ");
            int addingOption;
            int.TryParse(Console.ReadLine(), out addingOption);
            try
            {
                switch (addingOption)
                {
                    case 1:
                        int stationID, stationName, numChargeSlots;
                        double stationLatitude, stationLongitude;
                        Console.Write("Input ID: ");
                        int.TryParse(Console.ReadLine(), out stationID);
                        Console.Write("Input name: ");
                        int.TryParse(Console.ReadLine(), out stationName);
                        Console.Write("Input number of free charge slots: ");
                        int.TryParse(Console.ReadLine(), out numChargeSlots);
                        Console.Write("Input latitude: ");
                        double.TryParse(Console.ReadLine(), out stationLatitude);
                        Console.Write("Input longitude: ");
                        double.TryParse(Console.ReadLine(), out stationLongitude);
                        dal.AddStation(stationID, stationName, numChargeSlots, stationLatitude, stationLongitude);
                        break;
                    case 2:
                        int droneID;
                        string model;
                        Enums.WeightCategories maxWeight;
                        Console.Write("Input ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.Write("Input model: ");
                        model = Console.ReadLine();
                        Console.Write("Input maximum weight capacity (0: light, 1: medium , 2: heavy): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out maxWeight);
                        dal.AddDrone(droneID, model, maxWeight);
                        break;
                    case 3:
                        int customerID;
                        string customerName, phone;
                        double customerLatitude, customerLongitude;
                        Console.Write("Input ID: ");
                        int.TryParse(Console.ReadLine(), out customerID);
                        Console.Write("Input name: ");
                        customerName = Console.ReadLine();
                        Console.Write("Input phone number: ");
                        phone = Console.ReadLine();
                        Console.Write("Input latitude: ");
                        double.TryParse(Console.ReadLine(), out customerLatitude);
                        Console.Write("Input longitude: ");
                        double.TryParse(Console.ReadLine(), out customerLongitude);
                        dal.AddCustomer(customerID, customerName, phone, customerLatitude, customerLongitude);
                        break;
                    case 4:
                        int senderID, receiverID;
                        Enums.WeightCategories weight;
                        Enums.Priorities priority;
                        Console.Write("Input sender ID: ");
                        int.TryParse(Console.ReadLine(), out senderID);
                        Console.Write("Input receiver ID: ");
                        int.TryParse(Console.ReadLine(), out receiverID);
                        Console.Write("Input package weight (0: light, 1: medium , 2: heavy): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out weight);
                        Console.Write("Input package priority (0: regular, 1: fast, 2: emergency): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out priority);
                        dal.AddPackage(senderID, receiverID, weight, priority);
                        break;
                    default:
                        break;
                }
            }
            catch (IllegalArgumentException e)
            {
                Console.WriteLine($"{e}:\nOne of the arguments you entered is illegal.");
            }
            catch (NonUniqueIdException e)
            {
                Console.WriteLine($"{e}:\nAn object of this type with this ID already exists.");
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine($"{e}:\nAn object you referenced does not exist.");
            }
        }

        public static void UpdatingOptions(IDAL.IDal dal)
        {
            Console.WriteLine("Updatinging Options:");
            Console.WriteLine("1. Assign package to drone");
            Console.WriteLine("2. Collect package by drone");
            Console.WriteLine("3. Deliver package to customer");
            Console.WriteLine("4. Send drone to charge");
            Console.WriteLine("5. Release drone from charging station");
            Console.Write("Choose an updating option: ");
            int updatingOption;
            int.TryParse(Console.ReadLine(), out updatingOption);
            try
            {
                switch (updatingOption)
                {
                    case 1:
                        int assignPackageID, assignDroneID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out assignPackageID);
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out assignDroneID);
                        dal.AssignPackage(assignPackageID, assignDroneID);
                        break;
                    case 2:
                        int collectPackageID, collectDroneID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out collectPackageID);
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out collectDroneID);
                        dal.CollectPackage(collectPackageID, collectDroneID);
                        break;
                    case 3:
                        int deliveryPackageID, deliveryDroneID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out deliveryPackageID);
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out deliveryDroneID);
                        dal.DeliverPackage(deliveryPackageID, deliveryDroneID);
                        break;
                    case 4:
                        int chargeDroneID, chargeStationID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out chargeDroneID);
                        Console.Write("Input Station ID: ");
                        int.TryParse(Console.ReadLine(), out chargeStationID);
                        dal.ChargeDrone(chargeDroneID, chargeStationID);
                        break;
                    case 5:
                        int doneChargeDroneID, doneChargeStationID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out doneChargeDroneID);
                        Console.Write("Input Station ID: ");
                        int.TryParse(Console.ReadLine(), out doneChargeStationID);
                        dal.ReleaseDroneFromCharging(doneChargeDroneID, doneChargeStationID);
                        break;
                    default:
                        break;
                }
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine($"{e}:\nThe object you wish to update is not defined.");
            }
        }

        public static void DisplayingOptions(IDAL.IDal dal)
        {
            Console.WriteLine("Displaying Options:");
            Console.WriteLine("1. Display station");
            Console.WriteLine("2. Display drone");
            Console.WriteLine("3. Display customer");
            Console.WriteLine("4. Display package");
            Console.Write("Choose a displaying option: ");
            int displayingOption;
            int.TryParse(Console.ReadLine(), out displayingOption);
            try
            {
                switch (displayingOption)
                {
                    case 1:
                        int stationID;
                        Console.Write("Input Station ID: ");
                        int.TryParse(Console.ReadLine(), out stationID);
                        Console.WriteLine(dal.DisplayStation(stationID));
                        break;
                    case 2:
                        int droneID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.WriteLine(dal.DisplayDrone(droneID));
                        break;
                    case 3:
                        int customerID;
                        Console.Write("Input Customer ID: ");
                        int.TryParse(Console.ReadLine(), out customerID);
                        Console.WriteLine(dal.DisplayCustomer(customerID));
                        break;
                    case 4:
                        int packageID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out packageID);
                        Console.WriteLine(dal.DisplayPackage(packageID));
                        break;
                    default:
                        break;
                }
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine($"{e}:\nThe object you wish to display is not defined.");
            }
        }

        public static void ListDisplayingOptions(IDAL.IDal dal)
        {
            Console.WriteLine("List Displaying Options:");
            Console.WriteLine("1. Display all stations");
            Console.WriteLine("2. Display all drones");
            Console.WriteLine("3. Display all customers");
            Console.WriteLine("4. Display all packages");
            Console.WriteLine("5. Display all unassigned packages");
            Console.WriteLine("6. Display all stations with unoccupied charging slots");
            Console.Write("Choose a list displaying option: ");
            int listDisplayingOption;
            int.TryParse(Console.ReadLine(), out listDisplayingOption);
            switch (listDisplayingOption)
            {
                case 1:
                    foreach (Station station in dal.DisplayStationsList())
                    {
                        Console.WriteLine(station);
                    }
                    break;
                case 2:
                    foreach (Drone drone in dal.DisplayDronesList())
                    {
                        Console.WriteLine(drone);
                    }
                    break;
                case 3:
                    foreach (Customer customer in dal.DisplayCustomersList())
                    {
                        Console.WriteLine(customer);
                    }
                    break;
                case 4:
                    foreach (Package package in dal.DisplayPackagesList())
                    {
                        Console.WriteLine(package);
                    }
                    break;
                case 5:
                    foreach (Package package in dal.DisplayUnassignedPackagesList())
                    {
                        Console.WriteLine(package);
                    }
                    break;
                case 6:
                    foreach (Station station in dal.DisplayFreeStationsList())
                    {
                        Console.WriteLine(station);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
