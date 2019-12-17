using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.MarkerManagement
{
    public class MarkerAssigner
    {
        const int SIZE = 512;
        const float DISTANCE_THRESHOLD = 0.015f;
        const float ANGLE_THRESHOLD = 10f;

        public bool TrackingEnabled { get; set; }
        public int Assigns { get; private set; }
        
        private bool ThreadRunning;

        private TrackedHand[] hands;

        private long timeStamp;

        private List<Vector3> markerPositions;

        OptitrackMarker[] activeMarkers;
        List<Match> matchList;

        private bool[] markerTaken = new bool[SIZE];
        private bool[] positionTaken = new bool[SIZE];

        List<Vector3>[,] basePositionAnalysisPoints;

        public bool processing = false;
        object lockObj = new object();

        public int debug_time_0_match, debug_time_1_assign, debug_time_2_assign;

        private int[] debugTimings = new int[10];
        System.Diagnostics.Stopwatch debugStopwatch = new System.Diagnostics.Stopwatch();



        public MarkerAssigner(TrackedHand[] hands)
        {
            this.hands = hands;

            matchList = new List<Match>(SIZE * SIZE);
            markerPositions = new List<Vector3>(256);

            ThreadRunning = true;
            new Thread(
                new ThreadStart(AssignLoop)
            ).Start();
        }


        private void AssignLoop()
        {
            var streamingClient = FingerTrackingMaster.Instance.StreamingClient;

            while (ThreadRunning)
            {
                if (TrackingEnabled)
                {
                    if (timeStamp < streamingClient.TimeStamp)
                    {
                        timeStamp = streamingClient.TimeStamp;

                        streamingClient.ReadMarkerPositions(markerPositions);
                        MarkerAssignPipeline();
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

        public void GetAssignTimings(ref int[] timings)
        {
            for(int i = 0;i<10;i++)
            {
                timings[i] = debugTimings[i];
            }
        }

        public void MarkerAssignPipeline()
        {
            processing = true;
            lock (lockObj)
            {

                debugStopwatch.Restart();

                //0 Prepare
                PrepareMatchingAlgorithm();
                debugTimings[0] = (int)debugStopwatch.ElapsedTicks;
                debugStopwatch.Restart();

                //1 first pass: assign marker for every hand
                FindMatchesByDistance();
                debugTimings[1] = (int)debugStopwatch.ElapsedTicks;
                debugStopwatch.Restart();

                //2 second pass: sort and assign by quality priority
                AssignMarkersByQuality();
                debugTimings[2] = (int)debugStopwatch.ElapsedTicks;
                debugStopwatch.Restart();

                //3 third pass: check for plausibility
                //CheckPlausibility();
                debugTimings[3] = (int)debugStopwatch.ElapsedTicks;
                debugStopwatch.Restart();

                //4 fourth pass: try to locate missing

                //5 fifth pass: simulate missing

                //6 sixth pass: finalize hands

            }

            processing = false;
        }


        void PrepareMatchingAlgorithm()
        {
            matchList.Clear();
            for (int j = 0; j < markerPositions.Count; j++)
            {
                positionTaken[j] = false;
            }
        }

        private void FindMatchesByDistance()
        {
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
                    float minValue = float.MaxValue;
                    int minIndex = -1;

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
        }

        private void AssignMarkersByQuality()
        {
            matchList.Sort((x, y) => x.quality.CompareTo(y.quality));

            for (int i = 0; i < matchList.Count; i++)
            {
                if (!matchList[i].marker.assignedInLastFrame)
                {
                    if (!positionTaken[matchList[i].indexPosition])
                    {
                        //matchList[i].marker.UpdatePosition(matchList[i].position, matchList[i].quality);
                        matchList[i].marker.StoreFuturePositionUpdate(matchList[i].position, matchList[i].quality);
                        matchList[i].marker.ApplyFuturePositionUpdate(); //do that after plausibility check!

                        matchList[i].marker.assignedInLastFrame = true;
                        positionTaken[matchList[i].indexPosition] = true;
                    }
                }
            }
        }

        private void CheckPlausibility()
        {
            foreach (TrackedHand hand in hands)
            {
                if (!hand.calibrated)
                {
                    foreach (OptitrackMarker otm in hand.markers)
                        otm.ApplyFuturePositionUpdate();
                    continue;
                }

                for (int i = 0; i < 5; i++)
                {
                    Vector3 OMP_0 = hand.jointPositionsLocal[i, 0];
                    Vector3 OMP_1 = hand.markers[2 * i].GetCurrentPosition();
                    Vector3 OMP_2 = hand.markers[2 * i+1].GetCurrentPosition();

                    float l0 = Vector3.Distance(OMP_1, OMP_0) / hand.segmentLengths[i, 0];
                    float l1 = Vector3.Distance(OMP_2, OMP_1) / (hand.segmentLengths[i, 1]+ hand.segmentLengths[i, 2]);


                    //length check
                    if(l0 > 1.5f || l0 <0.5f)
                    {
                        hand.markers[2 * i].assignedInLastFrame = false;
                    }
                    if (l1 > 1.5f || l0  < 0.5f)
                    {
                        hand.markers[2 * i+1].assignedInLastFrame = false;
                    }

                    if(hand.markers[2 * i].assignedInLastFrame)
                    {
                        if (hand.markers[2 * i + 1].assignedInLastFrame)
                        {
                            //both markers found -> do angle check!
                            float angle = Vector3.Angle(OMP_1 - OMP_0, OMP_2 - OMP_1);
                            if(false || angle>ANGLE_THRESHOLD)
                            {
                                hand.markers[2 * i].assignedInLastFrame = false;
                                hand.markers[2 * i + 1].assignedInLastFrame = false;
                            } 
                            else
                            {
                                hand.markers[2 * i].ApplyFuturePositionUpdate();
                                hand.markers[2 * i + 1].ApplyFuturePositionUpdate();
                            }
                        }
                        else
                        {
                            //only proximal marker found
                            hand.markers[2 * i].ApplyFuturePositionUpdate();
                        }
                    }
                    else
                    {
                        if (hand.markers[2 * i + 1].assignedInLastFrame)
                        {
                            //only distal marker found
                            hand.markers[2 * i + 1].ApplyFuturePositionUpdate();
                        }
                        else
                        {
                            //no markers found!
                        }
                    }
                }
            }
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

