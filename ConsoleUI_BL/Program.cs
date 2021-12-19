using System;
using BlApi.BO;

namespace ConsoleUI_BL
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BlApi.IBL bl = new BlApi.BL();
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
            catch (UndefinedObjectException e)
            {
                Console.WriteLine($"{e}:\nInitialization failed. Try restarting the program.");
            }
        }

        public static void AddingOptions(BlApi.IBL bl)
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
                        Console.Write("Input location latitude (limit from -1 to 1): ");
                        double.TryParse(Console.ReadLine(), out stationLatitude);
                        Console.Write("Input location longitude (limit from 0 to 2): ");
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
                        Console.Write("Input location latitude (limit from -1 to 1): ");
                        double.TryParse(Console.ReadLine(), out customerLatitude);
                        Console.Write("Input location longitude (limit from 0 to 2): ");
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

        public static void UpdatingOptions(BlApi.IBL bl)
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
                        int droneID;
                        string model;
                        Console.WriteLine("Input drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.WriteLine("Input drone model: ");
                        model = Console.ReadLine();
                        bl.UpdateDroneModel(droneID, model);
                        break;
                    case 2:
                        int stationID, stationName, numChargingSlots;
                        Console.WriteLine("Input station ID: ");
                        int.TryParse(Console.ReadLine(), out stationID);
                        Console.WriteLine("Input station name (optional - press enter to skip): ");
                        var readStationName = Console.ReadLine();
                        Console.WriteLine("Input number of charging slots: (optional - press enter to skip): ");
                        var readNumChargeSlots = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(readStationName) && !string.IsNullOrWhiteSpace(readNumChargeSlots))
                        {
                            int.TryParse(readStationName, out stationName);
                            int.TryParse(readNumChargeSlots, out numChargingSlots);
                            bl.UpdateStation(stationID, stationName, numChargingSlots);
                        }
                        else if (!string.IsNullOrWhiteSpace(readStationName))
                        {
                            int.TryParse(readStationName, out stationName);
                            bl.UpdateStation(stationID, stationName);
                        }
                        else if (!string.IsNullOrWhiteSpace(readNumChargeSlots))
                        {
                            int.TryParse(readNumChargeSlots, out numChargingSlots);
                            bl.UpdateStation(stationID, -1, numChargingSlots);
                        }
                        else
                        {
                            Console.WriteLine("You did not enter any information to update.");
                        }
                        break;
                    case 3:
                        int customerID;
                        string customerName, phone;
                        Console.WriteLine("Input customer ID: ");
                        int.TryParse(Console.ReadLine(), out customerID);
                        Console.WriteLine("Input customer name (optional - press enter to skip): ");
                        customerName = Console.ReadLine();
                        Console.WriteLine("Input customer phone number: (optional - press enter to skip): ");
                        phone = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(customerName) && !string.IsNullOrWhiteSpace(phone))
                        {
                            bl.UpdateCustomer(customerID, customerName, phone);
                        }
                        else if (!string.IsNullOrWhiteSpace(customerName))
                        {
                            bl.UpdateCustomer(customerID, customerName);
                        }
                        else if (!string.IsNullOrWhiteSpace(phone))
                        {
                            bl.UpdateCustomer(customerID, "", phone);
                        }
                        else
                        {
                            Console.WriteLine("You did not enter any information to update.");
                        }
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
                        int collectDroneID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out collectDroneID);
                        bl.CollectPackage(collectDroneID);
                        break;
                    case 8:
                        int deliveryDroneID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out deliveryDroneID);
                        bl.DeliverPackage(deliveryDroneID);
                        break;
                    default:
                        break;
                }
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine($"{e}:\nThe object you wish to update is not defined.");
            }
            catch (UnableToChargeException e)
            {
                Console.WriteLine($"{e}:\nThe drone is unable to be sent to a station to charge.");
            }
            catch (UnableToReleaseException e)
            {
                Console.WriteLine($"{e}:\nThe drone is unable to be released from a charging slot.");
            }
            catch (UnableToAssignException e)
            {
                Console.WriteLine($"{e}:\nThis drone cannot be assigned a package.");
            }
            catch (UnableToCollectException e)
            {
                Console.WriteLine($"{e}:\nThis drone is unable to collect a package.");
            }
            catch (UnableToDeliverException e)
            {
                Console.WriteLine($"{e}:\nThis drone is unable to deliver a package.");
            }
        }

        public static void DisplayingOptions(BlApi.IBL bl)
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
                        Console.WriteLine(bl.DisplayStation(stationID));
                        break;
                    case 2:
                        int droneID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.WriteLine(bl.DisplayDrone(droneID));
                        break;
                    case 3:
                        int customerID;
                        Console.Write("Input Customer ID: ");
                        int.TryParse(Console.ReadLine(), out customerID);
                        Console.WriteLine(bl.DisplayCustomer(customerID));
                        break;
                    case 4:
                        int packageID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out packageID);
                        Console.WriteLine(bl.DisplayPackage(packageID));
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

        public static void ListDisplayingOptions(BlApi.IBL bl)
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
                    foreach (StationToList station in bl.DisplayAllStations())
                    {
                        Console.WriteLine(station);
                    }
                    break;
                case 2:
                    foreach (DroneToList drone in bl.DisplayAllDrones())
                    {
                        Console.WriteLine(drone);
                    }
                    break;
                case 3:
                    foreach (CustomerToList customer in bl.DisplayAllCustomers())
                    {
                        Console.WriteLine(customer);
                    }
                    break;
                case 4:
                    foreach (PackageToList package in bl.DisplayAllPackages())
                    {
                        Console.WriteLine(package);
                    }
                    break;
                case 5:
                    foreach (PackageToList package in bl.FindPackages(p => p.DroneID == null))
                    {
                        Console.WriteLine(package);
                    }
                    break;
                case 6:
                    foreach (StationToList station in bl.FindStations(s => s.AvailableChargeSlots > 0))
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
