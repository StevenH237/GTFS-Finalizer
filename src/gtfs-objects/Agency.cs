using System.Collections.Generic;
using System;
using OsmSharp.Complete;
using OsmSharp.Tags;

namespace Nixill.GTFS.Objects
{
  public class Agency
  {
    public readonly string Name;
    public readonly string ID;
    public readonly Uri Url;
    public readonly string Phone;
    public readonly Uri FareUrl;
    public readonly string Email;

    public static string TimeZone { get; private set; }
    public static bool UsePhone { get; private set; } = false;
    public static bool UseFare { get; private set; } = false;
    public static bool UseEmail { get; private set; } = false;
    public static bool UseID { get => AgencyCount > 1; }
    private static int AgencyCount = 0;

    public Agency(CompleteRelation rel)
    {
      string name = null;
      string id = null;
      Uri url = null;
      string phone = null;
      Uri fareUrl = null;
      string email = null;

      foreach (Tag tag in rel.Tags)
      {
        if (tag.Key == "name") name = tag.Value;
        else if (tag.Key.StartsWith("name:") && name == null) name = tag.Value;
        else if (tag.Key == "network") id = tag.Value;
        else if (tag.Key == "website" || tag.Key == "contact:website") url = new Uri(tag.Value);
        else if (tag.Key == "phone" || tag.Key == "contact:phone")
        {
          phone = tag.Value;
          UsePhone = true;
        }
        else if (tag.Key == "website:fares" || tag.Key == "contact:website:fares")
        {
          fareUrl = new Uri(tag.Value);
          UseFare = true;
        }
        else if (tag.Key == "email" || tag.Key == "contact:email")
        {
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

    public static void SetTimeZone(string input)
    {
      TimeZone = input;
    }

    public List<string> GetRow()
    {
      List<string> ret = new List<string> { ID, Name, Url.ToString(), TimeZone.ToString() };
      if (UseEmail) ret.Add(Email);
      if (UseFare) ret.Add(FareUrl.ToString());
      if (UsePhone) ret.Add(Phone);

      return ret;
    }

    public static List<string> GetHeaderRow()
    {
      List<string> ret = new List<string> { "agency_id", "agency_name", "agency_url", "agency_timezone" };
      if (UseEmail) ret.Add("agency_email");
      if (UseFare) ret.Add("agency_fare_url");
      if (UsePhone) ret.Add("agency_phone");

      return ret;
    }
  }
}