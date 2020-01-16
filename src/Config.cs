using System.IO;
using Newtonsoft.Json;

namespace Nixill.GTFS {
  public class Config {
    private static ConfigObject Cfg;

    public static string TimeZone { get => Cfg.TimeZone; }
    public static string AgencyIDTag { get => Cfg.IDs.Agency; }
    public static string RouteIDTag { get => Cfg.IDs.Route; }
    public static string StopIDTag { get => Cfg.IDs.Stop; }

    public static void Load() {
      Cfg = JsonConvert.DeserializeObject<ConfigObject>(File.ReadAllText("settings.json"));
    }
  }

  public class ConfigObject {
    public string TimeZone;
    public IDTagList IDs;
  }

  public class IDTagList {
    public string Agency;
    public string Route;
    public string Stop;
  }
}