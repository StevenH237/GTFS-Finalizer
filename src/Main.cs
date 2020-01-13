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

namespace Nixill.GTFS
{
  public class FinalizerMain
  {
    static void Main(string[] args)
    {
      string path = "";

      if (args.Length > 0) path = args[0];
      else
      {
        Console.WriteLine("Current directory is " + Directory.GetCurrentDirectory());
        Console.Write("Enter a directory or leave blank for the above: ");
        path = Console.ReadLine();
      }

      if (path != "") Directory.SetCurrentDirectory(path);

      // Make sure we have directories
      Directory.CreateDirectory("gtfs");
      Directory.CreateDirectory("shapes");
      Directory.CreateDirectory("trips-times");

      // Enter a timezone for the GTFS file
      Console.Write("Enter a time zone for all agencies: ");
      Agency.SetTimeZone(Console.ReadLine());

      // Initialize lists of things
      List<CompleteRelation> agencies = new List<CompleteRelation>();
      List<ICompleteOsmGeo> stops = new List<ICompleteOsmGeo>();

      using (FileStream fileStream = File.OpenRead("map.osm"))
      {
        // Open the map file
        XmlOsmStreamSource xmlSrc = new XmlOsmStreamSource(fileStream);

        // We'll keep the whole thing so we can iterate over it multiple times
        var src = (from osmGeo in xmlSrc select osmGeo).ToComplete();

        // Iterate through the file to get all the agencies
        // Other things will be added later
        foreach (var obj in src)
        {
          if (obj.Type == OsmGeoType.Relation && obj.Tags != null && obj.Tags.Contains("type", "network"))
            agencies.Add((CompleteRelation)obj);
        }

        Grid<string> agencyGrid = new Grid<string>();

        List<Agency> agencyList = new List<Agency>();

        foreach (var agency in agencies)
        {
          agencyList.Add(new Agency(agency));
        }

        agencyGrid.AddRow(Agency.GetHeaderRow());

        foreach (Agency agency in agencyList)
        {
          agencyGrid.AddRow(agency.GetRow());
        }

        CSVParser.GridToFile(agencyGrid, "gtfs/agency.txt");
      }
    }
  }
}