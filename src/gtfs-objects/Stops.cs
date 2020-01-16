using System.Collections.Generic;
using System.Linq;
using System;
using OsmSharp;
using OsmSharp.Complete;
using OsmSharp.Tags;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;

namespace Nixill.GTFS.Objects {
  public class Stop {
    public readonly string ID;
    public readonly string Name;
    public readonly double Lat;
    public readonly double Lon;
    public readonly int Type;
    public readonly string Code;
    public readonly string Desc;
    public readonly string Zone;
    public readonly string Url;
    public readonly int Wheelchairs;

    private static bool UseCodes = false;
    private static bool UseDescs = false;
    private static bool UseZones = false;
    private static bool UseUrls = false;
    private static bool UseWheelchair = false;

    public Stop(ICompleteOsmGeo stop) {
      string id = null;
      string code = null;
      string name = null;
      string desc = null;
      string zone = null;
      string url = null;
      int type = 0;
      int wheelchairs = 0;

      foreach (Tag tag in stop.Tags) {
        if (tag.Key == Config.StopIDTag) id = tag.Value;

        // Assign values to properties from tags
        if (tag.Key == "name") name = tag.Value;
        else if (tag.Key.StartsWith("name:") && name == null) name = tag.Value;
        else if (tag.Key == "ref") code = tag.Value;
        else if (tag.Key == "website" || tag.Key == "contact:website") {
          url = tag.Value;
          UseUrls = true;
        }
        else if (tag.Key == "desc") {
          desc = tag.Value;
          UseDescs = true;
        }
        else if (tag.Key == "fare_zone") {
          zone = tag.Value;
          UseZones = true;
        }
        else if (tag.Key == "wheelchair") {
          if (tag.Value == "yes" || tag.Value == "limited") {
            wheelchairs = 1;
            UseWheelchair = true;
          }
          else if (tag.Value == "no") {
            wheelchairs = 2;
            UseWheelchair = true;
          }
        }
      }

      if (stop is Node stopNode) {
        Lat = (double)(stopNode.Latitude);
        Lon = (double)(stopNode.Longitude);
      }
      else if (stop is CompleteWay stopWay) {
        Node[] stopNodes = stopWay.Nodes;
        int nodeCount = stopNodes.Length;

        if (stopNodes[0] == stopNodes.Last()) {
          // closed way - we'll use the center of the bounding box

          // This will cause a bug if the polygon goes across 180 degrees
          // longitude. It's extremely edge case, and a fix would cost
          // processing time that doesn't need to be spent on cases where
          // it doesn't apply, so it won't be fixed.

          double north = -90.0, south = 90.0, east = -180.0, west = 180.0;

          foreach (Node node in stopNodes) {
            if (node.Latitude > north) north = (double)node.Latitude;
            if (node.Latitude < south) south = (double)node.Latitude;
            if (node.Longitude > east) east = (double)node.Longitude;
            if (node.Longitude < west) west = (double)node.Longitude;
          }

          Lat = (north + south) / 2;
          Lon = (east + west) / 2;
        }
        else {
          if (nodeCount % 2 == 0) {
            // open way with even number of nodes

            // This will cause a bug if the two median nodes are on
            // opposite sides of 180 degrees longitude. It's extremely
            // edge case, and a fix would cost processing time that
            // doesn't need to be spent on cases where it doesn't apply,
            // so it won't be fixed.

            Node node1 = stopNodes[nodeCount / 2];
            Node node2 = stopNodes[nodeCount / 2 - 1];

            Lat = ((double)node1.Latitude + (double)node2.Latitude) / 2;
            Lon = ((double)node1.Longitude + (double)node2.Longitude) / 2;
          }
          else {
            // open way with odd number of nodes
            Node node = stopNodes[nodeCount / 2];

            Lat = (double)node.Latitude;
            Lon = (double)node.Longitude;
          }
        }
      }

      ID = id;
      Code = code;
      Name = name;
      Desc = desc;
      Zone = zone;
      Url = url;
      Type = type;
      Wheelchairs = wheelchairs;
    }

    public static List<string> GetHeader() {
      List<string> ret = new List<string> {
        "stop_id",
        "stop_name",
        "stop_lat",
        "stop_lon",
        "location_type"
      };
      if (UseCodes) ret.Add("stop_code");
      if (UseDescs) ret.Add("stop_desc");
      if (UseZones) ret.Add("zone_id");
      if (UseUrls) ret.Add("stop_url");
      if (UseWheelchair) ret.Add("wheelchair_boarding");

      return ret;
    }

    public List<string> GetRow() {
      List<string> ret = new List<string> {
        ID,
        Name,
        Lat.ToString(),
        Lon.ToString(),
        Type.ToString(),
      };
      if (UseCodes) ret.Add(Code);
      if (UseDescs) ret.Add(Desc);
      if (UseZones) ret.Add(Zone);
      if (UseUrls) ret.Add(Url);
      if (UseWheelchair) ret.Add(Wheelchairs.ToString());

      return ret;
    }
  }

  public class StopFile {
    public static void Create(List<ICompleteOsmGeo> stops) {
      // For each network in the osm file, create a new Agency.
      List<Stop> stopList = new List<Stop>();
      foreach (var stop in stops) {
        stopList.Add(new Stop(stop));
      }

      // Create the output grid
      Grid<string> stopGrid = new Grid<string>();
      stopGrid.AddRow(Stop.GetHeader());

      // Add each stop - This has to be separate because some stops may
      // have optional fields and some may not; optional fields nobody
      // specifies get skipped.
      foreach (Stop stop in stopList) {
        stopGrid.AddRow(stop.GetRow());
      }

      // Output the file
      CSVParser.GridToFile(stopGrid, "gtfs/stops.txt");
    }
  }
}