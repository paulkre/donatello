using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.MarkerManagement
{

    public class MarkerAssigner
    {
        const int SIZE = 512;
        const float DISTANCE_THRESHOLD = 0.03f;

        public bool TrackingEnabled { get; set; }
        public int Assigns { get; private set; }

        private TrackedHand[] hands;

        private long timeStamp;

        private List<Vector3> markerPositions;

        public MarkerAssigner(TrackedHand[] hands)
        {
            this.hands = hands;

            matchList = new List<Match>(SIZE * SIZE);
            markerPositions = new List<Vector3>(256);

            assigning = true;
            new Thread(
                new ThreadStart(AssignLoop)
            ).Start();
        }

        private bool assigning;

        private void AssignLoop()
        {
            var streamingClient = FingerTrackingMaster.Instance.StreamingClient;

            while (assigning)
            {
                if (TrackingEnabled)
                {
                    if (timeStamp < streamingClient.TimeStamp)
                    {
                        timeStamp = streamingClient.TimeStamp;

                        streamingClient.ReadMarkerPositions(markerPositions);
                        MatchPositionsWithMarkers();
                        Thread.Sleep(1);

                        Assigns++;
                    }
                }
                else Thread.Sleep(100);
            }
        }

        public float[] MarkerQualities
        {
            get
            {
                if (activeMarkers == null) return new float[] { };

                float[] qualities = new float[activeMarkers.Length];

                float sum = 0;

                for (int i = 0; i < activeMarkers.Length; i++)
                {
                    float q = activeMarkers[i].assignQuality;
                    sum += q;
                    qualities[i] = q;
                }

                if (sum == 0)
                    return new float[] { };


                return qualities;
            }
        }

        public List<Match> LastMatches
        {
            get
            {
                lock (matchList)
                {
                    return matchList;
                }
            }
        }


        public int debug_time0, debug_time1;
        System.Diagnostics.Stopwatch debugStopwatch = new System.Diagnostics.Stopwatch();

        public void GetAssignTimings(ref int timing0, ref int timing1)
        {
            timing0 = debug_time0;
            timing1 = debug_time1;
        }


        OptitrackMarker[] activeMarkers;
        List<Match> matchList;

        private bool[] markerTaken = new bool[SIZE];
        private bool[] positionTaken = new bool[SIZE];

        public bool processing = false;
        object lockObj = new object();

        public void MatchPositionsWithMarkers()
        {
            debugStopwatch.Restart();
            processing = true;
            lock (lockObj)
            {

                //Prepare
                matchList.Clear();

                float minValue;
                int minIndex;

                for (int j = 0; j < markerPositions.Count; j++)
                {
                    positionTaken[j] = false;
                }

                // Do
                //first pass assign marker for every hand
                foreach (TrackedHand hand in hands)
                {
                    hand.UpdateWorldToLocalMatrix_OUT();
                    activeMarkers = hand.GetMarkers();

                    for (int i = 0; i < activeMarkers.Length; i++)
                    {
                        activeMarkers[i].assignedInLastFrame = false;
                        markerTaken[i] = false;
                    }

                    int matches = 0;

                    for (int j = 0; j < activeMarkers.Length; j++)
                    {
                        minValue = float.MaxValue;
                        minIndex = -1;

                        for (int i = 0; i < markerPositions.Count; i++)
                        {
                            float v = activeMarkers[j].GetRatingForMatch(hand.TransformWorldToLocalSpace(markerPositions[i]));

                            if (v / DISTANCE_THRESHOLD < 1f)
                            {
                                matches++;
                                minValue = v;
                                minIndex = i;

                                matchList.Add(new Match(hand, activeMarkers[j], i, hand.TransformWorldToLocalSpace(markerPositions[i]), v));
                            }
                        }
                    }
                }

                debug_time0 = (int)debugStopwatch.ElapsedTicks;
                //second pass, sort and assign by quality priority
                matchList.Sort((x, y) => x.quality.CompareTo(y.quality));

                for (int i = 0; i < matchList.Count; i++)
                {
                    if (!matchList[i].marker.assignedInLastFrame)
                    {
                        if (!positionTaken[matchList[i].indexPosition])
                        {
                            matchList[i].marker.UpdatePosition(matchList[i].position, matchList[i].quality);

                            matchList[i].marker.assignedInLastFrame = true;
                            positionTaken[matchList[i].indexPosition] = true;
                        }
                    }
                }

                debug_time1 = (int)debugStopwatch.ElapsedTicks - debug_time0;

            }

            processing = false;
        }

        public class Match
        {
            public Match(TrackedHand hand, OptitrackMarker marker, int indexPosition, Vector3 pos, float quality)
            {
                this.marker = marker;
                this.indexPosition = indexPosition;
                this.position = pos;
                this.quality = quality;
                this.hand = hand;
            }

            public OptitrackMarker marker;
            public int indexPosition;
            public Vector3 position;
            public float quality;
            public TrackedHand hand;

        }


        List<Vector3>[,] basePositionAnalysisPoints;

        public void PrepareForSphericalAnalysisMeta()
        {
            basePositionAnalysisPoints = new List<Vector3>[hands.Length, 5];
            for (int i = 0; i < hands.Length; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    basePositionAnalysisPoints[i, j] = new List<Vector3>();
                }
            }
        }

        public void AddDataForSphericalAnalysisMeta(Vector3 point, int handID, int fingerID)
        {
            basePositionAnalysisPoints[handID, fingerID].Add(point);
        }

        public void CalculateSphericalAnalysisMeta()
        {
            for (int i = 0; i < basePositionAnalysisPoints.GetLength(0); i++) //i handindex
            {
                for (int j = 0; j < 5; j++)//j fingerindex
                {
                    CalculateBasePoints(basePositionAnalysisPoints[i, j], j, hands[i].SetMeta);
                }
            }
        }

        public void CalculateBasePoints(List<Vector3> dataPoints, int fingerId, System.Action<Vector3, int, float> callBack)
        {
            SphericalRegressionMATH.SphericalRegressionAccord sr = new SphericalRegressionMATH.SphericalRegressionAccord();
            sr.Data = dataPoints;
            sr.Fit();

            callBack(sr.center, fingerId, (float)sr.radius);
        }


    }

}

