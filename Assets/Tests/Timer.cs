using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tests {
	public class Timer {

		string name;

		private System.Diagnostics.Stopwatch stopwatch;
		private Dictionary<string, float> times;

		public float TotalTime {
			get {
				float total = 0;
				foreach (var pair in times)
					total += pair.Value;
				return total;
			}
		}

		public delegate void Process();

		public Timer(string name) {
			this.name = name;
			stopwatch = new System.Diagnostics.Stopwatch();
			times = new Dictionary<string, float>();
		}

		public void PrintTime(Process process, string label) {
			stopwatch.Restart();
			process();
			stopwatch.Stop();

			float ms = (float)stopwatch.ElapsedTicks / System.TimeSpan.TicksPerMillisecond;
			times[label] = ms;

			Debug.Log($"{(label != null ? $"[{label}] " : "")}Running time: {TimeToString(ms)}\n");
		}

		public void PrintTotalTime() {
			Debug.Log($"Total running time: {TimeToString(TotalTime)}\n");
		}

		public void SaveCsv() {
			string cvsDir = $"{Application.dataPath}/cvs";
			System.IO.Directory.CreateDirectory(cvsDir);

			var filePath = $"{cvsDir}/{name}.csv";
			var stream = new System.IO.StreamWriter(filePath);

			var labels = string.Join(",", times.Keys.Select(x => $"\"{x}\"").ToArray());
			var values = string.Join(",", times.Values.Select(x => $"\"{x}\"").ToArray());

			stream.WriteLine(string.Join(",", labels));
			stream.WriteLine(string.Join(",", values));

			stream.Close();

			Debug.Log($"Running times saved to {filePath}\n");
		}

		private static string TimeToString(float ms) {
			float value = ms;
			string unit = "ms";

			if (value > 1000) {
				value = Mathf.Round(value) / 1000;
				unit = "s";
			}

			return $"{value}{unit}";
		}
	}
}
