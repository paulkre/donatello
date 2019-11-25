using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.MachineLearning
{

    public class Recorder : MonoBehaviour
    {
        public bool gui = true;
        private string fileName = "recording";

        string recordString;

        bool recording = false;

        List<string> recordList;
        // Start is called before the first frame update
        void Start()
        {
            recordList = new List<string>();
        }

        // Update is called once per frame
        void Update()
        {
            if (recording)
            {
                DoRecord();
            }
        }

        void SaveRecord()
        {
            if (!System.IO.Directory.Exists("ML_recordings"))
                System.IO.Directory.CreateDirectory("ML_recordings");

            recordString = "";
            foreach (string s in recordList)
            {
                recordString += s + "\n";
            }

            string file = $"ML_recordings/{System.DateTime.Now.ToString().Replace(' ', '_').Replace('.', '-').Replace(':', '-')}_{fileName}.mlr";

            System.IO.File.WriteAllText(file, recordString);

            print("saved:\n" + recordString);
        }

        void DoRecord()
        {
            recordList.Add(GenerateMLString());
        }

        void StartRecord()
        {
            recordList.Clear();
            recording = true;
        }

        void StopRecord()
        {
            recording = false;
        }

        private string GenerateMLString()
        {
            string s = "";

            bool first = true;
            foreach (var h in FingerTrackingMaster.Instance.trackedHands)
            {
                if (first) first = false;
                else s += "|";

                s += h.GetMLString();
            }

            return s;
        }

        private void OnGUI()
        {
            if (gui)
            {
                GUI.Box(new Rect(5, 5, 410, 205), "");
                GUI.BeginGroup(new Rect(10, 10, 400, 200));
                if (GUI.Button(new Rect(0, 0, 100, 25), "Rec"))
                {
                    StartRecord();
                }
                if (GUI.Button(new Rect(105, 0, 100, 25), "Stop"))
                {
                    StopRecord();
                    SaveRecord();
                }
                fileName = GUI.TextField(new Rect(0, 30, 400, 25), fileName);
                GUI.EndGroup();
            }
        }
    }

}
