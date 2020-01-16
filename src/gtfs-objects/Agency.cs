using System.Collections.Generic;
using System;
using Nixill.Collections.Grid;
using Nixill.Collections.Grid.CSV;
using OsmSharp.Complete;
using OsmSharp.Tags;

namespace Nixill.GTFS.Objects {
  public class AgencyFile {
    public static void Create(List<CompleteRelation> agencies) {
      // For each network in the osm file, create a new Agency.
      List<Agency> agencyList = new List<Agency>();
      foreach (var agency in agencies) {
        agencyList.Add(new Agency(agency));
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
    public static bool UseID { get => AgencyCount > 1; }
    private static int AgencyCount = 0;

    public Agency(CompleteRelation rel) {
      string name = null;
      string id = null;
      string url = null;
      string phone = null;
      string fareUrl = null;
      string email = null;

      foreach (Tag tag in rel.Tags) {
        if (tag.Key == Config.AgencyIDTag) id = tag.Value;

        if (tag.Key == "name") name = tag.Value;
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
  }
}