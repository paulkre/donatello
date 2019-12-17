using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking
{
    using Debugging;
    using Audio;
    using MarkerManagement;
    using Calibration;

    [RequireComponent(typeof(OptitrackStreamingClient))]
    public class FingerTrackingMaster : MonoBehaviour
    {


        public List<TrackedHand> trackedHands;
        public bool showDebugView;

        public AudioPlayer audioPlayerPrefab;
        public DebugView debugViewPrefab;
        public DebugView debugView;
        
        public static FingerTrackingMaster Instance;
        public Calibrator Calibrator { get; private set; }
        public OptitrackStreamingClient StreamingClient { get; private set; }
        public MarkerAssigner MarkerAssigner { get; private set; }
        public AudioPlayer AudioPlayer { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            AudioPlayer = Instantiate(audioPlayerPrefab);
            AudioPlayer.Init();


            StreamingClient = GetComponent<OptitrackStreamingClient>();
            if (StreamingClient != null)
            {
                MarkerAssigner = new MarkerAssigner(trackedHands.ToArray());
                Calibrator = new Calibrator(MarkerAssigner, trackedHands.ToArray());
            }

            WindowsVoice.Speak("Fingertracking gestartet!");

            StartCoroutine(TryLoad());
        }


        void Update()
        {
            KeyControls();

            if (debugView == null && debugViewPrefab != null && showDebugView )
            {
                debugView = Instantiate(debugViewPrefab);
                debugView.Init();
            }
        }

        public void RegisterHand(TrackedHand hand)
        {
            //TODO implement
            //trackedHands.Add(hand);
        }

        private void KeyControls()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("clear meta");
                MarkerAssigner.PrepareForSphericalAnalysisMeta();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("add meta data");
                for (int i = 0; i < trackedHands.Count; i++) // i = hand
                {
                    for (int j = 0; j < 5; j++) // j = finger
                    {
                        MarkerAssigner.AddDataForSphericalAnalysisMeta(trackedHands[i].markers[2 * j].GetCurrentPosition(), i, j);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("calculate meta");
                MarkerAssigner.CalculateSphericalAnalysisMeta();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("calculate length");
                foreach (TrackedHand hand in trackedHands)
                {
                    hand.CalibrateLength();
                    hand.SetCalibrationPose();
                }
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                foreach (TrackedHand hand in trackedHands)
                {
                    Debug.Log($"length error: {hand.CalculateLengthError().ToString("N5")}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("save config");
                ConfigManager.SaveConfig();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("load config");
                ConfigManager.LoadConfig();
            }

            if (Input.GetKeyDown(KeyCode.C))
                StartCoroutine(Calibrator.Calibrate());

            if (showDebugView && Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (debugView.gameObject.activeSelf)
                    debugView.gameObject.SetActive(false);
                else
                    debugView.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.B))
                debugView.drawLines = !debugView.drawLines;
        }

        IEnumerator TryLoad()
        {
            print("trying to load data");
            yield return new WaitForSeconds(1f);
            try
            {
                ConfigManager.LoadConfig();
                WindowsVoice.Speak("Ladevorgang erfolgreich!");
                print("success loading data");
                AudioPlayer.PlaySound(AudioType.success);
                MarkerAssigner.TrackingEnabled = true;
            }
            catch
            {
                WindowsVoice.Speak("Ladevorgang fehlgeschlagen");
                print("failed to load data");
                AudioPlayer.PlaySound(AudioType.fail);
            }
        }


    }

}

