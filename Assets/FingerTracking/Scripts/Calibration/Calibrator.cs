using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.Calibration
{
    using Audio;
    using MarkerManagement;

    public class Calibrator
    {
        private MarkerAssigner markerAssigner;
        private TrackedHand[] hands;

        private List<Vector3> markerPositions;
        private List<Vector3> localPositions;

        private long timeStamp;

        public static Calibrator Instance;

        public Calibrator(MarkerAssigner markerAssigner, TrackedHand[] hands)
        {
            this.markerAssigner = markerAssigner;
            this.hands = hands;

            markerPositions = new List<Vector3>(128);
            localPositions = new List<Vector3>(128);

            Instance = this;
        }

        public IEnumerator Calibrate()
        {

            foreach (var h in hands)
                h.calibrated = false;

            var player = FingerTrackingMaster.Instance.AudioPlayer;

            WindowsVoice.Speak($"Kalibrierung gestartet!");

            Debug.Log("Solo Calibration 2started");
            //check
            player.PlaySound(AudioType.success);
            yield return new WaitForSeconds(2.0f);
            Debug.Log("Next Pose: <b>Detection</b>");
            WindowsVoice.Speak("Detektion: 3,2,1");
            yield return new WaitForSeconds(2.5f);
            for (int i = 0; i < 3; i++)
            {
                Debug.Log((3 - i) + "s");
                yield return new WaitForSeconds(1.0f);
            }

            MarkerAssignToHandsComplete();

            player.PlaySound(AudioType.accept);

            Debug.Log("Next Pose: <b>Tight</b>");
            WindowsVoice.Speak("Anliegend: 3,2,1");
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < 3; i++)
            {
                Debug.Log((3 - i) + "s");
                yield return new WaitForSeconds(1.0f);
            }

            foreach (var h in hands)
                h.SavePose(1);

            player.PlaySound(AudioType.accept);

            WindowsVoice.Speak("Winkel: 3,2,1");
            Debug.Log("Next Pose: <b>Angle</b>");
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < 3; i++)
            {
                Debug.Log((3 - i) + "s");
                yield return new WaitForSeconds(1.0f);
            }

            foreach (var h in hands)
                h.SavePose(2);

            player.PlaySound(AudioType.accept);

            Debug.Log("Next Pose: <b>Wide</b>");
            WindowsVoice.Speak("Offen: 3,2,1");
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < 3; i++)
            {
                Debug.Log((3 - i) + "s");
                yield return new WaitForSeconds(1.0f);
            }

            foreach (var h in hands)
            {
                //save last pose (wide)
                h.SavePose(3);

                //do calculations (lengeth, positions, etc.)
                MarkerAssignToHands(h);
                h.CalculateMetasFromPoses();
                h.CalibrateLength();
                h.SetCalibrationPose();

                //h.SavePose(0);
                
                //define parents
                h.DefineParents();
            }

            player.PlaySound(AudioType.complete);


            Debug.Log("Done. Have Fun!");
            ConfigManager.SaveConfig();

            foreach (var hand in hands) hand.Calibrated = true;
        }

        private void MarkerAssignToHandsComplete()
        {
            foreach (TrackedHand hand in hands)
            {
                MarkerAssignToHands(hand);
            }

            markerAssigner.TrackingEnabled = true;
        }

        public void MarkerAssignToHands(TrackedHand hand)
        {
            LoadLocalMarkerPositions(hand);

            OptitrackMarker[] currentHandFingerMarkers = hand.markers;


            //TODO: check for plausibility
            //
            //
            //

            int count = localPositions.Count;

            if (localPositions.Count < 10)
            {
                Debug.Log(
                    $"!!Not enough markers! -> local markers: {localPositions.Count}, all markers: {count}"
                );
                return;
            }

            //sort by angle, always starting with thumb
            if (hand.side == TrackedHand.Side.left)
                localPositions.Sort(
                    (v1, v2) =>
                        TransformPositionToSpherical(v2).x.CompareTo(
                            TransformPositionToSpherical(v1).x
                        )
                );
            else
                localPositions.Sort(
                    (v1, v2) => 
                        TransformPositionToSpherical(v1).x.CompareTo(
                            TransformPositionToSpherical(v2).x
                        )
                );


            //Create finger markers
            for (int i = 0; i < 10; i++)
                currentHandFingerMarkers[i].CalibrationInit(localPositions[i]);

            //sort by distance
            for (int i = 0; i < 5; i++)
            {
                float distance1 = TransformPositionToSpherical(currentHandFingerMarkers[2 * i].GetCurrentPosition()).z;
                float distance2 = TransformPositionToSpherical(currentHandFingerMarkers[2 * i + 1].GetCurrentPosition()).z;

                if (distance1 > distance2)
                {
                    OptitrackMarker help = currentHandFingerMarkers[2 * i];
                    currentHandFingerMarkers[2 * i] = currentHandFingerMarkers[2 * i + 1];
                    currentHandFingerMarkers[2 * i + 1] = help;
                }
            }

        }

        private void LoadLocalMarkerPositions(TrackedHand hand)
        {
            UpdateMarkerPositions();

            localPositions.Clear();

            foreach (var p in markerPositions)
            {
                Vector3 localPosition = hand.TransformWorldToLocalSpace(p);
                if (localPosition.magnitude < 0.2f)
                    localPositions.Add(localPosition);
            }
        }

        private void UpdateMarkerPositions()
        {
            var streamingClient = FingerTrackingMaster.Instance.StreamingClient;

            if (timeStamp >= streamingClient.TimeStamp) return;
            timeStamp = streamingClient.TimeStamp;

            streamingClient.ReadMarkerPositions(markerPositions);
        }

        private static Vector3 TransformPositionToSpherical(Vector3 point)
        {
            // (1/3): r
            float r = point.magnitude;

            // (2/3): phi
            float phi = -1;

            if (point.x == 0)
            {
                //y-axis
                if (point.z > 0)
                {
                    //forward
                    phi = Mathf.PI;
                }
                else
                {
                    //backwards
                    phi = 0;
                }
            }
            else if (point.x < 0 && point.z < 0)
            {
                phi = -1 * Mathf.PI / 2f - Mathf.Atan2(point.z, point.x);
            }
            else
            {
                phi = 3 * Mathf.PI / 2f - Mathf.Atan2(point.z, point.x);
            }

            // (3/3): theta
            float theta = Mathf.Asin(point.y / r);


            return new Vector3(phi, theta, r);
        }

    }

}
