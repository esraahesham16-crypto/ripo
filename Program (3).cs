using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelingServices
{
    // ===== Base Class =====
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Person(string name, string email, string phone, string username, string password)
        {
            Name = name;
            Email = email;
            Phone = phone;
            Username = username;
            Password = password;
        }

        public bool Login(string inputUser, string inputPass)
        {

            return Username == inputUser && Password == inputPass;
        }
    }

    public class Customer : Person
    {
        public List<Booking> Bookings { get; set; } = new();
        public Customer(string name, string email, string phone, string username, string password)
            : base(name, email, phone, username, password) { }
    }

    public class HotelOwner : Person
    {
        public List<Hotel> Hotels { get; set; } = new();
        public HotelOwner(string name, string email, string phone, string username, string password)
            : base(name, email, phone, username, password) { }

        public void UpdateHotel(int serviceId, string newName, string newLocation, int newRating)
        {
            var hotel = Hotels.FirstOrDefault(h => h.ServiceId == serviceId);
            if (hotel != null)
            {
                hotel.Name = newName;
                hotel.Location = newLocation;
                hotel.Rating = newRating;
            }
        }
    }

    public class TaxiDriver : Person
    {
        public bool IsAvailable { get; set; } = true;
        public string CarDetails { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public TaxiDriver(string name, string email, string phone, string username, string password, string carDetails)
            : base(name, email, phone, username, password) { CarDetails = carDetails; }
    }

    public class AirlineAdmin : Person
    {
        public List<Flight> Flights { get; set; } = new();
        public AirlineAdmin(string name, string email, string phone, string username, string password)
            : base(name, email, phone, username, password) { }

        public void UpdateFlight(int serviceId, string newName, string newOrigin, string newDestination)
        {
            var flight = Flights.FirstOrDefault(f => f.ServiceId == serviceId);
            if (flight != null)
            {
                flight.Name = newName;
                flight.Origin = newOrigin;
                flight.Destination = newDestination;
            }
        }

        public void DeleteFlight(int serviceId)
        {
            var flight = Flights.FirstOrDefault(f => f.ServiceId == serviceId);
            if (flight != null)
            {
                Flights.Remove(flight);
            }
        }
    }

    public class SystemAdmin : Person
    {
        public SystemAdmin(string name, string email, string phone, string username, string password)
            : base(name, email, phone, username, password) { }
    }

    // ===== Services =====
    public abstract class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public abstract bool CheckAvailability();
    }

    public class Room
    {
        public string RoomID { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        public void BookRoom() => IsAvailable = false;
        public void FreeRoom() => IsAvailable = true;
    }

    public class Hotel : Service
    {
        public string Location { get; set; }
        public int Rating { get; set; }
        public List<Room> Rooms { get; set; } = new();

        public override bool CheckAvailability()
        {
            return Rooms.Any(r => r.IsAvailable);
        }

        public void ShowAvailableRooms()
        {
            Console.WriteLine($"Available rooms in {Name}:");
            foreach (var room in Rooms.Where(r => r.IsAvailable))
            {
                Console.WriteLine($"Room {room.RoomID} - {room.RoomType} - {room.Price}$");
            }
        }
    }

    public class Flight : Service
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int AvailableSeats { get; set; }

        public override bool CheckAvailability()
        {
            return AvailableSeats > 0;
        }
    }

    public class Taxi : Service
    {
        public string DriverName { get; set; }
        public string CarDetails { get; set; }
        public bool IsAvailable { get; set; } = true;

        public override bool CheckAvailability()
        {
            return IsAvailable;
        }
    }

    // ===== Bookings =====
    // abstract booking class
    public abstract class Booking
    {
        public string BookingID { get; set; }
        public Customer Customer { get; set; }
        public string Status { get; set; } = "Pending";

        public abstract void ShowBookingDetails();
        public virtual void ConfirmBooking() => Status = "Confirmed";
        public virtual void CancelBooking() => Status = "Cancelled";
    }

    // inhritance classes from booking 

    public class HotelBooking : Booking
    {
        public Room Room { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public HotelBooking(string id, Customer c, Room r, DateTime checkIn, DateTime checkOut)
        {
            BookingID = id;
            Customer = c;
            Room = r;
            CheckIn = checkIn;
            CheckOut = checkOut;
        }

        public override void ShowBookingDetails()
        {
            Console.WriteLine($"HotelBooking [{BookingID}] Room {Room.RoomID} ({Room.RoomType}) - {Status}");
        }
    }

    public class FlightBooking : Booking
    {
        public Flight Flight { get; set; }

        public FlightBooking(string id, Customer c, Flight f)
        {
            BookingID = id;
            Customer = c;
            Flight = f;
        }

        public override void ShowBookingDetails()
        {
            Console.WriteLine($"FlightBooking [{BookingID}] {Flight.Origin} → {Flight.Destination} - {Status}");
        }
    }

    public class TaxiBooking : Booking
    {
        public Taxi Taxi { get; set; }
        public string Pickup { get; set; }
        public string Drop { get; set; }

        public TaxiBooking(string id, Customer c, Taxi taxi, string pickup, string drop)
        {
            BookingID = id;
            Customer = c;
            Taxi = taxi;
            Pickup = pickup;
            Drop = drop;
        }

        public override void ShowBookingDetails()
        {
            Console.WriteLine($"TaxiBooking [{BookingID}] {Taxi.DriverName} ({Taxi.CarDetails}) - {Status}");
        }
    }

}


