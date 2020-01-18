using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections.Generic;
using System;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;
using OsmSharp.Complete;
using OsmSharp.Tags;
using Nixill.Utils;

namespace Nixill.GTFS.Objects {
  public class RoutesFile {
    public static void Create(Dictionary<long, RouteRelation>.ValueCollection routes) {
      // For each network in the osm file, create a new Agency.
      List<Route> routeList = new List<Route>();
      foreach (var route in routes) {
        routeList.Add(new Route(route));
      }

      // Create the output grid
      Grid<string> routeGrid = new Grid<string>();
      routeGrid.AddRow(Route.GetHeaderRow());

      // Add each route - This has to be separate because some routes may
      // have optional fields and some may not; optional fields nobody
      // specifies get skipped.
      foreach (Route route in routeList) {
        routeGrid.AddRow(route.GetRow());
      }

      // Output the file
      CSVParser.GridToFile(routeGrid, "gtfs/routes.txt");
    }
  }

  public class Route {
    public readonly string ID;
    public readonly string ParentAgency;
    public readonly string ShortName;
    public readonly string LongName;
    public readonly int Type;
    public readonly string Desc;
    public readonly string Url;
    public readonly string Color;
    public readonly string TextColor;
    public readonly int SortOrder;

    private static bool UseShortName = false;
    private static bool UseLongName = false;
    private static bool UseDesc = false;
    private static bool UseUrl = false;
    private static bool UseColors = false;

    public Route(RouteRelation rtRel) {
      CompleteRelation rel = rtRel.Route;
      Agency parentAgency = rtRel.ParentAgency;
      int sortOrder = rtRel.Order;
      // we don't use Operates here, that's only for making the list

      string id = null;
      string agency = parentAgency.ID;
      string shortName = null;
      string longName = null;
      int type = -1;
      string desc = null;
      string url = null;
      string color = null;
      string textColor = null;
      // sortOrder is declared above

      if (Config.RouteIDTag == null) id = Util.OsmIDString(rel);

      foreach (Tag tag in rel.Tags) {
        if (tag.Key == Config.RouteIDTag) id = Config.PatternExtract(tag.Value, Config.RouteIDPattern);

        if (tag.Key == "name") {
          longName = Config.PatternExtract(tag.Value, Config.RouteNamePattern);
          UseLongName = true;
        }
        if (tag.Key.StartsWith("name:") && longName == null) {
          longName = tag.Value;
          UseLongName = true;
        }
        if (tag.Key == "ref") {
          shortName = tag.Value;
          UseShortName = true;
        }
        if (tag.Key == "route_master")
          type = tag.Value switch
          {
            "monorail" => 0,
            "tram" => 0,
            "subway" => 1,
            "train" => 2,
            "bus" => 3,
            "trolleybus" => 3,
            "ferry" => 4,
            "aerialway" => 6,
            _ => 0
          };
        if (tag.Key == "desc") {
          desc = tag.Value;
          UseDesc = true;
        }
        if (tag.Key == "url") {
          url = tag.Value;
          UseUrl = true;
        }
        if (tag.Key == "color" || tag.Key == "colour") {
          if (Regex.IsMatch(tag.Value, @"\#[0-9a-fA-F]{6}")) {
            color = tag.Value.Substring(1);
            int value = Convert.ToInt32(color, 16);
            int red = value / 65536;
            int green = value % 65536 / 256;
            int blue = value % 256;
            int sum = red + green + blue;
            textColor = (sum >= (256 + 128)) ? "000000" : "ffffff";
            UseColors = true;
          }
        }

      }

      ID = id;
      ParentAgency = agency;
      ShortName = shortName;
      LongName = longName;
      Type = type;
      Desc = desc;
      Url = url;
      Color = color;
      TextColor = textColor;
      SortOrder = sortOrder;
    }

    public List<string> GetRow() {
      List<string> ret = new List<string> {
        ID,
        ParentAgency,
        Type.ToString(),
        SortOrder.ToString()
      };
      if (UseShortName) ret.Add(ShortName);
      if (UseLongName) ret.Add(LongName);
      if (UseDesc) ret.Add(Desc);
      if (UseUrl) ret.Add(Url);
      if (UseColors) {
        ret.Add(Color);
        ret.Add(TextColor);
      }

      return ret;
    }

    public static List<string> GetHeaderRow() {
      List<string> ret = new List<string> {
        "route_id",
        "agency_id",
        "route_type",
        "route_sort_order"
      };

      if (UseShortName) ret.Add("route_short_name");
      if (UseLongName) ret.Add("route_long_name");
      if (UseDesc) ret.Add("route_desc");
      if (UseUrl) ret.Add("route_url");
      if (UseColors) {
        ret.Add("route_color");
        ret.Add("route_text_color");
      }

      return ret;
    }
  }

  public struct RouteRelation {
    public CompleteRelation Route;
    public Agency ParentAgency;
    public bool Operates;
    public int Order;
  }
}