using System.Collections.Generic;
using System;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;
using OsmSharp.Complete;
using OsmSharp.Tags;

namespace Nixill.GTFS.Objects {
  public class AgencyFile {
    public static void Create(List<CompleteRelation> agencies, Dictionary<long, RouteRelation> routes) {
      // For each network in the osm file, create a new Agency.
      List<Agency> agencyList = new List<Agency>();
      foreach (var agency in agencies) {
        agencyList.Add(new Agency(agency, routes));
      }

      // Create the output grid
      Grid<string> agencyGrid = new Grid<string>();
      agencyGrid.AddRow(Agency.GetHeaderRow());

      // Add each agency - This has to be separate because some agencies
      // may have optional fields and some may not; optional fields nobody
      // specifies get skipped.
      foreach (Agency agency in agencyList) {
        agencyGrid.AddRow(agency.GetRow());
      }

      // Output the file
      CSVParser.GridToFile(agencyGrid, "gtfs/agency.txt");
    }
  }

  public class Agency {
    public readonly string Name;
    public readonly string ID;
    public readonly string Url;
    public readonly string Phone;
    public readonly string FareUrl;
    public readonly string Email;

    public static bool UsePhone { get; private set; } = false;
    public static bool UseFare { get; private set; } = false;
    public static bool UseEmail { get; private set; } = false;

    public Agency(CompleteRelation rel, Dictionary<long, RouteRelation> routes) {
      string name = null;
      string id = null;
      string url = null;
      string phone = null;
      string fareUrl = null;
      string email = null;

      if (Config.AgencyIDTag == null) id = Util.OsmIDString(rel);

      foreach (Tag tag in rel.Tags) {
        if (tag.Key == Config.AgencyIDTag) id = Config.PatternExtract(tag.Value, Config.AgencyIDPattern);

        if (tag.Key == Config.AgencyNameTag) name = Config.PatternExtract(tag.Value, Config.AgencyNamePattern);
        else if (tag.Key.StartsWith("name:") && name == null) name = tag.Value;
        else if (tag.Key == "website" || tag.Key == "contact:website") url = tag.Value;
        else if (tag.Key == "phone" || tag.Key == "contact:phone") {
          phone = tag.Value;
          UsePhone = true;
        }
        else if (tag.Key == "website:fares" || tag.Key == "contact:website:fares") {
          fareUrl = tag.Value;
          UseFare = true;
        }
        else if (tag.Key == "email" || tag.Key == "contact:email") {
          email = tag.Value;
          UseEmail = true;
        }
      }

      Name = name;
      ID = id;
      Url = url;
      Phone = phone;
      FareUrl = fareUrl;
      Email = email;

      AddRoutes(routes, this, rel);
    }

    public List<string> GetRow() {
      List<string> ret = new List<string> { ID, Name, Url.ToString(), Config.TimeZone };
      if (UseEmail) ret.Add(Email);
      if (UseFare) ret.Add(FareUrl.ToString());
      if (UsePhone) ret.Add(Phone);

      return ret;
    }

    public static List<string> GetHeaderRow() {
      List<string> ret = new List<string> { "agency_id", "agency_name", "agency_url", "agency_timezone" };
      if (UseEmail) ret.Add("agency_email");
      if (UseFare) ret.Add("agency_fare_url");
      if (UsePhone) ret.Add("agency_phone");

      return ret;
    }

    public void AddRoutes(Dictionary<long, RouteRelation> routes, Agency agency, CompleteRelation aRel) {
      int i = 0;
      foreach (CompleteRelationMember mem in aRel.Members) {
        i++; // yes, it'll start at 1 the first time around
        if (mem.Member is CompleteRelation rel && rel.Tags != null && rel.Tags.Contains("type", "route_master")) {
          if (routes.ContainsKey(rel.Id)) {
            // Route is already listed
            if (routes[rel.Id].Operates && (mem.Role != "operates")) {
              // ... but is only "operated by" the agency it's listed under
              routes[rel.Id] = new RouteRelation { ParentAgency = agency, Operates = false, Route = rel, Order = i };
            }
          }
          else {
            // Route isn't already listed
            routes[rel.Id] = new RouteRelation { ParentAgency = agency, Operates = mem.Role == "operates", Route = rel, Order = i };
          }
        }
      }
    }
  }
}