namespace TravelingServices
{
    class Program
    {
        static List<Customer> customers = new List<Customer>();
        static List<HotelOwner> hotelOwners = new List<HotelOwner>();
        static List<TaxiDriver> taxiDrivers = new List<TaxiDriver>();
        static List<AirlineAdmin> airlineAdmins = new List<AirlineAdmin>();
        static List<SystemAdmin> systemAdmins = new List<SystemAdmin>();


        // Services lists
        static List<Hotel> hotels = new();
        static List<Flight> flights = new();
        static List<Taxi> taxis = new();

        // All bookings (for all types)
        static List<Booking> bookings = new();

        static void VirtualData()
        {
            // Add Hotels
            var hotel1 = new Hotel
            {
                ServiceId = 1,
                Name = "Grand Hotel",
                Location = "New York",
                Rating = 5,
                Price = 0
            };
            hotel1.Rooms.Add(new Room { RoomID = "101", RoomType = "Single", Price = 100, IsAvailable = true });
            hotel1.Rooms.Add(new Room { RoomID = "102", RoomType = "Double", Price = 150, IsAvailable = true });
            hotel1.Rooms.Add(new Room { RoomID = "103", RoomType = "Suite", Price = 250, IsAvailable = true });

            var hotel2 = new Hotel
            {
                ServiceId = 2,
                Name = "Sea View Resort",
                Location = "Miami",
                Rating = 4,
                Price = 0
            };
            hotel2.Rooms.Add(new Room { RoomID = "201", RoomType = "Single", Price = 120, IsAvailable = true });
            hotel2.Rooms.Add(new Room { RoomID = "202", RoomType = "Double", Price = 180, IsAvailable = true });

            hotels.Add(hotel1);
            hotels.Add(hotel2);

            // Add Flights
            var flight1 = new Flight
            {
                ServiceId = 1,
                Name = "Flight NY100",
                Origin = "New York",
                Destination = "London",
                DepartureTime = DateTime.Now.AddDays(1).AddHours(9),
                ArrivalTime = DateTime.Now.AddDays(1).AddHours(16),
                AvailableSeats = 50,
                Price = 550
            };

            var flight2 = new Flight
            {
                ServiceId = 2,
                Name = "Flight MI200",
                Origin = "Miami",
                Destination = "Paris",
                DepartureTime = DateTime.Now.AddDays(2).AddHours(14),
                ArrivalTime = DateTime.Now.AddDays(2).AddHours(22),
                AvailableSeats = 70,
                Price = 650
            };

            flights.Add(flight1);
            flights.Add(flight2);

            // Add Taxis
            var taxi1 = new Taxi
            {
                ServiceId = 1,
                Name = "Taxi A",
                DriverName = "Mike",
                CarDetails = "Toyota Prius - White",
                Price = 30,
                IsAvailable = true
            };

            var taxi2 = new Taxi
            {
                ServiceId = 2,
                Name = "Taxi B",
                DriverName = "Alex",
                CarDetails = "Honda Civic - Black",
                Price = 25,
                IsAvailable = true
            };

            taxis.Add(taxi1);
            taxis.Add(taxi2);


        }
        static void Main(string[] args)
        {
            // virtual sysyem admin to check
            systemAdmins.Add(new SystemAdmin("Ahmed", "ahmed@gmail.com", "01023456789", "admin", "12345678"));
            VirtualData();

            bool op = true;

            while (op)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("===========================================");
                Console.WriteLine(" Welcome in The Travelling Services System");
                Console.WriteLine("===========================================");
                Console.WriteLine("      Main Menu      ");
                Console.WriteLine("1.Login");
                Console.WriteLine("2.Register (Customers only) ");
                Console.WriteLine("3. Exit");
                Console.ResetColor();
                Console.Write("Your Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        op = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Thank you for using our System.");
                        Console.ResetColor();

                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invaild Option!");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nاPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void Register()
        {

            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the Full Name field is required");
                Console.ResetColor();
                Console.Write("Fullname:");
                name = Console.ReadLine();
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            // to make sure that phone number 11 dighits
            while (phone.Length < 11)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Phone number must not be less than 11 digits");
                Console.ResetColor();
                Console.WriteLine("Please try again");
                phone = Console.ReadLine();
            }

            Console.Write("Username: ");
            string username = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the username field is required");
                Console.ResetColor();
                Console.Write("Username:");
                username = Console.ReadLine();
            }

