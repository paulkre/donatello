using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.Calibration
{
    public static class ConfigManager
    {
        private static string filename = "handsConfig.hnd";

        public static void SaveConfig()
        {
            var hands = FingerTrackingMaster.Instance.trackedHands;

            /*
            //Json
            string jSave = JsonUtility.ToJson(FingerTrackingMaster.Instance.trackedHands, true);

            File.WriteAllText("config.hnd", jSave);
            */
            
            BinaryFormatter bf = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.OpenOrCreate);

            List<TrackedHandData> list = new List<TrackedHandData>();
            foreach (TrackedHand th in hands)
                list.Add(th.GetData());
            
            bf.Serialize(stream, list);
            stream.Close();
            
        }

        public static void LoadConfig()
        {
            var hands = FingerTrackingMaster.Instance.trackedHands;
            /*
            string jLoad = File.ReadAllText("config.hnd");
            FingerTrackingMaster.Instance.trackedHands = JsonUtility.FromJson<List<TrackedHand>>(jLoad);
            */
            
            BinaryFormatter bf = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open);

            List<TrackedHandData> list = bf.Deserialize(stream) as List<TrackedHandData>;
            stream.Close();

            for (int i = 0; i < list.Count; i++)
            {
                hands[i].SetData(list[i]);
                hands[i].FinalizeCalibration();
                Calibrator.Instance.MarkerAssignToHands(hands[i]);
            }
            
        }
    }
}

