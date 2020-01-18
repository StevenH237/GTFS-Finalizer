using OsmSharp;
using OsmSharp.Complete;

namespace Nixill.GTFS {
  public class Util {
    public static string OsmIDString(ICompleteOsmGeo geo) {
      if (geo is Node) {
        return "n" + geo.Id;
      }
      else if (geo is CompleteWay) {
        return "w" + geo.Id;
      }
      else if (geo is CompleteRelation) {
        return "r" + geo.Id;
      }
      else return "x";
    }
  }
}