using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FingerTracking.Debugging
{

    public class DebugView : MonoBehaviour
    {
        public Text UI_rawMarkers;
        public Text UI_timings;
        public Dropdown handSelect;
        public GameObject bars;
        public Text UI_hugeText;
        public RawImage plotterImage;

        public bool drawLines = true;

        private int lastAssigns = 0;

        private long timeStamp;
        private List<Vector3> markerPositions;

        private List<int> optitrackIds;

        // Start is called before the first frame update
        public void Init()
        {
            markerPositions = new List<Vector3>(128);
            optitrackIds = new List<int>(128);

            UpdateHands();
            CreatePlotter(10);
        }

        void Update()
        {

            if (drawLines) DrawLines();

            UI_hugeText.text = GenerateDebugString();

            UpdateRawMarkers();
        }

        private void DrawLines()
        {
            var firstHand = FingerTrackingMaster.Instance.trackedHands[0];
            var assigner = FingerTrackingMaster.Instance.MarkerAssigner;

            UpdateBarPanel(assigner.MarkerQualities);
            AddLineValue(0, firstHand.lengthError * 100f, Color.blue);
            AddLineValue(1, firstHand.calibrationPoseMSE * 10f, Color.cyan);
            UpdateImage();
        }

        private string GenerateDebugString()
        {
            var assigner = FingerTrackingMaster.Instance.MarkerAssigner;
            var hands = FingerTrackingMaster.Instance.trackedHands;

            var currentAssigns = assigner.Assigns - lastAssigns;
            lastAssigns = assigner.Assigns;

            string debugS = $"DFPS:" + Mathf.RoundToInt(1f / Time.deltaTime).ToString("D5") + "\n";

            debugS += string.Format(
                "PULL:{0,-10:D5}\nSUCC:{1,-10:D5}\nASGN:{2,-10:D5}\n",
                0,
                0,
                currentAssigns
            );

            int t0 = 0, t1 = 0;
            assigner.GetAssignTimings(ref t0, ref t1);
            debugS += "MAT0:" + string.Format("{0,-8:D5}", t0) + "\nMAT1:" + string.Format("{0,-8:D5}\n\n", t1);

            UI_timings.text = debugS;

            debugS = $"hand count: {hands.Count}";

            foreach (TrackedHand hand in hands)
            {
                debugS += $"hand (rbid {hand.rigidBodyID}) -> calibrated:{hand.calibrated} \n";
                debugS += $"\tcalibrationPose-mse: {hand.calibrationPoseMSE.ToString("N5")}\n";
                debugS += $"\ttracked-mrks: {hand.TrackedMarkerCount().ToString("D2")}\n";
                debugS += $"\tlength-error: {hand.CalculateLengthError().ToString("N5")}\n";
                debugS += $"\tassign-error: {hand.CalculateAssignError().ToString("N5")}\n";
            }

            return debugS;
        }

        public void UpdateRawMarkers()
        {
            UpdateMarkerPositions();
            var pos = markerPositions;
            var ids = optitrackIds;

            string s = "id          x         y         z\n";

            for (int i = 0; i < ids.Count; i++)
                s += $"{ids[i]}:\t{pos[i].x.ToString("0.000")}\t{pos[i].y.ToString("0.000")}\t{pos[i].z.ToString("0.000")}\n";

            UI_rawMarkers.text = s;
        }

        public void UpdateMarkerPositions()
        {
            var streamingClient = FingerTrackingMaster.Instance.StreamingClient;

            if (timeStamp >= streamingClient.TimeStamp) return;
            timeStamp = streamingClient.TimeStamp;

            streamingClient.ReadMarkerPositions(markerPositions);
        }

        Plotter plotter;
        public void CreatePlotter(int count)
        {
            plotter = new Plotter(1900, 200, Color.black, 512);
            for (int i = 0; i < count; i++)
            {
                plotter.RegisterNewLine();
            }
        }

        public void AddLineValue(int line, float v, Color c)
        {
            plotter.AddDataLine(v, c, line);
        }

        public void UpdateImage()
        {
            plotter.UpdateImage();
            plotterImage.texture = plotter.image;
        }

        public void UpdateHands()
        {
            var hands = FingerTrackingMaster.Instance.trackedHands;

            handSelect.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>(hands.Count);

            for (int i = 0; i < hands.Count; i++)
                options.Add(new Dropdown.OptionData(i + ": " + hands[i].gameObject.name));

            handSelect.AddOptions(options);
        }

        public int GetCurrentHandSelection()
        {
            return handSelect.value;
        }

        public void UpdateBarPanel(float[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                float f = 0.01f + 0.99f * Mathf.Sqrt(values[i] / 0.1f);
                bars.transform.GetChild(i).GetComponent<RectTransform>().localScale = 0.8f * bars.transform.GetChild(i).GetComponent<RectTransform>().localScale + 0.2f * new Vector3(f, 1, 1);
                bars.transform.GetChild(i).GetComponent<Image>().color = Tools.Colors.RedYellowRed(1f - f);
            }
        }

        private void OnGUI()
        {
            //GUI.DrawTexture(new Rect(10, Screen.height - plotter.image.height -10, Screen.width-20, plotter.image.height), plotter.image);
        }

    }

}