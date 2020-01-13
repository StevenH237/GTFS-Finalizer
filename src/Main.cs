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

      // Open the map file
      XmlOsmStreamSource src = null;

      using (FileStream fileStream = File.OpenRead("map.osm"))
      {
        src = new XmlOsmStreamSource(fileStream);
        // We'll keep the whole thing so we can iterate over it multiple times

        // Get all agencies from the file
        var agencies = (from osmGeo in src
                        where (osmGeo.Type == OsmSharp.OsmGeoType.Relation
                          && osmGeo.Tags != null
                          && osmGeo.Tags.Contains("type", "network"))
                        select osmGeo).ToComplete();

        Grid<string> agencyGrid = new Grid<string>();

        List<Agency> agencyList = new List<Agency>();

        foreach (var agency in agencies)
        {
          agencyList.Add(new Agency((CompleteRelation)agency));
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