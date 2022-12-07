using CutomerEvents;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CutomerEvents
{
    public class Program
    {
        static HashSet<string> Cities = new HashSet<string>();
        static Dictionary<string, EventWithDistance> CitiesDistances = new Dictionary<string, EventWithDistance>();
        static void Main(string[] args)
        {
            var events = new List<Event>{
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23)),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02)),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06)),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23)),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20)),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01)),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04)),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07)),
                new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22)),
                new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01)),
                new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04)),
                new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15))
            };
            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };

            Console.WriteLine("How many results per filteration:");
            Int32.TryParse(Console.ReadLine().ToString(), out int count);

            var engine = new Engine(events);
            Console.WriteLine($"1- Filter events by customer city: {customer.City}");
            engine.FilterEventsByCustomerCity(customer, count);
            Console.WriteLine();

            Console.WriteLine($"2- Filter events by customer Birthdate: {customer.BirthDate}");
            engine.FilterEventsByCustomerBirthday(customer, count);
            Console.WriteLine();

            Console.WriteLine($"3- Order events by closest to the customer in city: {customer.City}");
            engine.OrderEventsByClosest(customer, count);
            Console.WriteLine();

            Console.WriteLine($"4- Sort events by Id");
            engine.SortEvents(customer, x => x.Id, count);
            Console.WriteLine();

        }
        class Engine
        {
            private List<Event> Events { get; set; }
            private Dictionary<(string, string), int> citiesDistances = new Dictionary<(string, string), int>();
            public Engine(List<Event> events)
            {
                this.Events = events;
            }
            public void SendCustomerNotifications(Customer customer, Event e)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");
            }
            public void FilterEventsByCustomerCity(Customer customer, int count = 5)
            {
                var events = Events.Where(x => x.City == customer.City).Take(count);
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }
            public void FilterEventsByCustomerBirthday(Customer customer, int count = 5)
            {
                var now = DateTime.UtcNow;
                var birthdateDay = customer.BirthDate.Day;
                var birthdateMonth = customer.BirthDate.Month;
                var birthdateYear = customer.BirthDate.Year;

                var events = this.Events.Where(x => x.Date > now
                && (x.Date.Month >= birthdateMonth && x.Date.Day > birthdateDay) || (x.Date.Month > birthdateMonth))
                    .OrderBy(x => x.Date)
                    .Take(count)
                    .ToList();
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }
            public void OrderEventsByClosest(Customer customer, int count = 5)
            {
                var events = Events
                    .OrderBy(x => GetDistances(customer.City, x.City))
                    .Take(count)
                    .ToList();
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }
            public void SortEvents(Customer customer, Func<Event, int> func, int count = 5)
            {
                var events = Events
                    .OrderBy(func)
                    .Take(count)
                    .ToList();
                foreach (var e in events)
                {
                    SendCustomerNotifications(customer, e);
                }
            }
            private int GetDistances(string cityA, string cityB, int tries = 5)
            {
                if (citiesDistances.ContainsKey((cityA, cityB))) return citiesDistances[(cityA, cityB)];
                int distance = 0;
                int counter = 0;
                while (counter < tries)
                {
                    try
                    {
                        distance = AlphebiticalDistance(cityA, cityB);
                        break;
                    }
                    catch (Exception e)
                    {
                        counter++;
                        if (counter >= tries)
                        {
                            Console.WriteLine($"Exception {e} happened with {cityA}, and {cityB}");
                            throw;
                        }
                    }
                }
                citiesDistances[(cityA, cityB)] = distance;
                citiesDistances[(cityB, cityA)] = distance;
                return distance;
            }
            private int AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for (i = 0; i < Math.Min(s.Length, t.Length); i++)
                {
                    // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                    result += Math.Abs(s[i] - t[i]);
                }
                for (; i < Math.Max(s.Length, t.Length); i++)
                {
                    // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                    result += s.Length > t.Length ? s[i] : t[i];
                }

                return result;
            }
        }
        class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime Date { get; set; }

            public Event(int id, string name, string city, DateTime date)
            {
                this.Id = id;
                this.Name = name;
                this.City = city;
                this.Date = date;
            }
        }
        class EventWithDistance
        {
            public int Distance { get; set; }
            public Event Event { get; set; }
        }

        class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime BirthDate { get; set; }
        }

        /*-------------------------------------
        Coordinates are roughly to scale with miles in the USA
           2000 +----------------------+  
                |                      |  
                |                      |  
             Y  |                      |  
                |                      |  
                |                      |  
                |                      |  
                |                      |  
             0  +----------------------+  
                0          X          4000
        ---------------------------------------*/

    }
}
