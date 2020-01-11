using System.IO;
using System;

namespace Nixill.GTFS {
  public class FinalizerMain {
    static void Main(string[] args) {
      string path = null;

      if (args.Length > 0) path = args[0];
      else {
        Console.Write("Select a folder: ");
        path = Console.ReadLine();
      }

      Directory.SetCurrentDirectory(path);

      // Make sure we have directories
      
    }
  }
}