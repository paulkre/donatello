using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace FingerTracking.Calibration
{
    public static class ConfigManager
    {
        private static string filename = "handsConfig.hnd";

        public static void SaveConfig()
        {
            var hands = FingerTrackingMaster.Instance.trackedHands;

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

            BinaryFormatter bf = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open);

            List<TrackedHandData> list = bf.Deserialize(stream) as List<TrackedHandData>;
            stream.Close();

            for (int i = 0; i < list.Count; i++)
                hands[i].SetData(list[i]);
        }
    }
}

