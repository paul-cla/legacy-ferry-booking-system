using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FerryLegacy.Tests
{

    [TestFixture]
    public class SaffSqueeze
    {
        [Test]
        public void test_one()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var bookings = new Bookings();
            var _ports = new Ports();
            var _ferryService = new FerryAvailabilityService(_ports, ferries, timeTables,
                new PortManager(_ports, ferries));
            var _bookingService = new JourneyBookingService(timeTables, bookings, _ferryService);
            var _timeTableService = new TimeTableService(timeTables, bookings, _ferryService);

            var ports = _ports.All();
            var timeTable = _timeTableService.GetTimeTable(ports);

            foreach (var port in ports)
            {
                var items = timeTable.Where(x => x.OriginPort == port.Name).OrderBy(x => x.StartTime);
                var firstBoat = items.First();

                Assert.That(firstBoat.FerryName, Is.EqualTo("Titanic"));
            }
        }

        [Test]
        public void test_two()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var ferryService = new FerryAvailabilityService(portService, ferries, timeTables,
                new PortManager(portService, ferries));

            var ports = portService.All();
            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();
            var rows = new List<TimeTableViewModelRow>();

            foreach (var timetable in allEntries)
            {
                var origin = ports.Single(x => x.Id == timetable.OriginId);
                var destination = ports.Single(x => x.Id == timetable.DestinationId);
                var destinationName = destination.Name;
                var originName = origin.Name;
                var ferry = ferryService.NextFerryAvailableFrom(origin.Id, timetable.Time);
                var arrivalTime = timetable.Time.Add(timetable.JourneyTime);
                var row = new TimeTableViewModelRow
                {
                    DestinationPort = destinationName,
                    FerryName = ferry == null ? "" : ferry.Name,
                    JourneyLength = timetable.JourneyTime.ToString("hh':'mm"),
                    OriginPort = originName,
                    StartTime = timetable.Time.ToString("hh':'mm"),
                    ArrivalTime = arrivalTime.ToString("hh':'mm"),
                };
                rows.Add(row);
            }

            var firstRow = rows.First();
            Assert.That(firstRow.FerryName, Is.EqualTo("Titanic"));
        }

        [Test]
        public void test_three()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var ferryService = new FerryAvailabilityService(portService, ferries, timeTables,
                new PortManager(portService, ferries));

            var ports = portService.All();
            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();
            var firstEntry = allEntries.First();
            var origin = ports.Single(x => x.Id == firstEntry.OriginId);
            var ferry = ferryService.NextFerryAvailableFrom(origin.Id, firstEntry.Time);

            var theFerryName = ferry == null ? "" : ferry.Name;

            Assert.That(theFerryName, Is.EqualTo("Titanic"));
        }

        [Test]
        public void test_four()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var ferryService = new FerryAvailabilityService(portService, ferries, timeTables,
                new PortManager(portService, ferries));

            var ports = portService.All();
            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();
            var firstEntry = allEntries.First();
            var origin = ports.Single(x => x.Id == firstEntry.OriginId);
            var ferry = ferryService.NextFerryAvailableFrom(origin.Id, firstEntry.Time);

            Assert.That(ferry, Is.Not.Null);
        }

        [Test]
        public void test_five()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var portManager = new PortManager(portService, ferries);

            var ports = portService.All();
            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();
            var firstEntry = allEntries.First();
            var origin = ports.Single(x => x.Id == firstEntry.OriginId);
            //var ferry = ferryService.NextFerryAvailableFrom(origin.Id, firstEntry.Time);

            Ferry returnedFerry = null;
            var portManagerPorts = portManager.PortModels();

            foreach (var entry in allEntries)
            {
                var ferry = FerryManager.CreateFerryJourney(portManagerPorts, entry);
                if (ferry != null)
                {
                    PortModel destination = ferry.Destination;
                    if (ferry.Ferry == null)
                        FerryManager.AddFerry(entry, ferry);

                    var ferry1 = ferry.Ferry;

                    var time = FerryModule.TimeReady(entry, destination);
                    destination.AddBoat(time, ferry1);
                }
                if (entry.OriginId == origin.Id)
                {
                    if (entry.Time >= firstEntry.Time)
                    {
                        if (ferry != null)
                        {
                            returnedFerry = ferry.Ferry;
                        }
                    }
                }
            }

            Assert.That(returnedFerry, Is.Not.Null);
        }

        [Test]
        public void test_six()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var portManager = new PortManager(portService, ferries);

            var ports = portService.All();
            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();
            var firstEntry = allEntries.First();
            var origin = ports.Single(x => x.Id == firstEntry.OriginId);

            var portManagerPorts = portManager.PortModels();

            foreach (var entry in allEntries)
            {
                var ferry = FerryManager.CreateFerryJourney(portManagerPorts, entry);
                if (ferry != null)
                {
                    PortModel destination = ferry.Destination;
                    if (ferry.Ferry == null)
                        FerryManager.AddFerry(entry, ferry);

                    var ferry1 = ferry.Ferry;

                    var time = FerryModule.TimeReady(entry, destination);
                    destination.AddBoat(time, ferry1);
                }

                if (entry.OriginId == origin.Id)
                {
                    if (entry.Time >= firstEntry.Time)
                    {
                        if (ferry != null)
                        {
                            Assert.That(ferry.Ferry, Is.Not.Null);
                        }
                    }
                }
            }
        }

        [Test]
        public void test_seven()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var portManager = new PortManager(portService, ferries);

            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            var portManagerPorts = portManager.PortModels();

            foreach (var entry in allEntries)
            {
                var ferry = FerryManager.CreateFerryJourney(portManagerPorts, entry);
                if (ferry != null)
                {
                    PortModel destination = ferry.Destination;
                    if (ferry.Ferry == null)
                        FerryManager.AddFerry(entry, ferry);

                    var ferry1 = ferry.Ferry;

                    var time = FerryModule.TimeReady(entry, destination);
                    destination.AddBoat(time, ferry1);

                    Assert.That(ferry.Ferry, Is.Not.Null);
                }
            }
        }

        [Test]
        public void test_eight()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var portManager = new PortManager(portService, ferries);

            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            var portManagerPorts = portManager.PortModels();

            foreach (var entry in allEntries)
            {
                var ferry = FerryManager.CreateFerryJourney(portManagerPorts, entry);
                if (ferry != null)
                {
                    if (ferry.Ferry == null)
                    {
                        FerryManager.AddFerry(entry, ferry);
                        Assert.That(ferry.Ferry, Is.Not.Null);
                    }
                }
            }
        }

        [Test]
        public void test_eight_b()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();
            var portManager = new PortManager(portService, ferries);

            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            var portManagerPorts = portManager.PortModels();

            foreach (var entry in allEntries)
            {
                var ferry = FerryManager.CreateFerryJourney(portManagerPorts, entry);
                if (ferry != null)
                {
                    if (ferry.Ferry == null)
                    {
                        ferry.Ferry = ferry.Origin.GetNextAvailable(entry.Time);
                        Assert.That(ferry.Ferry, Is.Not.Null);
                    }
                }
            }
        }


        [Test]
        public void test_nine()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();

            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            var ports = portService.All().Select(x => new PortModel(x)).ToList();

            Dictionary<int, TimeSpan> boatAvailability = new Dictionary<int, TimeSpan>();
            List<Ferry> boats = new List<Ferry>();

            foreach (var ferry in ferries.All())
            {
                boats.Add(ferry);
                boatAvailability.Add(ferry.Id, new TimeSpan(0, 0, 10));
            }

            foreach (var entry in allEntries)
            {
                var journey = new FerryJourney
                {
                    Origin = ports.Single(x => x.Id == entry.OriginId),
                    Destination = ports.Single(x => x.Id == entry.DestinationId)
                };

                if (journey.Ferry != null) continue;
                var available = boatAvailability.FirstOrDefault(x => entry.Time >= x.Value);
                Assert.That(available.Key, Is.Not.EqualTo(0));
            }
        }

        [Test]
        public void test_ten()
        {
            var timeTables = new TimeTables();
            var ferries = new Ferries();
            var portService = new Ports();

            var timetables = timeTables.All();

            var allEntries = timetables.SelectMany(x => x.Entries).OrderBy(x => x.Time).ToList();

            var ports = portService.All().Select(x => new PortModel(x)).ToList();

            Dictionary<int, TimeSpan> boatAvailability = new Dictionary<int, TimeSpan>();
            List<Ferry> boats = new List<Ferry>();

            foreach (var ferry in ferries.All())
            {
                boats.Add(ferry);
                boatAvailability.Add(ferry.Id, new TimeSpan(0, 0, 10));
            }

            foreach (var entry in allEntries)
            {
                var journey = new FerryJourney
                {
                    Origin = ports.Single(x => x.Id == entry.OriginId),
                    Destination = ports.Single(x => x.Id == entry.DestinationId)
                };

                if (journey.Ferry != null) continue;
                Assert.That(boatAvailability.Any(x => entry.Time >= x.Value), Is.True);
                var available = boatAvailability.FirstOrDefault(x => entry.Time >= x.Value);
                Assert.That(available.Key, Is.Not.EqualTo(0));
            }
        }
    }
}
