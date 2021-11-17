using System;
using IBL.BO;

namespace ConsoleUI_BL
{
    class Program
    {
        static void Main(string[] args)
        {
            IBL.IBL bl = new IBL.BL();
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
                        AddingOptions(bl);
                        break;
                    case 2:
                        UpdatingOptions(bl);
                        break;
                    case 3:
                        DisplayingOptions(bl);
                        break;
                    case 4:
                        ListDisplayingOptions(bl);
                        break;
                    case 5:
                        Console.WriteLine("Bye");
                        break;
                    default:
                        break;
                }
            } while (option != 5);
        }

        public static void AddingOptions(IBL.IBL bl)
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
                        Console.Write("Input location latitude: ");
                        double.TryParse(Console.ReadLine(), out stationLatitude);
                        Console.Write("Input location longitude: ");
                        double.TryParse(Console.ReadLine(), out stationLongitude);
                        Location stationLocation = new(stationLatitude, stationLongitude);
                        bl.AddStation(stationID, stationName, stationLocation, numChargeSlots);
                        break;
                    case 2:
                        int droneID, stationFirstChargingID;
                        string model;
                        Enums.WeightCategories maxWeight;
                        Console.Write("Input drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.Write("Input model: ");
                        model = Console.ReadLine();
                        Console.Write("Input maximum weight capacity (1: light, 2: medium , 3: heavy): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out maxWeight);
                        Console.Write("Input station ID: ");
                        int.TryParse(Console.ReadLine(), out stationFirstChargingID);
                        bl.AddDrone(droneID, model, maxWeight, stationFirstChargingID);
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
                        Console.Write("Input location latitude: ");
                        double.TryParse(Console.ReadLine(), out customerLatitude);
                        Console.Write("Input location longitude: ");
                        double.TryParse(Console.ReadLine(), out customerLongitude);
                        Location customerLocation = new(customerLatitude, customerLongitude);
                        bl.AddCustomer(customerID, customerName, phone, customerLocation);
                        break;
                    case 4:
                        int senderID, receiverID;
                        Enums.WeightCategories weight;
                        Enums.Priorities priority;
                        Console.Write("Input sender ID: ");
                        int.TryParse(Console.ReadLine(), out senderID);
                        Console.Write("Input receiver ID: ");
                        int.TryParse(Console.ReadLine(), out receiverID);
                        Console.Write("Input package weight (1: light, 2: medium , 3: heavy): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out weight);
                        Console.Write("Input package priority (0: regular, 1: fast, 2: emergency): ");
                        Enums.WeightCategories.TryParse(Console.ReadLine(), out priority);
                        bl.AddPackage(senderID, receiverID, weight, priority);
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

        public static void UpdatingOptions(IBL.IBL bl)
        {
            Console.WriteLine("Updatinging Options:");
            Console.WriteLine("1. Update drone model");
            Console.WriteLine("2. Update station information");
            Console.WriteLine("3. Update customer information");
            Console.WriteLine("4. Send drone to charge");
            Console.WriteLine("5. Release drone from charging station");
            Console.WriteLine("6. Assign package to drone");
            Console.WriteLine("7. Collect package by drone");
            Console.WriteLine("8. Deliver package to customer");
            Console.Write("Choose an updating option: ");
            int updatingOption;
            int.TryParse(Console.ReadLine(), out updatingOption);
            try
            {
                switch (updatingOption)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        int droneToChargeID;
                        Console.Write("Input drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneToChargeID);
                        bl.SendDroneToCharge(droneToChargeID);
                        break;
                    case 5:
                        int droneToReleaseID;
                        double chargingTime;
                        Console.Write("Input drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneToReleaseID);
                        Console.Write("Input charging time: ");
                        double.TryParse(Console.ReadLine(), out chargingTime);
                        bl.ReleaseFromCharge(droneToReleaseID, chargingTime);
                        break;
                    case 6:
                        int assignDroneID;
                        Console.Write("Input drone ID: ");
                        int.TryParse(Console.ReadLine(), out assignDroneID);
                        bl.AssignPackage(assignDroneID);
                        break;
                    case 7:
                        int collectPackageID, collectDroneID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out collectPackageID);
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out collectDroneID);
                        bl.CollectPackage(collectPackageID, collectDroneID);
                        break;
                    case 8:
                        int deliveryPackageID, deliveryDroneID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out deliveryPackageID);
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out deliveryDroneID);
                        bl.DeliverPackage(deliveryPackageID, deliveryDroneID);
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
    }
}
