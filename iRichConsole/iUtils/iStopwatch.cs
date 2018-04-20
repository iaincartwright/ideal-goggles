using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iUtils
{
  public class iStopwatch
  {
	  readonly string _name;

    long _pulseStart;
    long _pulseStop;

	  readonly Stopwatch _stopWatch;

	  readonly List<double> _pulses;
	  readonly List<int> _data;

    int _skipCount = 0;

    public iStopwatch(string a_name) :
      this(a_name, 0)
    {
    }

    public iStopwatch(string a_name, int a_skipCount)
    {
      _name = a_name;

      _pulses = new List<double>();
      _data = new List<int>();

      _skipCount = a_skipCount;

      _stopWatch = new Stopwatch();
    }

    public void PulseStart()
    {
      if (_skipCount > 0)
        return;

      _pulseStart = _stopWatch.ElapsedTicks;

      _stopWatch.Start();
    }

    public void PulseStop()
    {
      PulseStop(0);
    }

    public void PulseStop(int a_datum)
    {
      _stopWatch.Stop();

      if (_skipCount > 0)
      {
        _skipCount--;
      }
      else
      {
        _pulseStop = _stopWatch.ElapsedTicks;

        _pulses.Add((double)(_pulseStop - _pulseStart));
        _data.Add(a_datum);
      }
    }

    public string ReportString()
    {
      int count = _pulses.Count;

      double toSeconds = (1.0 / (double)Stopwatch.Frequency);
      double toMillis = toSeconds * 1000.0;

      double seconds = _pulses.Sum() * toSeconds;
      double millis = _pulses.Sum() * toMillis;

      string report = String.Format("{2,20} : {0:N0} updates took {1:N3} seconds\n", count, seconds, _name);

      if (count > 1)
      {
        report += String.Format("{3,20} : Max {0:N3} ms Min {1:N3} ms Mean {2:N3} ms \n",
            _pulses.Max() * toMillis,
            _pulses.Min() * toMillis,
            _pulses.Average() * toMillis, _name);
      }

      return report;
    }

    public void Report()
    {
      Console.WriteLine(ReportString());
    }
  }
}