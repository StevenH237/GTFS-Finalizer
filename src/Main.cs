using System.Collections.Generic;
using System.IO;
using System;
using OsmSharp.Streams;
using OsmSharp.Streams.Complete;
using System.Linq;
using OsmSharp.Complete;
using OsmSharp;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;
using Nixill.GTFS.Objects;

namespace Nixill.GTFS {
  public class FinalizerMain {
    static void Main(string[] args) {
      /* TODO DO NOT FORGET TO PUT THIS BACK
      string path = "";

      if (args.Length > 0) path = args[0];
      else {
        Console.WriteLine("Current directory is " + Directory.GetCurrentDirectory());
        Console.Write("Enter a directory or leave blank for the above: ");
        path = Console.ReadLine();
      } */

      string path = @"C:\Users\Nixill\Documents\Git\NXS-GTFS";

      if (path != "") Directory.SetCurrentDirectory(path);

      // Get the configuration
      Console.WriteLine("Reading config...");
      Config.Load();

      // Make sure we have directories
      Console.WriteLine("Creating folders...");
      Directory.CreateDirectory("gtfs");
      Directory.CreateDirectory("shapes");
      Directory.CreateDirectory("trip-times");
      Directory.CreateDirectory("trips");

      // Initialize lists of things
      List<CompleteRelation> agencies = new List<CompleteRelation>();
      List<ICompleteOsmGeo> stops = new List<ICompleteOsmGeo>();
      Dictionary<long, RouteRelation> routes = new Dictionary<long, RouteRelation>();

      Console.WriteLine("Reading map file...");
      using (FileStream fileStream = File.OpenRead("map.osm")) {
        // Open the map file
        XmlOsmStreamSource xmlSrc = new XmlOsmStreamSource(fileStream);

        // We'll keep the whole thing so we can iterate over it multiple times
        var src = (from osmGeo in xmlSrc select osmGeo).ToComplete();

        // Iterate through the file to get all the agencies
        // Other things will be added later
        foreach (var obj in src) {
          if (obj.Tags != null) {
            // Get agencies
            // Routes are added within each agency
            if (obj.Type == OsmGeoType.Relation && obj.Tags.Contains("type", "network"))
              agencies.Add((CompleteRelation)obj);

            // Get stops
            else if (obj.Type != OsmGeoType.Relation && obj.Tags.Contains("public_transport", "platform"))
              stops.Add(obj);
          }
        }

        Console.WriteLine("");

        // Now make the agency file
        Console.WriteLine("Found " + agencies.Count + " agency(ies).");
        Console.WriteLine("Writing agency.txt...");
        AgencyFile.Create(agencies, routes);
        Console.WriteLine("Done.");

        Console.WriteLine("");

        // Now make the stops file
        Console.WriteLine("Found " + stops.Count + " stop(s).");
        Console.WriteLine("Writing stops.txt...");
        StopFile.Create(stops);
        Console.WriteLine("Done.");

        Console.WriteLine("");

        // Now make the routes file
        Console.WriteLine("Found " + routes.Count + " route(s).");
        Console.WriteLine("Writing routes.txt...");
        RoutesFile.Create(routes.Values);
        Console.WriteLine("Done.");

        Console.WriteLine("");

        // Now make the trips file
        Console.WriteLine("Writing trips.txt by merging existing files...");
        TripsFile.Create();
        Console.WriteLine("Done.");

        Console.WriteLine("");

        // Now make the times file
        Console.WriteLine("Writing stop_times.txt by merging existing files...");
        TimesFile.Create();
        Console.WriteLine("Done.");

        Console.WriteLine("");

        // Now make the trips file
        Console.WriteLine("Writing shapes.txt by merging existing files...");
        ShapesFile.Create();
        Console.WriteLine("Done.");
      }
    }
  }
}