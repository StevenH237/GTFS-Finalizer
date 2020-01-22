using System.ComponentModel;
using System;
using System.Text;
using Nixill.Collections;
using System.IO;

namespace Nixill.GTFS.Objects {
  public class CalendarFile {
    public static void Create() {
      using (StreamWriter calTxt = new StreamWriter("gtfs/calendar.txt"))
      using (StreamWriter datesTxt = new StreamWriter("gtfs/calendar_dates.txt")) {
        calTxt.Write(CalendarHeader);
        datesTxt.Write(CalendarDatesHeader);

        // Write all primary calendars
        foreach (Calendar cal in Config.Calendars) {
          calTxt.Write(CalendarTxt(cal));
          datesTxt.Write(CalendarDatesTxt(cal));
        }

        // Write any blank calendars
        foreach (string str in Config.BlankCalendars) {
          calTxt.Write(CalendarTxt(str));
        }
      }
    }

    public const string CalendarHeader = "service_id,start_date,end_date,monday,tuesday,wednesday,thursday,friday,saturday,sunday\n";
    public const string CalendarDatesHeader = "service_id,date,exception_type\n";

    public static string CalendarTxt(Calendar input) {
      // First, we want to get the individual service IDs
      GeneratorDictionary<string, int> services = new GeneratorDictionary<string, int>(new SingleValueGenerator<string, int>(0));
      if (input.Monday != null) services[input.Monday] += 1;
      if (input.Tuesday != null) services[input.Tuesday] += 2;
      if (input.Wednesday != null) services[input.Wednesday] += 4;
      if (input.Thursday != null) services[input.Thursday] += 8;
      if (input.Friday != null) services[input.Friday] += 16;
      if (input.Saturday != null) services[input.Saturday] += 32;
      if (input.Sunday != null) services[input.Sunday] += 64;

      // Including those for which only exceptions exist
      foreach (string service in input.Exceptions.Values) {
        if (service != null) services[service] += 0; // This'll leave existing values unchanged but generate 0s for new values
      }

      StringBuilder ret = new StringBuilder();

      foreach (string service in services.Keys) {
        ret.Append(service + "," + input.Start.ToString("yyyyMMdd") + "," + input.End.ToString("yyyyMMdd") + ",");
        ret.Append(((services[service] & 1) == 1) ? "1," : "0,");
        ret.Append(((services[service] & 2) == 2) ? "1," : "0,");
        ret.Append(((services[service] & 4) == 4) ? "1," : "0,");
        ret.Append(((services[service] & 8) == 8) ? "1," : "0,");
        ret.Append(((services[service] & 16) == 16) ? "1," : "0,");
        ret.Append(((services[service] & 32) == 32) ? "1," : "0,");
        ret.Append(((services[service] & 64) == 64) ? "1\n" : "0\n");
      }

      return ret.ToString();
    }

    public static string CalendarTxt(string input) {
      return input + "," + Config.Today + "," + Config.Today + ",0,0,0,0,0,0,0\n";
    }

    public static string CalendarDatesTxt(Calendar input) {
      StringBuilder ret = new StringBuilder();
      foreach (DateTime key in input.Exceptions.Keys) {

        // For each exception...
        // First, figure out what day of the week it was
        DayOfWeek dow = key.DayOfWeek;

        // And what service we need to remove
        // It might be null
        string remove = dow switch
        {
          DayOfWeek.Monday => input.Monday,
          DayOfWeek.Tuesday => input.Tuesday,
          DayOfWeek.Wednesday => input.Wednesday,
          DayOfWeek.Thursday => input.Thursday,
          DayOfWeek.Friday => input.Friday,
          DayOfWeek.Saturday => input.Saturday,
          DayOfWeek.Sunday => input.Sunday,
          _ => null
        };

        // It should be null if it's outside the range, too
        if (key < input.Start || key > input.End) remove = null;

        // 2 means removed
        if (remove != null) ret.AppendLine(remove + "," + key.ToString("yyyyMMdd") + ",2");

        // And what service we need to add instead
        // Which may also be null
        string add = input.Exceptions[key];

        // 1 means added
        if (add != null) ret.AppendLine(add + "," + key.ToString("yyyyMMdd") + ",1");
      }

      return ret.ToString();
    }
  }
}