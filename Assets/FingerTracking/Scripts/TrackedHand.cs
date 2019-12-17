using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking
{
    using HandModel;
    using MarkerManagement;

    [System.Serializable]
    public class TrackedHand : MonoBehaviour
    {
        Color[] fingerColors = new Color[] { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta };

        public bool debug;

        public List<Vector3> localMarkerPositionsFromHand;

        public Transform publicTransform;

        private Matrix4x4 worldToLocalMatrix;
        private Matrix4x4 localToWorldMatrix;

        private Vector3 localVectorRight = Vector3.zero;
        private Vector3 localVectorUp = Vector3.zero;
        private Vector3 localVectorForward = Vector3.zero;

        public int rigidBodyID = -1;

        public bool calibrated = false;

        private TrackedHandData handData;

        public Vector3[] poseCalibrationFull = new Vector3[22];
        public Vector3[] poseCalibrationDetection = new Vector3[10];
        public Vector3[] poseCalibrationTight = new Vector3[10];
        public Vector3[] poseCalibrationWide = new Vector3[10];
        public Vector3[] poseCalibrationAngle = new Vector3[10];
        public Vector3[] calibrationPoseDirect;


        public float calibrationPoseMSE = 1;

        const float RADtoNORM = 0.15915494309f;

        public enum Side
        {
            left,
            right
        }

        [SerializeField]
        public float[,] segmentLengths = new float[5, 3];
        public Vector3[,] jointPositionsLocal = new Vector3[5, 4];
        private Vector3 carpus;
        private Vector3 trapezium;

        //public Vector3[] metaPositions;
        public OptitrackMarker[] markers = new OptitrackMarker[10];
        public Vector3[] sphericalMarkerPositions = new Vector3[10];

        public Side side;

        private List<Vector3> markerPositions;
        private long timeStamp;

        public bool Calibrated { get; set; }

        // Start is called before the first frame update
        void Start()
        {

            handData = new TrackedHandData();

            publicTransform = transform;

            localMarkerPositionsFromHand = new List<Vector3>(512);

            calibrationPoseDirect = new Vector3[10];

            for (int i = 0; i < markers.Length; i++)
            {
                markers[i] = new OptitrackMarker(Vector3.zero);
            }

            try
            {
                rigidBodyID = GetComponent<OptitrackRigidBody>().RigidBodyId;
            }
            catch (System.Exception)
            {
                Debug.LogError("no optitrack rigidbody found!");
            }

            CreateCylinderDummyHand();

            FingerTrackingMaster.Instance.RegisterHand(this);

            if (side == Side.left)
            {
                thumbOffsetAnglesMin = new Vector3(0, 0, -115);
                thumbOffsetAnglesMax = new Vector3(0, 0, -110);
            }
            else
            {
                thumbOffsetAnglesMin = new Vector3(0, 0, 115);
                thumbOffsetAnglesMax = new Vector3(0, 0, 110);
            }

            markerPositions = new List<Vector3>(128);

        }

        public Vector3 TransformWorldToLocalSpace(Vector3 worldPoint)
        {
            return worldToLocalMatrix.MultiplyPoint3x4(worldPoint);
        }

        public Vector3 TransformLocalToWorldSpace(Vector3 localPoint)
        {
            return localToWorldMatrix.MultiplyPoint3x4(localPoint);
        }

        public Vector3 TransformWorldToLocalSpaceVector(Vector3 worldVector)
        {
            return worldToLocalMatrix.MultiplyVector(worldVector);
        }

        public void SetSphericalMarkerPositions(ref Vector3[] positions)
        {
            for (int i = 0; i < sphericalMarkerPositions.Length; i++)
            {
                sphericalMarkerPositions[i] = positions[i];
            }
        }

        public string GetMLString()
        {
            string s = $"{transform.position.x},{transform.position.y},{transform.position.z}:";
            for (int i = 0; i < sphericalMarkerPositions.Length; i++)
            {
                if (i > 0) s += ";"; //add semicolon between entries (except first) 

                float phi = side == Side.left ? 1 - sphericalMarkerPositions[i].x : sphericalMarkerPositions[i].x; //if left hand -> invert angle

                sphericalMarkerPositions[i] = TransformPositionToSpherical(markers[i].GetCurrentPosition());
                s += $"{phi.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{sphericalMarkerPositions[i].y.ToString(System.Globalization.CultureInfo.InvariantCulture)}," +
                    $"{sphericalMarkerPositions[i].z.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            }
            return s;

            Vector3 TransformPositionToSpherical(Vector3 position)
            {
                // (1/3): r
                float r = position.magnitude;

                // (2/3): phi
                float phi = -1;

                if (position.x == 0)
                {
                    //y-axis
                    if (position.z > 0)
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
                else if (position.x < 0 && position.z < 0)
                {
                    phi = -1 * Mathf.PI / 2f - Mathf.Atan2(position.z, position.x);
                }
                else
                {
                    phi = 3 * Mathf.PI / 2f - Mathf.Atan2(position.z, position.x);
                }

                // (3/3): theta
                float theta = Mathf.Asin(position.y / r);

                float normR = 1f / poseCalibrationFull[11].magnitude; //distance to middle finger 
                return new Vector3(RADtoNORM * phi, RADtoNORM * theta, normR * r);
            }
        }

        public void CalculateMetasFromPoses()
        {
            Vector3 mp0, mp1, mp2;
            Vector3 lp0, lp1;
            float error = 0;
            for (int i = 0; i < 5; i++)
            {
                //tight and angle
                Math3D.ClosestPointsOnTwoLines(
                    out lp0,
                    out lp1,
                    poseCalibrationTight[2 * i], poseCalibrationTight[2 * i + 1] - poseCalibrationTight[2 * i],
                    poseCalibrationAngle[2 * i], poseCalibrationAngle[2 * i + 1] - poseCalibrationAngle[2 * i]);
                mp0 = (lp0 + lp1) / 2f;
                error += Vector3.Distance(lp0, lp1) * Vector3.Distance(lp0, lp1);

                //wide and angle
                Math3D.ClosestPointsOnTwoLines(
                    out lp0,
                    out lp1,
                    poseCalibrationWide[2 * i], poseCalibrationWide[2 * i + 1] - poseCalibrationWide[2 * i],
                    poseCalibrationAngle[2 * i], poseCalibrationAngle[2 * i + 1] - poseCalibrationAngle[2 * i]);
                mp1 = (lp0 + lp1) / 2f;
                error += Vector3.Distance(lp0, lp1) * Vector3.Distance(lp0, lp1);

                // wide and tight
                Math3D.ClosestPointsOnTwoLines(
                    out lp0,
                    out lp1,
                    poseCalibrationWide[2 * i], poseCalibrationWide[2 * i + 1] - poseCalibrationWide[2 * i],
                    poseCalibrationTight[2 * i], poseCalibrationTight[2 * i + 1] - poseCalibrationTight[2 * i]);
                mp2 = (lp0 + lp1) / 2f;
                error += Vector3.Distance(lp0, lp1) * Vector3.Distance(lp0, lp1);

                //calculate middle
                jointPositionsLocal[i, 0] = (mp0 + mp1 + mp2) / 3f;

            }

            print($"meta done, <b>error:{error}</b>");
        }

        public void SetMeta(Vector3 position, int fingerID, float radius) //step 1 / 2
        {
            jointPositionsLocal[fingerID, 0] = position;
        }

        public void CalibrateLength() // step 2 / 2
        {
            for (int i = 0; i < 5; i++)
            {
                segmentLengths[i, 0] = Vector3.Distance(markers[2 * i].GetCurrentPosition(), jointPositionsLocal[i, 0]);
                segmentLengths[i, 1] = (2f / 3f) * (Vector3.Distance(markers[2 * i + 1].GetCurrentPosition(), markers[2 * i].GetCurrentPosition()) + 0.001f);
                segmentLengths[i, 2] = 0.5f * segmentLengths[i, 1]; // = 1/3...
            }
        }

        public void CalculateErrorToCalibrationPose()
        {
            UpdateMarkerPositions();

            if (markerPositions.Count == 0)
                return;

            float mse = 0;
            for (int j = 0; j < 10; j++)
            {
                float f = float.MaxValue, d;
                int imin = -1;
                for (int i = 0; i < markerPositions.Count; i++)
                {
                    d = Vector3.Distance(markerPositions[i], TransformLocalToWorldSpace(calibrationPoseDirect[j]));
                    if (d < f)
                    {
                        imin = i;
                        f = d;
                    }
                }
                //Debug.DrawLine(checkPositions[imin], TransformLocalToWorldSpace(calibrationPose[j]), Color.red);

                if (imin != -1)
                {
                    mse += f * f;
                }
            }

            calibrationPoseMSE = mse;
        }

        /// <summary>
        /// Updates the current marker positions in the "markerPositions" array
        /// </summary>
        private void UpdateMarkerPositions()
        {
            var streamingClient = FingerTrackingMaster.Instance.StreamingClient;

            if (timeStamp >= streamingClient.TimeStamp) return;
            timeStamp = streamingClient.TimeStamp;

            streamingClient.ReadMarkerPositions(markerPositions);
        }

        public Vector3[] GetPose(int p)
        {
            //TODO switch case p
            return poseCalibrationFull;
        }

        public void SavePose(int p)
        {
            Vector3[] target;
            switch (p)
            {
                case 0:
                    target = poseCalibrationFull;
                    break;
                case 1:
                    target = poseCalibrationTight;
                    break;
                case 2:
                    target = poseCalibrationAngle;
                    break;
                case 3:
                    target = poseCalibrationWide;
                    break;
                case 4:
                    target = poseCalibrationDetection;
                    break;
                default:
                    return;
            }

            if (p == 0)
            {
                for (int fi = 0; fi < 5; fi++)
                {
                    for (int ji = 0; ji < 4; ji++)
                    {
                        target[fi * 4 + ji] = GetLocalPosition(fi, ji);
                    }
                }
                target[20] = carpus;
                target[21] = trapezium;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    target[i] = markers[i].GetCurrentPosition();
                }
            }
        }

        public void FinalizeCalibration()
        {
            foreach (GameObject g in handDummies)
            {
                g.SetActive(true);
            }

            calibrated = true;

            GetComponent<HandModelInterface>().CalibrateHandModel();
        }

        public void SetCalibrationPose()
        {
            for (int i = 0; i < 10; i++)
            {
                calibrationPoseDirect[i] = markers[i].GetCurrentPosition();
            }

            localVectorRight = Vector3.Cross(transform.InverseTransformDirection(transform.up), (markers[5].GetCurrentPosition() - markers[4].GetCurrentPosition()).normalized);
            localVectorUp = Vector3.Cross(localVectorRight, -(markers[5].GetCurrentPosition() - markers[4].GetCurrentPosition()).normalized).normalized;
            localVectorForward = Vector3.Cross(localVectorRight, localVectorUp).normalized;

            // simulate carpus and thumb meta
            carpus = jointPositionsLocal[4, 0] - localVectorForward * (segmentLengths[4, 0] + segmentLengths[4, 1] + segmentLengths[4, 2]);
            jointPositionsLocal[0, 0] = jointPositionsLocal[1, 0] - localVectorForward * 0.95f * (segmentLengths[1, 0] + segmentLengths[1, 1] + segmentLengths[1, 2]);

            //set static offset
            trapezium = jointPositionsLocal[0, 0] - localVectorUp * 0.00f;// - localVectorUp * 0.01f;
            carpus -= localVectorUp * 0.00f;

            for (int i = 1; i < 5; i++)
            {
                jointPositionsLocal[i, 0] -= localVectorUp * 0.01f;
            }
            jointPositionsLocal[0, 0] -= localVectorUp * 0.01f;

            if (side == Side.left)
            {
                carpus += localVectorRight * 0.01f;
                jointPositionsLocal[0, 0] -= localVectorRight * 0.02f;
                trapezium -= localVectorRight * 0.025f;
            }
            else
            {
                carpus -= localVectorRight * 0.01f;
                jointPositionsLocal[0, 0] += localVectorRight * 0.02f;
                trapezium += localVectorRight * 0.025f;
            }
            

            //activate
            SavePose(0);
            FinalizeCalibration();
        }

        public void DefineParents()
        {
            print("setting parents");
            //parent
            for (int i = 0; i < 5; i++)
            {
                markers[2 * i + 1].ClearParent();
                markers[2 * i].ClearParent();

                markers[2 * i + 1].SetParent(markers[2 * i]);
                markers[2 * i].SetParent(jointPositionsLocal[i, 0]);
            }
        }

        public int TrackedMarkerCount()
        {
            int count = 0;
            foreach (OptitrackMarker m in markers)
            {
                if (m.assignedInLastFrame)
                    count++;
            }
            return count;
        }

        public float CalculateAssignError()
        {
            float assignError = 0;
            int count = 0;
            foreach (OptitrackMarker m in markers)
            {
                if (m.assignedInLastFrame)
                {
                    count++;
                    assignError += m.assignQuality * m.assignQuality;
                }
            }
            assignError /= count;

            return assignError;
        }


        public float lengthError = 0f;
        public float CalculateLengthError()
        {
            float f = 0, mse = 0, max = 0;
            for (int i = 1; i < 5; i++)
            {
                f = Vector3.Distance(markers[2 * i].GetCurrentPosition(), jointPositionsLocal[i, 0]) - segmentLengths[i, 0];
                mse += f * f;

                if (f > max)
                    max = f;

                f = (Vector3.Distance(markers[2 * i + 1].GetCurrentPosition(), markers[2 * i].GetCurrentPosition()) - segmentLengths[i, 1] - segmentLengths[i, 2]); //weight because they are not constant
                if (f > 0)
                {
                    mse += f * f;
                }

                if (f > max)
                    max = f;
            }

            lengthError = mse;
            return lengthError;
        }

        public void UpdateMarker(OptitrackMarker marker, Vector3 position, float quality)
        {
            marker.UpdatePosition(position, quality);
        }

        public OptitrackMarker[] GetMarkers()
        {
            return markers;
        }

        public void ReceiveUpdate(OptitrackMarker[] marker)
        {
            markers = marker;
        }

        public void ReceiveUpdate(Vector3[] positionsUpdate)
        {
            for (int i = 0; i < 10; i++)
            {
                markers[i].UpdatePosition(positionsUpdate[i], 1.0f);
            }
        }

        public void UpdateWorldToLocalMatrix_OUT() //out of unity
        {
            var rb = FingerTrackingMaster.Instance.StreamingClient.GetLatestRigidBodyState(rigidBodyID);

            if (rb == null) return;

            OptitrackPose pose = rb.Pose;
            Matrix4x4 m = Matrix4x4.identity;
            m.SetTRS(pose.Position, pose.Orientation, Vector3.one);
            localToWorldMatrix = m;
            worldToLocalMatrix = m.inverse;
        }

        public void UpdateWorldToLocalMatrix()
        {
            worldToLocalMatrix = transform.worldToLocalMatrix; //update each frame!
            localToWorldMatrix = transform.localToWorldMatrix;
        }

        public Vector3 thumbOffsetAnglesMin;
        public Vector3 thumbOffsetAnglesMax;
        public float thumbDistance;

        private Vector3 GetThumbRightVector()
        {
            Vector3 rigForward = (markers[1].GetCurrentPosition() - markers[0].GetCurrentPosition()).normalized;

            //reference to metacarpal joint of index

            //print("JP:" + (jointPositions[0,1].y-markers[0].GetCurrentPosition().y));
            thumbDistance = (jointPositionsLocal[1, 0].y - markers[0].GetCurrentPosition().y - 0.02f) / 0.01f;
            thumbDistance = Mathf.Clamp01(thumbDistance);

            Quaternion qThumbOffset = Quaternion.Euler(Vector3.Lerp(thumbOffsetAnglesMin, thumbOffsetAnglesMax, thumbDistance));
            Vector3 rigRight = Vector3.Cross(localVectorUp, rigForward).normalized;
            rigRight = qThumbOffset * rigRight;
            return rigRight;
        }

        Vector3 CalcDistalIK(Vector3 proximal, Vector3 tip, float lengthMiddle, float lengthDistal, bool isThumb = false)
        {
            float length = Vector3.Distance(proximal, tip);

            float ld = (lengthDistal * lengthDistal - lengthMiddle * lengthMiddle + length * length) / (2 * length);

            float h = lengthDistal * lengthDistal - ld * ld;
            h = h >= 0 ? Mathf.Sqrt(h) : 0;

            Vector3 rigForward = (tip - proximal).normalized;
            Vector3 rigUp, rigRight;
            if (isThumb)
            {
                //Quaternion qThumbOffset = Quaternion.Euler(thumbOffsetAngles);
                // rigRight = Vector3.Cross(localVectorUp, rigForward).normalized;
                rigRight = GetThumbRightVector();
                rigUp = Vector3.Cross(rigForward, rigRight).normalized;
            }
            else
            {
                rigUp = Vector3.Cross(rigForward, localVectorRight).normalized;
                rigRight = Vector3.Cross(rigUp, rigForward).normalized;
            }


            //Debug.DrawRay(transform.TransformPoint(tip - ld * rigForward + h * rigUp), 0.05f * transform.TransformDirection(rigForward), Color.blue);
            //Debug.DrawRay(transform.TransformPoint(tip - ld * rigForward + h * rigUp), 0.05f * transform.TransformDirection(rigRight), Color.red);
            //Debug.DrawRay(transform.TransformPoint(tip - ld * rigForward + h * rigUp), 0.05f * transform.TransformDirection(rigUp), Color.green);

            return tip - ld * rigForward + h * rigUp;
            //return tip - ld * rigForward + h * Vector3.Cross(rigForward, Vector3.Cross(rigForward, Vector3.up).normalized).normalized;
        }

        void CheckHealthAndRepair()
        {
            if (!calibrated)
                return;

            CalculateErrorToCalibrationPose();

            if (CalculateLengthError() > 0.0005f)
            {

                if (calibrationPoseMSE < 0.005f)
                {
                    FingerTrackingMaster.Instance.Calibrator.MarkerAssignToHands(this);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateWorldToLocalMatrix();
            if (debug)
            {
                VisualizeHand();
            }

            CheckHealthAndRepair();

            if (Input.GetKeyDown(KeyCode.H))
            {
                ReportJointPositions();
            }



        }

        Vector3 localPosition = Vector3.zero;
        Vector3 fwd, dwn;
        public Vector3 GetWorldPosition(int fingerIndex, int joint)
        {
            return TransformLocalToWorldSpace(GetLocalPosition(fingerIndex, joint));
        }

        Vector3 GetLocalPosition(int fingerIndex, int joint)
        {
            switch (joint)
            {
                case 0:
                    localPosition = jointPositionsLocal[fingerIndex, 0];
                    break;
                case 1:
                    localPosition = markers[fingerIndex * 2].GetCurrentPosition();
                    fwd = (localPosition - jointPositionsLocal[fingerIndex, 0]).normalized;
                    if (fingerIndex == 0)
                    {
                        dwn = Vector3.Cross(GetThumbRightVector(), fwd).normalized;
                    }
                    else
                    {
                        dwn = Vector3.Cross(localVectorRight, fwd).normalized;
                    }
                    localPosition += fwd * 0.0f + dwn * 0.01f;
                    break;
                case 2:
                    localPosition = CalcDistalIK(
                        TransformWorldToLocalSpace(GetWorldPosition(fingerIndex, 1)),
                        TransformWorldToLocalSpace(GetWorldPosition(fingerIndex, 3)),
                        segmentLengths[fingerIndex, 1],
                        segmentLengths[fingerIndex, 2],
                        isThumb: fingerIndex == 0);
                    break;
                case 3:
                    localPosition = markers[fingerIndex * 2 + 1].GetCurrentPosition();
                    fwd = (localPosition - markers[fingerIndex * 2].GetCurrentPosition()).normalized;
                    if (fingerIndex == 0)
                    {
                        dwn = Vector3.Cross(GetThumbRightVector(), fwd).normalized;
                    }
                    else
                    {
                        dwn = Vector3.Cross(localVectorRight, fwd).normalized;
                    }
                    Debug.DrawLine(localPosition, localPosition + dwn, Color.white * 0.06f);
                    localPosition += fwd * 0.005f + dwn * 0.008f;
                    break;
            }

            return localPosition;
        }
        void VisualizeHand()
        {
            if (markers != null)
            {
                lock (markers)
                {
                    foreach (OptitrackMarker m in markers)
                    {
                        //Debug.DrawLine(TransformLocalToWorldSpace(m.GetCurrentPosition()), transform.position, Color.cyan);
                        //Debug.DrawLine(m.GetCurrentPosition(), transform.position, Color.yellow);
                    }

                    if (calibrated)
                        ControlHandDummy();

                    //Debug.DrawRay(transform.position, transform.TransformVector(localVectorRight) * 1f, Color.red);
                    //Debug.DrawRay(transform.position, transform.TransformVector(localVectorUp) * 1f, Color.green);
                    //Debug.DrawRay(transform.position, transform.TransformVector(localVectorForward) * 1f, Color.blue);

                    for (int i = 0; i < 5; i++)
                    {

                        Debug.DrawLine(GetWorldPosition(i, 0), GetWorldPosition(i, 1), fingerColors[i] * 0.6f);
                        Debug.DrawLine(GetWorldPosition(i, 1), GetWorldPosition(i, 3), fingerColors[i]);
                        /*
                        Debug.DrawLine(
                            transform.TransformPoint(metaPositions[i]), 
                            transform.TransformPoint(markers[2*i].GetCurrentPosition()), 
                            fingerColors[i] * 0.6f);
                        */
                        //calculate distal IK

                        Vector3 distal = CalcDistalIK(markers[2 * i].GetCurrentPosition(), markers[2 * i + 1].GetCurrentPosition(), segmentLengths[i, 1], segmentLengths[i, 2], isThumb: i == 0);
                        /*
                        Debug.DrawLine(
                            transform.TransformPoint(markers[2 * i].GetCurrentPosition()),
                            transform.TransformPoint(distal),
                            fingerColors[i]);
                        Debug.DrawLine(
                            transform.TransformPoint(distal),
                            transform.TransformPoint(markers[2 * i + 1].GetCurrentPosition()),
                            fingerColors[i]);
                        */
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        Debug.DrawLine(
                            transform.TransformPoint(markers[i].GetLastPosition()),
                            transform.TransformPoint(markers[i].GetCurrentPosition()),
                            Color.black);
                        Debug.DrawLine(
                            transform.TransformPoint(markers[i].GetPredictedPosition()),
                            transform.TransformPoint(markers[i].GetCurrentPosition()),
                            Color.white);
                    }
                }
            }
        }

        Vector3 v;
        public TrackedHandData GetData()
        {
            //length
            handData.segmentLengths = segmentLengths;

            //side
            handData.side = (int)side;

            //convert joint positions from Vector3 to float-array
            for (int fi = 0; fi < 5; fi++)
            {
                for (int ji = 0; ji < 4; ji++)
                {
                    v = poseCalibrationFull[4 * fi + ji];
                    handData.jointPositions[fi * 4 + ji, 0] = v.x;
                    handData.jointPositions[fi * 4 + ji, 1] = v.y;
                    handData.jointPositions[fi * 4 + ji, 2] = v.z;
                }
            }

            handData.jointPositions[20, 0] = carpus.x;
            handData.jointPositions[20, 1] = carpus.y;
            handData.jointPositions[20, 2] = carpus.z;
            handData.jointPositions[21, 0] = trapezium.x;
            handData.jointPositions[21, 1] = trapezium.y;
            handData.jointPositions[21, 2] = trapezium.z;


            //convert calibration pose from V3 to f[]
            for(int i = 0;i<10;i++)
            {
                handData.calibrationPositions[i, 0] = calibrationPoseDirect[i].x;
                handData.calibrationPositions[i, 1] = calibrationPoseDirect[i].y;
                handData.calibrationPositions[i, 2] = calibrationPoseDirect[i].z;
            }

            return handData;
        }

        public void SetData(TrackedHandData loadedHandData)
        {
            //lengths
            segmentLengths = loadedHandData.segmentLengths;

            //side
            if (loadedHandData.side == 0)
            {
                side = Side.left;
            }
            else
                side = Side.right;

            //jointPositions
            int index;
            for (int fi = 0; fi < 5; fi++)
            {
                for (int ji = 0; ji < 4; ji++)
                {
                    index = ji + fi * 4;
                    for (int c = 0; c < 3; c++)
                    {
                        jointPositionsLocal[fi, ji][c] = loadedHandData.jointPositions[index, c];
                    }
                }
                //transfer jointPosition to marker
                markers[2 * fi].UpdatePosition(jointPositionsLocal[fi, 1], 1);
                markers[2 * fi + 1].UpdatePosition(jointPositionsLocal[fi, 3], 1);
            }

            //Debug.Log("carpus" + thd.positions[20, 0] + " " + thd.positions[20, 1] + " " + thd.positions[20, 2]);

            //simulated
            carpus[0] = loadedHandData.jointPositions[20, 0];
            carpus[1] = loadedHandData.jointPositions[20, 1];
            carpus[2] = loadedHandData.jointPositions[20, 2];
            trapezium[0] = loadedHandData.jointPositions[21, 0];
            trapezium[1] = loadedHandData.jointPositions[21, 1];
            trapezium[2] = loadedHandData.jointPositions[21, 2];


            calibrationPoseDirect = new Vector3[10];
            //convert calibration pose from V3 to f[]
            for (int i = 0; i < 10; i++)
            {
                calibrationPoseDirect[i] = new Vector3(calibrationPoseDirect[i].x, calibrationPoseDirect[i].y, calibrationPoseDirect[i].z);
            }

            FinalizeCalibration();

            SavePose(0);
            calibrated = true;
            GetComponent<HandModelInterface>().CalibrateHandModel();

            /*
            void CopyVector3Array(Vector3[] source, ref Vector3[] destination)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    destination[i] = new Vector3(source[i].x, source[i].y, source[i].z);
                }
            }
            */
        }


        public int GetCurrentJointData(ref Vector3[] positions)
        {
            if (positions.Length < 22)
            {
                Debug.LogWarning("error: TrackedHand, GetCurrentJointData not long enough");
                return -1;
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    positions[4 * i + j] = GetWorldPosition(i, j);
                }
            }
            positions[20] = TransformLocalToWorldSpace(carpus);
            positions[21] = TransformLocalToWorldSpace(trapezium);

            return 1;
        }

        void ReportJointPositions()
        {
            Vector3[] data = new Vector3[21];
            GetCurrentJointData(ref data);
            string r = "Vector3[] positions = new Vector3[]{\n";
            for (int i = 0; i < data.Length; i++)
            {
                r += "new Vector3(" + data[i].x.ToString().Replace(',', '.') + "f," + data[i].y.ToString().Replace(',', '.') + "f," + data[i].z.ToString().Replace(',', '.') + "f) //id:" + i;
                if (i < data.Length - 1)
                    r += ",\n";
            }
            r += "\n};";

            print(r);
        }



        GameObject[] handDummies;
        void ControlHandDummy()
        {
            int index;
            for (int i = 0; i < 5; i++)
            {
                index = 7 * i;
                SetJoint(index, i, 0);
                SetPhalanx(index + 1, i, 0, i, 1);
                SetJoint(index + 2, i, 1);

                SetPhalanx(index + 3, i, 1, i, 2);//should simulate distal
                SetJoint(index + 4, i, 2); //should simulate distal
                SetPhalanx(index + 5, i, 2, i, 3);//should simulate distal
                SetJoint(index + 6, i, 3);//should simulate distal
            }

            SetPhalanx(35, 1, 0, trapezium);
            SetPhalanx(36, 1, 0, 2, 0);
            SetPhalanx(37, 2, 0, 3, 0);
            SetPhalanx(38, 3, 0, 4, 0);
            SetPhalanx(39, 4, 0, trapezium);

            SetPhalanx(40, trapezium, carpus);
            SetPhalanx(41, 4, 0, carpus);
            SetPhalanx(42, 1, 0, carpus);
            SetJoint(43, carpus);
            SetJoint(44, trapezium);
        }
        void SetJoint(int objIndex, int indexFinger, int indexJoint)
        {
            handDummies[objIndex].transform.position = GetWorldPosition(indexFinger, indexJoint);
        }
        void SetJoint(int objIndex, Vector3 localPosition)
        {
            handDummies[objIndex].transform.position = TransformLocalToWorldSpace(localPosition);
        }
        void SetPhalanx(int objIndex, int jointFromIndex, int jointToIndex)
        {
            handDummies[objIndex].transform.position = (handDummies[jointFromIndex].transform.position + handDummies[jointToIndex].transform.position) / 2f;
            handDummies[objIndex].transform.localScale = new Vector3(0.01f, Vector3.Distance(handDummies[jointFromIndex].transform.position, handDummies[jointToIndex].transform.position) / 2f, 0.01f);
            handDummies[objIndex].transform.LookAt(handDummies[jointToIndex].transform.position);
            handDummies[objIndex].transform.Rotate(90, 0, 0, Space.Self);
        }
        void SetPhalanx(int objIndex, int indexFinger, int indexJoint, int targetIndexFinger, int targetIndexJoint)
        {
            handDummies[objIndex].transform.position = (GetWorldPosition(indexFinger, indexJoint) + GetWorldPosition(targetIndexFinger, targetIndexJoint)) / 2f;
            handDummies[objIndex].transform.localScale = new Vector3(0.01f, Vector3.Distance(GetWorldPosition(indexFinger, indexJoint), GetWorldPosition(targetIndexFinger, targetIndexJoint)) / 2f, 0.01f);
            handDummies[objIndex].transform.LookAt(GetWorldPosition(targetIndexFinger, targetIndexJoint));
            handDummies[objIndex].transform.Rotate(90, 0, 0, Space.Self);
        }
        void SetPhalanx(int objIndex, Vector3 localFromPosition, Vector3 localToPosition)
        {
            Vector3 worldFromPosition = TransformLocalToWorldSpace(localFromPosition);
            Vector3 worldToPosition = TransformLocalToWorldSpace(localToPosition);

            handDummies[objIndex].transform.position = (worldFromPosition + worldToPosition) / 2f;
            handDummies[objIndex].transform.localScale = new Vector3(0.01f, Vector3.Distance(worldFromPosition, worldToPosition) / 2f, 0.01f);
            handDummies[objIndex].transform.LookAt(worldToPosition);
            handDummies[objIndex].transform.Rotate(90, 0, 0, Space.Self);
        }
        void SetPhalanx(int objIndex, int indexFinger, int indexJoint, Vector3 localPosition)
        {
            Vector3 worldPosition = TransformLocalToWorldSpace(localPosition);

            handDummies[objIndex].transform.position = (GetWorldPosition(indexFinger, indexJoint) + worldPosition) / 2f;
            handDummies[objIndex].transform.localScale = new Vector3(0.01f, Vector3.Distance(GetWorldPosition(indexFinger, indexJoint), worldPosition) / 2f, 0.01f);
            handDummies[objIndex].transform.LookAt(worldPosition);
            handDummies[objIndex].transform.Rotate(90, 0, 0, Space.Self);
        }

        void CreateCylinderDummyHand()
        {
            List<GameObject> dummies = new List<GameObject>(50);
            for (int i = 0; i < 5; i++) //0-34 fingersegments
            {
                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //meta
                dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_J_meta";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //meta -> proximal
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_P_proximal";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //proximal
                dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_J_proximal";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //proximal -> distal
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_P_intermediate";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //distal
                dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_J_distal";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //distal->tip
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_P_distal";

                dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //tip
                dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                dummies[dummies.Count - 1].GetComponent<Renderer>().material.color = fingerColors[i];
                dummies[dummies.Count - 1].name = $"f{i}_J_tip";
            }

            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //35 meta index -> thumb
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //36 meta middle -> index
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //37 meta ring -> middle
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //38 meta little -> ring
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //39 meta thumb ->  litte

            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //40 meta thumb ->  carpus
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //41 meta little ->  carpus
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Cylinder)); //42 carpus ->  meta index
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //43 carpus
            dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            dummies.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere)); //44 trapezium
            dummies[dummies.Count - 1].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            GameObject hcontainer = new GameObject($"container hand {rigidBodyID}");
            foreach (GameObject g in dummies)
            {
                Destroy(g.GetComponent<Collider>());
                g.transform.parent = hcontainer.transform;
                g.SetActive(false);
            }

            handDummies = dummies.ToArray();
        }


        public bool drawCenteredGizmo = false;
        private void OnDrawGizmos()
        {
            //Debug.DrawLine(ap + 3*ad, ap - 3*ad, Color.red);
            //Debug.DrawLine(bp + 3 * bd, bp - 3 * bd, Color.blue);
            //Debug.DrawLine(is0, is1, Color.magenta);

            if (drawCenteredGizmo)
            {
                for (int i = 0; i < 5; i++)
                {
                    Gizmos.color = fingerColors[i];

                    for (int j = 0; j < 4; j++)
                    {
                        Gizmos.DrawSphere(TransformWorldToLocalSpace(GetWorldPosition(i, j)), 0.01f);
                    }

                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(markers[2 * i].GetCurrentPosition(), markers[2 * i].GetParentPosition());

                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(markers[2 * i + 1].GetCurrentPosition(), markers[2 * i + 1].GetParentPosition());


                    float axisLength = 0.02f;
                    for (int j = 0; j < 2; j++)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(markers[2 * i + j].GetCurrentPosition(), markers[2 * i + j].GetCurrentPosition() + markers[2 * i + j].orientation * Vector3.forward * axisLength);

                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(markers[2 * i + j].GetCurrentPosition(), markers[2 * i + j].GetCurrentPosition() + markers[2 * i + j].orientation * Vector3.right * axisLength);

                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(markers[2 * i + j].GetCurrentPosition(), markers[2 * i + j].GetCurrentPosition() + markers[2 * i + j].orientation * Vector3.up * axisLength);

                    }
                }
            }
        }

    }

}