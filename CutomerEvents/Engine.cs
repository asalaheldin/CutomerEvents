using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TicketsConsole.Program;

namespace CutomerEvents
{

    internal class Engine
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
            foreach(var e in events)
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
            foreach(var e in events)
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
            foreach(var e in events)
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
            foreach(var e in events)
            {
                SendCustomerNotifications(customer, e);
            }
        }
        private int GetDistances(string cityA, string cityB, int tries = 5)
        {
            if (citiesDistances.ContainsKey((cityA, cityB))) return citiesDistances[(cityA, cityB)];
            int distance = 0;
            int counter = 0;
            while(counter < tries)
            {
                try
                {
                    distance = AlphebiticalDistance(cityA, cityB);
                    break;
                }
                catch(Exception e)
                {
                    counter++;
                    if(counter >= tries)
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
}