            Console.Write("Password: ");
            string password = Console.ReadLine();

            // to make sure that password 8 dighits
            while (password.Length < 8)
            {
                Console.WriteLine("Password must not be less than 8 digits");
                Console.WriteLine("Please try again");
                password = Console.ReadLine();
            }
            Customer newCustomer = new Customer(name, email, phone, username, password);
            newCustomer.Id = customers.Count + 1;
            customers.Add(newCustomer);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Customer registered successfully!");
            Console.ResetColor();
        }


        static void Login()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== Login Menu ===");
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Hotel Owner");
            Console.WriteLine("3. Taxi Driver");
            Console.WriteLine("4. Airline Admin");
            Console.WriteLine("5. System Admin");
            Console.WriteLine("6. Exit");
            Console.ResetColor();
            Console.Write("Choose: ");

            string role = Console.ReadLine();

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            switch (role)
            {
                case "1": // Customer
                    Customer c = customers.Find(u => u.Username == username && u.Password == password);
                    if (c != null) CustomerMenu(c);
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid account.");
                    Console.ResetColor();
                    break;

                case "2": // HotelOwner
                    HotelOwner h = hotelOwners.Find(u => u.Username == username && u.Password == password);
                    if (h != null) HotelOwnerMenu(h);
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid account.");
                    Console.ResetColor();
                    break;

                case "3": // TaxiDriver
                    TaxiDriver t = taxiDrivers.Find(u => u.Username == username && u.Password == password);
                    if (t != null) TaxiDriverMenu(t);
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid account.");
                    Console.ResetColor();
                    break;

                case "4": // AirlineAdmin
                    AirlineAdmin a = airlineAdmins.Find(u => u.Username == username && u.Password == password);
                    if (a != null) AirlineAdminMenu(a);
                    else
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid account.");
                    Console.ResetColor();
                    break;

                case "5": // SystemAdmin
                    SystemAdmin s = systemAdmins.Find(u => u.Username == username && u.Password == password);
                    if (s != null) SystemAdminMenu(s);
                    else
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Invalid account.");
                    Console.ResetColor();
                    break;

                case "6": // Exit
                    Console.WriteLine("Returning to Main Menu...");
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option.");
                    Console.ResetColor();
                    break;
            }
        }


        static void CustomerMenu(Customer c)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Customer Menu");
                Console.WriteLine("1. Search And BookHotel");
                Console.WriteLine("2. Search And Book Taxi");
                Console.WriteLine("3. Search And Book Flight");
                Console.WriteLine("4. Show My Bookings");
                Console.WriteLine("5. Logout");
                Console.ResetColor();
                Console.Write("Your Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SearchAndBookHotel(c);
                        break;
                    case "2":
                        SearchAndBookTaxi(c);
                        break;
                    case "3":
                        SearchAndBookFlight(c);
                        break;

                    case "4":
                        ShowMyBookings(c);
                        break;

                    case "5":
                        loggedIn = false;
                        Console.WriteLine("Logged out Successfully.");
                        break;

                    default:
                        Console.WriteLine("Invaild Option");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void SearchAndBookHotel(Customer c)
        {
            Console.Clear();
            // showing the hotel list
            Console.WriteLine("The Available Hotels");

            for (int i = 0; i < hotels.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {hotels[i].Name} - {hotels[i].Location} ({hotels[i].Rating} Stars)");
            }

            Console.Write("Choose Hotel: ");
            int hotelChoice = int.Parse(Console.ReadLine()) - 1;

            if (hotelChoice < 0 || hotelChoice >= hotels.Count)
            {
                Console.WriteLine("Invaild option!");
                return;
            }

            Hotel selectedHotel = hotels[hotelChoice];
            selectedHotel.ShowAvailableRooms();

            Console.Write("Choose room number: ");
            // searchig for the room by id


            Room selectedRoom = null;

            while (selectedRoom == null)
            {
                Console.WriteLine("Enter the room Id or Enter x to back to the Menu");
                string roomID = Console.ReadLine();
                selectedRoom = selectedHotel.Rooms.Find(r => r.RoomID == roomID && r.IsAvailable);
                // to cancel entering the id 
                if (roomID == "x")
                {
                    Console.WriteLine("Booking has been canceled and Backing to the Menu.");
                    return;
                }

            }

            DateTime checkIn;
            // making sure from entering the date in the correct syntax
            while (true)
            {
                Console.Write("ChechIn Date (yyyy-mm-dd): ");
                string inputI = Console.ReadLine();
                if (DateTime.TryParse(inputI, out checkIn))
                { break; }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invaild Date syntax please try again.");
                    Console.ResetColor();
                }
            }
            // making sure from entering the date in the correct syntax
            DateTime checkOut;
            while (true)
            {
                Console.Write("ChechOut Date (yyyy-mm-dd): ");
                string inputO = Console.ReadLine();
                if (DateTime.TryParse(inputO, out checkOut))
                { break; }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invaild Date syntax please try again.");
                    Console.ResetColor();
                }
            }

            // booking hotel from the list 
            string bookingID = $"B{bookings.Count + 1:000}";
            HotelBooking hb = new HotelBooking(bookingID, c, selectedRoom, checkIn, checkOut);
            hb.ConfirmBooking();
            bookings.Add(hb);

            selectedRoom.BookRoom();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{bookingID},booking has been done successfully.");
            Console.WriteLine("Wish you a Happy stay");
            Console.ResetColor();
        }
        static void SearchAndBookFlight(Customer c)
        {
            Console.Clear();
            Console.WriteLine("=== Available Flights ===");

            if (flights.Count == 0)
            {
                Console.WriteLine("No flights available.");
                return;
            }

            for (int i = 0; i < flights.Count; i++)
            {
                var f = flights[i];
                Console.WriteLine($"{i + 1}. {f.Name} - {f.Origin} → {f.Destination} | " +
                                  $"Dep: {f.DepartureTime}, Arr: {f.ArrivalTime}, Price: {f.Price}$, Seats: {f.AvailableSeats}");
            }

            Console.Write("Choose flight: ");
            int flightChoice = int.Parse(Console.ReadLine()) - 1;

            // making sure that flight choice postive number and in the list
            if (flightChoice < 0 || flightChoice >= flights.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
                return;
            }

            Flight selectedFlight = flights[flightChoice];

            if (!selectedFlight.CheckAvailability())
            {
                Console.WriteLine("No seats available.");
                return;
            }

            string bookingID = $"FB{bookings.Count + 1:000}";
            FlightBooking fb = new FlightBooking(bookingID, c, selectedFlight);

            fb.ConfirmBooking();
            bookings.Add(fb);
            c.Bookings.Add(fb);

            selectedFlight.AvailableSeats--;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Flight booking successful! (ID = {bookingID})");
            Console.WriteLine("Wish you a Happy Travel");
            Console.ResetColor();
        }
        static void SearchAndBookTaxi(Customer c)
        {
            Console.Clear();
            Console.WriteLine("=== Available Taxis ===");

            // show taxies list 
            var allTaxis = taxis.Where(t => t.IsAvailable).ToList();
            
            if (allTaxis.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" No taxis available.");
                Console.ResetColor();
                return;
            }
            
            for (int i = 0; i < allTaxis.Count; i++)
            {
                var t = allTaxis[i];
                Console.WriteLine($"{i + 1}. {t.Name} - Driver: {t.DriverName}, Car: {t.CarDetails}, Price: {t.Price}$");
            }

            Console.Write("Choose taxi: ");
            int taxiChoice = int.Parse(Console.ReadLine()) - 1;

            if (taxiChoice < 0 || taxiChoice >= allTaxis.Count)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice!");
                Console.ResetColor();
                return;
            }

            Taxi selectedTaxi = allTaxis[taxiChoice];

            Console.Write("Pickup Location: ");
            string pickup = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(pickup))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the pickup Location field is required");
                Console.ResetColor();
                Console.Write("Pickup Location:");
                pickup = Console.ReadLine();
            }

            Console.Write("Drop Location: ");
            string drop = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(drop))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the Drop Location field is required");
                Console.ResetColor();
                Console.Write("Drop Location:");
                drop = Console.ReadLine();
            }

            string bookingID = $"TB{bookings.Count + 1:000}";
            TaxiBooking tb = new TaxiBooking(bookingID, c, selectedTaxi, pickup, drop);

            tb.ConfirmBooking();
            bookings.Add(tb);
            c.Bookings.Add(tb);

            selectedTaxi.IsAvailable = false; // taxi is now busy

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Taxi booking successful! (ID = {bookingID})");
            Console.WriteLine("Wish you a Happy Travel");
            Console.ResetColor();
        }

        static void ShowMyBookings(Customer c)
        {
            Console.Clear();
            Console.WriteLine("My bookings");

            foreach (var b in bookings)
            {
                if (b.Customer == c)
                {
                    b.ShowBookingDetails();
                }
            }
        }

        static void CancelBooking(Customer c)
        {
            Console.Clear();
            Console.WriteLine("=== Cancel a Booking ===");

            if (c.Bookings.Count == 0)
            {
                Console.WriteLine("You don't have any bookings to cancel.");
                return;
            }

            for (int i = 0; i < c.Bookings.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                c.Bookings[i].ShowBookingDetails();
            }

            Console.Write("Choose booking number to cancel: ");
            int choice = int.Parse(Console.ReadLine()) - 1;

            // making sure the choice positive number and in th list 
            if (choice < 0 || choice >= c.Bookings.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Booking booking = c.Bookings[choice];

            // Cancel booking
            booking.CancelBooking();

            // Free services after cancel bookings 
            if (booking is HotelBooking hb)
            {
                hb.Room.FreeRoom();
            }
            else if (booking is FlightBooking fb)
            {
                fb.Flight.AvailableSeats++;
            }
            else if (booking is TaxiBooking tb)
            {
                tb.Taxi.IsAvailable = true;
            }

            Console.WriteLine($"Booking {booking.BookingID} cancelled successfully.");
        }

        //  Hotel Owner Menu
        static void HotelOwnerMenu(HotelOwner h)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" Hotel Owner Menu ");
                Console.WriteLine("1. Add a New Hotel");
                Console.WriteLine("2. Update Hotel");
                Console.WriteLine("3. Delete Hotel");
                Console.WriteLine("4. Show My Hotels");
                Console.WriteLine("5. Add Room to a Hotel");
                Console.WriteLine("6. Logout");
                Console.ResetColor();
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddHotel(h);
                        break;
                    case "2":
                        UpdateHotel(h);
                        break;
                    case "3":
                        DeleteHotel(h);
                        break;
                    case "4":
                        ShowHotels(h);
                        break;
                    case "5":
                        AddRoomToHotel(h);
                        break;
                    case "6":
                        loggedIn = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Logged out successfully.");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option.");
                        Console.ResetColor();
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        //  Add Hotel
        static void AddHotel(HotelOwner h)
        {
            Console.Clear();
            Console.WriteLine("=== Add a New Hotel ===");

            Console.Write("Hotel Name: ");
            string name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the Hotel Name field is required");
                Console.ResetColor();
                Console.Write("Hotelname:");
                name = Console.ReadLine();
            }

            Console.Write("Location: ");
            string location = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(location))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the location field is required");
                Console.ResetColor();
                Console.Write("Location:");
                location = Console.ReadLine();
            }

            Console.Write("Rating (1-5): ");
            int rating = int.Parse(Console.ReadLine());

            Hotel hotel = new Hotel
            {
                ServiceId = hotels.Count + 1,
                Name = name,
                Location = location,
                Rating = rating,
                Price = 0
            };

            hotels.Add(hotel);
            h.Hotels.Add(hotel);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Hotel added: {hotel.Name} ({hotel.Location})");
            Console.ResetColor();
        }

        //  Update Hotel
        static void UpdateHotel(HotelOwner h)
        {
            Console.Clear();
            Console.WriteLine("=== Update Hotel ===");

            if (h.Hotels.Count == 0)
            {
                Console.WriteLine("You don't own any hotels.");
                return;
            }

            ShowHotels(h);

            Console.Write("Choose hotel number to update: ");
            int idx = int.Parse(Console.ReadLine()) - 1;

            if (idx < 0 || idx >= h.Hotels.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Hotel hotel = h.Hotels[idx];

            Console.Write("New Hotel Name: ");
            hotel.Name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(hotel.Name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the New Hotelname field is required");
                Console.ResetColor();
                Console.Write("New name:");
                hotel.Name = Console.ReadLine();
            }

            Console.Write("New Location: ");
            hotel.Location = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(hotel.Location))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the New Location field is required");
                Console.ResetColor();
                Console.Write("New Location:");
                hotel.Location = Console.ReadLine();
            }

            Console.Write("New Rating: ");
            hotel.Rating = int.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hotel updated successfully.");
            Console.ResetColor();
        }

        // Delete Hotel
        static void DeleteHotel(HotelOwner h)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Hotel ===");

            if (h.Hotels.Count == 0)
            {
                Console.WriteLine("You don't own any hotels.");
                return;
            }

            ShowHotels(h);

            Console.Write("Choose hotel number to delete: ");
            int index = int.Parse(Console.ReadLine()) - 1;

            if (index < 0 || index >= h.Hotels.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Hotel hotel = h.Hotels[index];
            h.Hotels.Remove(hotel);
            hotels.Remove(hotel);

            Console.WriteLine($"Hotel deleted: {hotel.Name}");
        }

        // Show Hotels
        static void ShowHotels(HotelOwner h)
        {
            Console.Clear();
            Console.WriteLine("Hotels");

            if (h.Hotels.Count == 0)
            {
                Console.WriteLine("You don't own any hotels.");
                return;
            }

            for (int i = 0; i < h.Hotels.Count; i++)
            {
                var hotel = h.Hotels[i];
                Console.WriteLine($"{i + 1}. {hotel.Name} - {hotel.Location} ({hotel.Rating} Stars)");
            }
        }

        // Add Room to a Hotel
        static void AddRoomToHotel(HotelOwner h)
        {
            Console.Clear();
            Console.WriteLine("=== Add Room to a Hotel ===");

            if (h.Hotels.Count == 0)
            {
                Console.WriteLine("You don't own any hotels.");
                return;
            }

            ShowHotels(h);

            Console.Write("Choose hotel number: ");
            int idx = int.Parse(Console.ReadLine()) - 1;

            if (idx < 0 || idx >= h.Hotels.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Hotel selectedHotel = h.Hotels[idx];

            Console.Write("Room ID: ");
            string roomId = Console.ReadLine();

            Console.Write("Room Type: ");
            string type = Console.ReadLine();

            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine());

            Room room = new Room
            {
                RoomID = roomId,
                RoomType = type,
                Price = price,
                IsAvailable = true
            };

            selectedHotel.Rooms.Add(room);

            Console.WriteLine($"Room {room.RoomID} ({room.RoomType}) added to {selectedHotel.Name}.");
        }
        static void TaxiDriverMenu(TaxiDriver t)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"=== Taxi Driver Menu ({t.Username}) ===");
                Console.WriteLine("1. Change Availability");
                Console.WriteLine("2. View My Bookings");
                Console.WriteLine("3. Logout");
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ChangeAvailability(t);
                        break;
                    case "2":
                        ViewDriverBookings(t);
                        break;
                    case "3":
                        loggedIn = false;
                        Console.WriteLine("Logged out successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
        static void ChangeAvailability(TaxiDriver t)
        {
            Console.Clear();
            Console.WriteLine("=== Change Availability ===");
            Console.WriteLine($"Current status: {(t.IsAvailable ? "Available" : "Not Available")}");
            Console.Write("Do you want to change status? (y/n): ");
            string input = Console.ReadLine().ToLower();

            if (input == "y")
            {
                t.IsAvailable = !t.IsAvailable;
                Console.WriteLine($"Status updated: {(t.IsAvailable ? "Available" : "Not Available")}");
            }
            else
            {
                Console.WriteLine("No changes made.");
            }
        }
        static void ViewDriverBookings(TaxiDriver t)
        {
            Console.Clear();
            Console.WriteLine("=== My Taxi Bookings ===");

            if (t.Bookings.Count == 0)
            {
                Console.WriteLine("You have no bookings yet.");
                return;
            }

            foreach (var b in t.Bookings)
            {
                b.ShowBookingDetails();
            }
        }

        static void AirlineAdminMenu(AirlineAdmin a)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"=== Airline Admin Menu ({a.Username}) ===");
                Console.WriteLine("1. Add Flight");
                Console.WriteLine("2. Update Flight");
                Console.WriteLine("3. Delete Flight");
                Console.WriteLine("4. Show All Flights");
                Console.WriteLine("5. Logout");
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddFlight(a);
                        break;
                    case "2":
                        UpdateFlight(a);
                        break;
                    case "3":
                        DeleteFlight(a);
                        break;
                    case "4":
                        ShowAllFlights(a);
                        break;
                    case "5":
                        loggedIn = false;
                        Console.WriteLine("Logged out successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
        static void AddFlight(AirlineAdmin a)
        {
            Console.Clear();
            Console.WriteLine("=== Add a New Flight ===");

            Console.Write("Flight Name: ");
            string name = Console.ReadLine();

            Console.Write("Origin: ");
            string origin = Console.ReadLine();

            Console.Write("Destination: ");
            string destination = Console.ReadLine();

            Console.Write("Departure Time (yyyy-mm-dd HH:mm): ");
            DateTime departure = DateTime.Parse(Console.ReadLine());

            Console.Write("Arrival Time (yyyy-mm-dd HH:mm): ");
            DateTime arrival = DateTime.Parse(Console.ReadLine());

            Console.Write("Available Seats: ");
            int seats = int.Parse(Console.ReadLine());

            Console.Write("Price: ");
            decimal price = decimal.Parse(Console.ReadLine());

            Flight flight = new Flight
            {
                ServiceId = flights.Count + 1,
                Name = name,
                Origin = origin,
                Destination = destination,
                DepartureTime = departure,
                ArrivalTime = arrival,
                AvailableSeats = seats,
                Price = price
            };

            flights.Add(flight);
            a.Flights.Add(flight);

            Console.WriteLine($"Flight {flight.Name} added successfully.");
        }
        static void UpdateFlight(AirlineAdmin a)
        {
            Console.Clear();
            Console.WriteLine("=== Update Flight ===");

            if (a.Flights.Count == 0)
            {
                Console.WriteLine("You don't manage any flights.");
                return;
            }

            ShowAllFlights(a);

            Console.Write("Choose flight number to update: ");
            int idx = int.Parse(Console.ReadLine()) - 1;

            if (idx < 0 || idx >= a.Flights.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Flight flight = a.Flights[idx];

            Console.Write("New Flight Name: ");
            flight.Name = Console.ReadLine();

            Console.Write("New Origin: ");
            flight.Origin = Console.ReadLine();

            Console.Write("New Destination: ");
            flight.Destination = Console.ReadLine();

            Console.Write("New Departure Time (yyyy-mm-dd HH:mm): ");
            flight.DepartureTime = DateTime.Parse(Console.ReadLine());

            Console.Write("New Arrival Time (yyyy-mm-dd HH:mm): ");
            flight.ArrivalTime = DateTime.Parse(Console.ReadLine());

            Console.Write("New Available Seats: ");
            flight.AvailableSeats = int.Parse(Console.ReadLine());

            Console.Write("New Price: ");
            flight.Price = decimal.Parse(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Flight updated successfully.");
            Console.ResetColor();
        }
        static void DeleteFlight(AirlineAdmin a)
        {
            Console.Clear();
            Console.WriteLine("=== Delete Flight ===");

            if (a.Flights.Count == 0)
            {
                Console.WriteLine("You don't manage any flights.");
                return;
            }

            ShowAllFlights(a);

            Console.Write("Choose flight number to delete: ");
            int idx = int.Parse(Console.ReadLine()) - 1;

            if (idx < 0 || idx >= a.Flights.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            Flight flight = a.Flights[idx];
            a.Flights.Remove(flight);
            flights.Remove(flight);

            Console.WriteLine($"Flight {flight.Name} deleted successfully.");
        }
        static void ShowAllFlights(AirlineAdmin a)
        {
            Console.Clear();
            Console.WriteLine("=== Flights You Manage ===");

            if (a.Flights.Count == 0)
            {
                Console.WriteLine("No flights found.");
                return;
            }

            for (int i = 0; i < a.Flights.Count; i++)
            {
                var f = a.Flights[i];
                Console.WriteLine($"{i + 1}. {f.Name} - {f.Origin} → {f.Destination} | " +
                                  $"Dep: {f.DepartureTime}, Arr: {f.ArrivalTime}, Price: {f.Price}$, Seats: {f.AvailableSeats}");
            }
        }
        // system admin
        static void SystemAdminMenu(SystemAdmin s)
        {
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"=== System Admin Menu ({s.Username}) ===");
                Console.WriteLine("1. Add Hotel Owner");
                Console.WriteLine("2. Add Taxi Driver");
                Console.WriteLine("3. Add Airline Admin");
                Console.WriteLine("4. View All Customers");
                Console.WriteLine("5. View All Bookings");
                Console.WriteLine("6. Delete a Customer");
                Console.WriteLine("7. Logout");
                Console.ResetColor();
                Console.Write("Choose: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddHotelOwner();
                        break;
                    case "2":
                        AddTaxiDriver();
                        break;
                    case "3":
                        AddAirlineAdmin();
                        break;
                    case "4":
                        ViewAllCustomers();
                        break;
                    case "5":
                        ViewAllBookings();
                        break;
                    case "6":
                        DeleteCustomer();
                        break;
                    case "7":
                        loggedIn = false;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Logged out successfully.");
                        Console.ResetColor();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
        static void AddHotelOwner()
        {
            Console.WriteLine("Adding Hotel Owner: ");
            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("sorry,the Full Name field is required");
                Console.Write("Fullname:");
                name = Console.ReadLine();
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            // to make sure that phone number 11 dighits
            while (phone.Length < 11)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Phone number must not be less than 11 digits");
                Console.ResetColor();
                Console.WriteLine("Please try again");
                phone = Console.ReadLine();
            }


            Console.Write("Username: ");
            string username = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("sorry,the username field is required");
                Console.Write("Username:");
                username = Console.ReadLine();
            }

            Console.Write("Password: ");
            string password = Console.ReadLine();

            // to make sure that password 8 dighits
            while (password.Length < 8)
            {
                Console.WriteLine("Password must not be less than 8 digits");
                Console.WriteLine("Please try again");
                password = Console.ReadLine();
            }
            HotelOwner newOwner = new HotelOwner(name, email, phone, username, password);
            newOwner.Id = hotelOwners.Count + 1;
            hotelOwners.Add(newOwner);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hotel Owner added successfully!");
            Console.ResetColor();
        }
        //adding taxi driver by the system admin
        static void AddTaxiDriver()
        {
            Console.WriteLine("Adding Taxi Driver: ");
            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the Full Name field is required");
                Console.Write("Fullname:");
                name = Console.ReadLine();
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            // to make sure that phone number 11 dighits
            while (phone.Length < 11)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Phone number must not be less than 11 digits");
                Console.ResetColor();
                Console.WriteLine("Please try again");
                phone = Console.ReadLine();
            }


            Console.Write("Username: ");
            string username = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("sorry,the username field is required");
                Console.Write("Username:");
                username = Console.ReadLine();
            }

            Console.Write("Password: ");
            string password = Console.ReadLine();

            // to make sure that password 8 dighits
            while (password.Length < 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Password must not be less than 8 digits");
                Console.ResetColor();
                Console.WriteLine("Please try again");
                password = Console.ReadLine();
            }

            Console.Write("Car Details: ");
            string car = Console.ReadLine();

            TaxiDriver newDriver = new TaxiDriver(name, email, phone, username, password, car);
            newDriver.Id = taxiDrivers.Count + 1;
            taxiDrivers.Add(newDriver);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Taxi Driver added successfully!");
            Console.ResetColor();
        }
        static void AddAirlineAdmin()
        {
            Console.WriteLine("Airline Admin: ");
            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("sorry,the Full Name field is required");
                Console.Write("Fullname:");
                name = Console.ReadLine();
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            // to make sure that phone number 11 dighits
            while (phone.Length < 11)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Phone number must not be less than 11 digits");
                Console.ResetColor();
                Console.WriteLine("Please try again");
                phone = Console.ReadLine();
            }


            Console.Write("Username: ");
            string username = Console.ReadLine();
            //make sure the text is not empty and does not contain only spaces
            while (string.IsNullOrWhiteSpace(username))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("sorry,the username field is required");
                Console.ResetColor();
                Console.Write("Username:");
                username = Console.ReadLine();
            }

            Console.Write("Password: ");
            string password = Console.ReadLine();

            // to make sure that password 8 dighits
            while (password.Length < 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Password must not be less than 8 digits");
                Console.ResetColor();
                    
                Console.WriteLine("Please try again");
                password = Console.ReadLine();
            }
            AirlineAdmin newAdmin = new AirlineAdmin(name, email, phone, username, password);
            newAdmin.Id = airlineAdmins.Count + 1;
            airlineAdmins.Add(newAdmin);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Airline Admin added successfully!");
            Console.ResetColor();
        }
        static void ViewAllCustomers()
        {
            Console.Clear();
            Console.WriteLine("=== All Customers ===");

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers registered.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.Id}, Name: {c.Name}, Username: {c.Username}, Email: {c.Email}");
            }
        }
        static void ViewAllBookings()
        {
            Console.Clear();
            Console.WriteLine("=== All Bookings ===");

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings found.");
                return;
            }

            foreach (var b in bookings)
            {
                b.ShowBookingDetails();
            }
        }
        static void DeleteCustomer()
        {
            Console.Clear();
            Console.WriteLine("=== Delete a Customer ===");

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers registered.");
                return;
            }

            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.Id}, Name: {c.Name}, Username: {c.Username}");
            }

            Console.Write("Enter Customer ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            var customer = customers.FirstOrDefault(c => c.Id == id);

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            // Remove their bookings too
            bookings.RemoveAll(b => b.Customer == customer);
            customers.Remove(customer);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Customer {customer.Name} (ID: {customer.Id}) deleted successfully.");
            Console.ResetColor();
        }





    }
}









