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
      string path = "";

      if (args.Length > 0) path = args[0];
      else {
        Console.WriteLine("Current directory is " + Directory.GetCurrentDirectory());
        Console.Write("Enter a directory or leave blank for the above: ");
        path = Console.ReadLine();
      }

      if (path != "") Directory.SetCurrentDirectory(path);

      // Get the configuration
      Console.WriteLine("Reading config...");
      Config.Load();

      // Make sure we have directories
      Console.WriteLine("Creating folders...");
      Directory.CreateDirectory("gtfs");
      Directory.CreateDirectory("shapes");
      Directory.CreateDirectory("trips-times");

      // Initialize lists of things
      List<CompleteRelation> agencies = new List<CompleteRelation>();
      List<ICompleteOsmGeo> stops = new List<ICompleteOsmGeo>();

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
            if (obj.Type == OsmGeoType.Relation && obj.Tags.Contains("type", "network"))
              agencies.Add((CompleteRelation)obj);
            // Get routes
            else if (obj.Type != OsmGeoType.Relation && obj.Tags.Contains("public_transport", "platform"))
              stops.Add(obj);
          }
        }

        // Now make the agency file
        Console.WriteLine("Found " + agencies.Count + " agency(ies).");
        Console.WriteLine("Writing agency.txt...");
        AgencyFile.Create(agencies);
        Console.WriteLine("Done.");

        // Now make the stops file
        Console.WriteLine("Found " + stops.Count + " stop(s).");
        Console.WriteLine("Writing stops.txt...");
        StopFile.Create(stops);
        Console.WriteLine("Done.");
      }
    }
  }
}