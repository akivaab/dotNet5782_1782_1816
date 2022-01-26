using System;
using BO;

namespace ConsoleUI_BL
{
    /// <summary>
    /// Console program to test system from the business layer.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Instance of BL.
        /// </summary>
        static BlApi.IBL bl = BlApi.BlFactory.GetBl();

        /// <summary>
        /// Main method, runs the main menu.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
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
                        AddingOptions();
                        break;
                    case 2:
                        UpdatingOptions();
                        break;
                    case 3:
                        DisplayingOptions();
                        break;
                    case 4:
                        ListDisplayingOptions();
                        break;
                    case 5:
                        Console.WriteLine("Bye");
                        break;
                    default:
                        break;
                }
            } while (option != 5);
        }

        #region Adding
        /// <summary>
        /// All the options of adding entities to the system.
        /// </summary>
        public static void AddingOptions()
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
                Console.WriteLine(e.Message);
            }
            catch (NonUniqueIdException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (XMLFileLoadCreateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region Updating
        /// <summary>
        /// All the options of updating the entities of the system.
        /// </summary>
        public static void UpdatingOptions()
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
                Console.WriteLine(e.Message);
            }
            catch (UnableToChargeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnableToReleaseException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnableToAssignException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnableToCollectException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnableToDeliverException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (XMLFileLoadCreateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region Displaying
        /// <summary>
        /// All the options of displaying the entities of the system.
        /// </summary>
        public static void DisplayingOptions()
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
                        Console.WriteLine(bl.GetStation(stationID));
                        break;
                    case 2:
                        int droneID;
                        Console.Write("Input Drone ID: ");
                        int.TryParse(Console.ReadLine(), out droneID);
                        Console.WriteLine(bl.GetDrone(droneID));
                        break;
                    case 3:
                        int customerID;
                        Console.Write("Input Customer ID: ");
                        int.TryParse(Console.ReadLine(), out customerID);
                        Console.WriteLine(bl.GetCustomer(customerID));
                        break;
                    case 4:
                        int packageID;
                        Console.Write("Input Package ID: ");
                        int.TryParse(Console.ReadLine(), out packageID);
                        Console.WriteLine(bl.GetPackage(packageID));
                        break;
                    default:
                        break;
                }
            }
            catch (UndefinedObjectException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (XMLFileLoadCreateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region Displaying List
        /// <summary>
        /// All the options of displaying a list of the entities of the system.
        /// </summary>
        public static void ListDisplayingOptions()
        {
            try
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
                        foreach (StationToList station in bl.GetStationsList())
                        {
                            Console.WriteLine(station);
                        }
                        break;
                    case 2:
                        foreach (DroneToList drone in bl.GetDronesList())
                        {
                            Console.WriteLine(drone);
                        }
                        break;
                    case 3:
                        foreach (CustomerToList customer in bl.GetCustomersList())
                        {
                            Console.WriteLine(customer);
                        }
                        break;
                    case 4:
                        foreach (PackageToList package in bl.GetPackagesList())
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
            catch (XMLFileLoadCreateException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
