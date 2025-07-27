using EventClassLibrary.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EventExtension.Data
{
    public static class SeedEvents
    {
        public static async Task SeedEvent(EventDBContext context)
        {
            if (context.Events.Any())
            {
                return;
            } 

            //var json = await File.ReadAllTextAsync("Data/HSTD.json");
            //var json2 = await File.ReadAllTextAsync("Data/eventsVBG.json"); 
            var json3 = await File.ReadAllTextAsync("Data/eventsFBG.json");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,                
            };

            //var events = JsonSerializer.Deserialize<List<EventClassLibrary.Models.EventItem>>(json, options);         
            //var eventsVBG = JsonSerializer.Deserialize<List<EventClassLibrary.Models.EventItem>>(json2, options);
            var eventsHSTD = JsonSerializer.Deserialize<List<EventItem>>(json3, options);
            if (/*events != null ||*/ eventsHSTD != null)
            {
                foreach (var ev in eventsHSTD)
                {
                    if (ev.EventDates != null)
                    {
                        foreach (var date in ev.EventDates)
                        {
                            date.EventItem = ev;
                        }
                    }


                }
                await context.Events.AddRangeAsync(eventsHSTD);
                await context.SaveChangesAsync();
            }
        }
    }
}