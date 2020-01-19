using System.IO;

namespace Nixill.GTFS.Objects {
  public class FileMerger {
    public static void Merge(string folder, string output) {
      bool isFirstFile = true;
      using (StreamWriter writer = new StreamWriter(output)) {
        foreach (string file in Directory.GetFiles(folder)) {
          bool isFirstLine = true;
          foreach (string line in File.ReadLines(file)) {
            if (!isFirstLine || isFirstFile) writer.WriteLine(line);
            isFirstLine = false;
          }
          isFirstFile = false;
        }
      }
    }

    // TODO DELETE THIS WHEN IT'S NO LONGER NEEDED
    public static void Merge(string folder, string output, int commas) {
      bool isFirstFile = true;
      using (StreamWriter writer = new StreamWriter(output)) {
        foreach (string file in Directory.GetFiles(folder)) {
          bool isFirstLine = true;
          foreach (string line in File.ReadLines(file)) {
            if (!isFirstLine || isFirstFile) {
              string outLine = line + new string(',', commas - CountCommas(line));
              writer.WriteLine(outLine);
            }
            isFirstLine = false;
          }
          isFirstFile = false;
        }
      }
    }

    private static int CountCommas(string input) {
      int count = 0;
      foreach (char chr in input) {
        if (chr == ',') count++;
      }
      return count;
    }
  }

  public class TripsFile {
    public static void Create() => FileMerger.Merge("trips", "gtfs/trips.txt", 6);
  }

  public class TimesFile {
    public static void Create() => FileMerger.Merge("trip-times", "gtfs/stop_times.txt");
  }

  public class ShapesFile {
    public static void Create() => FileMerger.Merge("shapes", "gtfs/shapes.txt");
  }
}