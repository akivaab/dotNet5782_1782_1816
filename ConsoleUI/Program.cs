using System;
using DalObject;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            DalObject.DalObject dal = new DalObject.DalObject();
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
                        Console.WriteLine("Bye:");
                        break;
                    default:
                        break;
                }
            } while (option != 5);
        }

        public static void AddingOptions(DalObject.DalObject dal)
        {
            Console.WriteLine("Adding Options:");
            Console.WriteLine("1. Add new base station");
            Console.WriteLine("2. Add new drone");
            Console.WriteLine("3. Add new customer");
            Console.WriteLine("4. Add new package");
            Console.Write("Choose an adding option: ");
            int addingOption;
            int.TryParse(Console.ReadLine(), out addingOption);
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
                    IDAL.DO.Enums.WeightCategories maxWeight;
                    IDAL.DO.Enums.DroneStatuses status;
                    double batteryLevel;
                    Console.Write("Input ID: ");
                    int.TryParse(Console.ReadLine(), out droneID);
                    Console.Write("Input model: ");
                    model = Console.ReadLine();
                    Console.Write("Input maximum weight capacity (0: light, 1: medium , 2: heavy): ");
                    IDAL.DO.Enums.WeightCategories.TryParse(Console.ReadLine(), out maxWeight);
                    Console.Write("Input status (0: free, 1: maintenance): ");
                    IDAL.DO.Enums.DroneStatuses.TryParse(Console.ReadLine(), out status);
                    Console.Write("Input battery level (0.0-100.0): ");
                    double.TryParse(Console.ReadLine(), out batteryLevel);
                    dal.AddDrone(droneID, model, maxWeight, status, batteryLevel);
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
                    IDAL.DO.Enums.WeightCategories weight;
                    IDAL.DO.Enums.Priorities priority;
                    Console.Write("Input sender ID: ");
                    int.TryParse(Console.ReadLine(), out senderID);
                    Console.Write("Input receiver ID: ");
                    int.TryParse(Console.ReadLine(), out receiverID);
                    Console.Write("Input package weight (0: light, 1: medium , 2: heavy): ");
                    IDAL.DO.Enums.WeightCategories.TryParse(Console.ReadLine(), out weight);
                    Console.Write("Input package priority (0: regular, 1: fast, 2: emergency): ");
                    IDAL.DO.Enums.WeightCategories.TryParse(Console.ReadLine(), out priority);
                    dal.AddPackage(senderID, receiverID, weight, priority);
                    break;
                default:
                    break;
            }
        }

        public static void UpdatingOptions(DalObject.DalObject dal)
        {
            Console.WriteLine("Updatinging Options:");
            Console.WriteLine("1. Assign package to drone");
            Console.WriteLine("2. Collect package by drone");
            Console.WriteLine("3. Deliver package to customer");
            Console.WriteLine("4. Send drone to charge");
            Console.WriteLine("5. Release drone from chargine station");
            Console.Write("Choose an updating option: ");
            int updatingOption;
            int.TryParse(Console.ReadLine(), out updatingOption);
            switch(updatingOption)
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

        public static void DisplayingOptions(DalObject.DalObject dal)
        {
            Console.WriteLine("Displaying Options:");
            Console.WriteLine("1. Display station");
            Console.WriteLine("2. Display drone");
            Console.WriteLine("3. Display customer");
            Console.WriteLine("4. Display package");
            Console.Write("Choose a displaying option: ");
            int displayingOption;
            int.TryParse(Console.ReadLine(), out displayingOption);
            switch(displayingOption)
            {
                case 1:
                    int stationID;
                    Console.Write("Input Station ID: ");
                    int.TryParse(Console.ReadLine(), out stationID);
                    dal.DisplayStation(stationID);
                    break;
                case 2:
                    int droneID;
                    Console.Write("Input Drone ID: ");
                    int.TryParse(Console.ReadLine(), out droneID);
                    dal.DisplayDrone(droneID);
                    break;
                case 3:
                    int customerID;
                    Console.Write("Input Customer ID: ");
                    int.TryParse(Console.ReadLine(), out customerID);
                    dal.DisplayCustomer(customerID);
                    break;
                case 4:
                    int packageID;
                    Console.Write("Input Package ID: ");
                    int.TryParse(Console.ReadLine(), out packageID);
                    dal.DisplayPackage(packageID);
                    break;
                default:
                    break;
            }
        }

        public static void ListDisplayingOptions(DalObject.DalObject dal)
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
            switch(listDisplayingOption)
            {
                case 1:
                    dal.DisplayStationsList();
                    break;
                case 2:
                    dal.DisplayDronesList();
                    break;
                case 3:
                    dal.DisplayCustomersList();
                    break;
                case 4:
                    dal.DisplayPackagesList();
                    break;
                case 5:
                    dal.DisplayUnassignedPackagesList();
                    break;
                case 6:
                    dal.DisplayUnoccupiedStationsList();
                    break;
                default:
                    break;
            }
        }
    }
}
