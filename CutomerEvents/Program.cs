using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CutomerEvents
{
    class Event
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    class Customer
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
    class CityDistance
    {
        public int Distance { get; set; }
        public string City { get; set; }
    }

    class Program
    {
        public static Dictionary<string, List<Event>> Events { get; set; }
        static async Task Main(string[] args)
        {
            var events = new List<Event> {
                new Event { Name = "Phantom of the Opera", City ="New York" },
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
            };
            var customer = new Customer { Name = "Mr. Fake", City = "New York" };

            Events = new Dictionary<string, List<Event>>();

            //prepare list of events to be in dictionary
            await PrepareEvents(events);

            //Asking user which task it want to perform 
            Console.WriteLine("Choose which task you need to perform 1 or 2:");
            Int32.TryParse(Console.ReadLine(), out int choice);
            Console.WriteLine();

            var closeEvents = choice == 2 ? await GetClosestEvents(customer.City) : Events[customer.City];
            Console.WriteLine($"***Start Preparing email to customer: {customer.Name} from city: {customer.City}***");
            Console.WriteLine();
            var tasks = new List<Task>();
            foreach (var item in closeEvents.Take(5))
            {
                tasks.Add(AddToEmail(customer, item));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine();
            Console.WriteLine($"***Finish Preparing email to customer: {customer.Name} from city: {customer.City}***");
        }

        

        static async Task PrepareEvents(List<Event> events)
        {
            foreach (var item in events)
            {
                if (!Events.ContainsKey(item.City))
                    Events.Add(item.City, new List<Event>());

                Events[item.City].Add(item);
            }
        }
        static async Task<List<Event>> GetClosestEvents(string customerCity)
        {
            var closeEvents = new List<Event>();
            var orderedCityDistances = new SortedSet<CityDistance>(Comparer<CityDistance>.Create((a, b) => a.Distance.CompareTo(b.Distance)));
            foreach(var item in Events)
            {
                orderedCityDistances.Add(new CityDistance() { Distance = await GetDistance(customerCity, item.Key), City = item.Key });
            }
            foreach(var cityDistance in orderedCityDistances)
            {
                closeEvents.AddRange(Events[cityDistance.City]);
                if (closeEvents.Count >= 5)
                    break;
            }

            return closeEvents;
        }
        static async Task AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = await GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
            + (distance > 0 ? $" ({distance} miles away)" : "")
            + (price.HasValue ? $" for ${price}" : ""));
        }
        static async Task<int> GetPrice(Event e)
        {
            return (await AlphebiticalDistance(e.City, "") + await AlphebiticalDistance(e.Name, "")) / 10;
        }
        static async Task<int> GetDistance(string fromCity, string toCity)
        {
            return await AlphebiticalDistance(fromCity, toCity);
        }
        static async Task<int> AlphebiticalDistance(string s, string t)
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
}
