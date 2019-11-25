using UnityEngine;

namespace FingerTracking.HandModel
{

    [RequireComponent(typeof(HandModelDataLoader))]
    [RequireComponent(typeof(TrackedHand))]
    public class HandModelInterface : MonoBehaviour
    {
        [Header("References")]
        public Transform handRootJoint;

        [Header("Debug")]
        public bool drawGizmos;
        public bool drawUpVectors;
        public bool forceCalibration;

        private enum BodySide { left, right };
        private BodySide bodySide;

        private TrackedHand trackedHand;
        private HandModelDataLoader handModelDataLoader;

        private Transform[][] handJoints;
        private float[][] handJointLengths;
        private Vector3[][] jointPositions;

        private float masterScaleReference = 1f;
        private float masterScaleFactor = 1f;


        private void Start()
        {
            handModelDataLoader = GetComponent<HandModelDataLoader>();

            if (handRootJoint == null || handModelDataLoader == null)
            {
                Debug.LogError("Missing References.");
                return;
            }

            GetReferences();
            CollectJoints();
            CollectJointLengths();
            ChangeJointHierarchy();

            if (handModelDataLoader.mode == HandModelDataLoader.Mode.testDataCalibration)
                CalibrateHandModel();
        }

        private void Update()
        {
            jointPositions = handModelDataLoader.GetJointPositions();

            if (handModelDataLoader.mode == HandModelDataLoader.Mode.live)
            {
                if (GetCalibrationStatus())
                    UpdateHandModel();
            }
            else
            {
                UpdateHandModel();
            }

            //Debug
            if (forceCalibration)
            {
                CalibrateHandModel();
                forceCalibration = false;
            }

            //Debug
            if (drawUpVectors)
            {
                for (int i = 0; i < handJoints.Length; i++)
                {
                    for (int j = 0; j < handJoints[i].Length; j++)
                    {
                        Debug.DrawRay(handJoints[i][j].parent.position, handJoints[i][j].parent.up * 0.05f, Color.green);
                    }
                }
            }
        }


        //Connection to Data-Delivery-Script
        private void GetReferences()
        {
            trackedHand = GetComponent<TrackedHand>();

            if (trackedHand == null)
            {
                Debug.LogError("Missing Reference.");
                return;
            }

            if (trackedHand.side == TrackedHand.Side.left)
                bodySide = BodySide.left;
            else if (trackedHand.side == TrackedHand.Side.right)
                bodySide = BodySide.right;
            else
                Debug.LogError("No valid side.");
        }
        private bool GetCalibrationStatus()
        {
            return trackedHand.calibrated;
        }


        //Collects References to all used Joints
        private void CollectJoints()
        {
            Transform[] allChildren = handRootJoint.GetComponentsInChildren<Transform>();
            handJoints = new Transform[6][];

            for (int i = 0; i < handJoints.Length; i++)
            {
                if (i == 0) //Arm Joints
                {
                    handJoints[0] = new Transform[3];

                    handJoints[0][0] = handRootJoint.parent.parent;
                    handJoints[0][1] = handRootJoint.parent;
                    handJoints[0][2] = handRootJoint;
                }
                else //Finger Joints
                {
                    handJoints[i] = new Transform[4];

                    for (int j = 0; j < handJoints[i].Length; j++)
                    {
                        int childIndex = (i - 1) * handJoints[i].Length + j;
                        handJoints[i][j] = allChildren[childIndex + 1];
                    }
                }
            }
        }

        //Calculates Joint Lengths of raw Model
        private void CollectJointLengths()
        {
            handJointLengths = new float[6][];

            for (int i = 0; i < handJoints.Length; i++)
            {
                if (i == 0) //Arm Joints
                    handJointLengths[i] = new float[handJoints[i].Length];
                else //Finger Joints
                    handJointLengths[i] = new float[handJoints[i].Length - 1];

                for (int j = 0; j < handJointLengths[i].Length; j++)
                {
                    if (i == 0 && j == 2) //Wrist Length
                        handJointLengths[0][2] = Vector3.Distance(handJoints[0][2].position, handJoints[3][0].position);
                    else //Finger Length
                        handJointLengths[i][j] = Vector3.Distance(handJoints[i][j].position, handJoints[i][j + 1].position);
                }
            }

            masterScaleReference = Vector3.Distance(handJoints[0][2].position, handJoints[3][3].position);
        }

        //Changes the Joint Hierarchy of the used Rig so that each Joint has a Pre and a Post Scaling-Object
        private void ChangeJointHierarchy()
        {
            GameObject preOffsetObject;
            GameObject postOffsetObject;
            Transform targetJoint;

            for (int i = 0; i < handJoints.Length; i++)
            {
                for (int j = 0; j < handJoints[i].Length; j++)
                {
                    if (j == handJoints[i].Length - 1)
                        if (i == 0) //Wrist Joint
                            targetJoint = handJoints[3][0];
                        else //Tip Joints
                            continue;
                    else
                        targetJoint = handJoints[i][j + 1];

                    preOffsetObject = new GameObject($"PreOffset_{handJoints[i][j].name}");
                    preOffsetObject.transform.position = handJoints[i][j].position;
                    preOffsetObject.transform.LookAt(targetJoint);
                    preOffsetObject.transform.parent = handJoints[i][j].parent;
                    handJoints[i][j].parent = preOffsetObject.transform;

                    for (int k = 1; k < handJoints.Length; k++)
                    {
                        if (i == 0 && j == handJoints[i].Length - 1) //Wrist Joint
                            targetJoint = handJoints[k][0];
                        else
                            k = handJoints.Length;

                        postOffsetObject = new GameObject($"PostOffset_{handJoints[i][j].name}_{k}");
                        postOffsetObject.transform.position = targetJoint.position;
                        postOffsetObject.transform.rotation = preOffsetObject.transform.rotation;
                        postOffsetObject.transform.parent = handJoints[i][j];
                        targetJoint.parent = postOffsetObject.transform;
                    }
                }
            }
        }

        //Scales all Bones to the appropriate real Finger Lengths
        public void CalibrateHandModel()
        {
            jointPositions = handModelDataLoader.GetJointPositions();

            //Calculates the MasterScaleFactor based on Wrist to Fingertip Length for an approximated Thickness of the Bones
            float targetLength = Vector3.Distance(jointPositions[0][2], jointPositions[3][3]);
            float scaleFactor = targetLength / masterScaleReference;
            masterScaleFactor = scaleFactor;

            //Calculates the Elbow Position if no suitable Data exists
            if (jointPositions[0][1] == Vector3.zero)
                jointPositions[0][1] = jointPositions[0][2] + (jointPositions[0][2] - jointPositions[3][0]).normalized * (handJointLengths[0][1] * masterScaleFactor);

            //Calculates the Shoulder Position if no suitable Data exists
            if (jointPositions[0][0] == Vector3.zero)
                jointPositions[0][0] = jointPositions[0][1] + (jointPositions[0][1] - jointPositions[0][2]).normalized * (handJointLengths[0][0] * masterScaleFactor);

            //Scales all Bones to the appropriate real Finger Lengths
            for (int i = 0; i < handJoints.Length; i++)
            {
                for (int j = 0; j < handJoints[i].Length - 1; j++)
                {
                    targetLength = Vector3.Distance(jointPositions[i][j], jointPositions[i][j + 1]);
                    scaleFactor = targetLength / handJointLengths[i][j];
                    handJoints[i][j].parent.localScale = new Vector3(masterScaleFactor, masterScaleFactor, scaleFactor);
                    handJoints[i][j].GetChild(0).localScale = new Vector3(1 / masterScaleFactor, 1 / masterScaleFactor, 1 / scaleFactor);

                    if (i == 0 && j == 1) //Wrist Bone
                    {
                        targetLength = Vector3.Distance(jointPositions[0][2], jointPositions[3][0]);
                        scaleFactor = targetLength / handJointLengths[i][j + 1];
                        handJoints[i][j + 1].parent.localScale = new Vector3(masterScaleFactor, masterScaleFactor, scaleFactor);

                        for (int k = 0; k < handJoints[i][j + 1].childCount; k++)
                            handJoints[i][j + 1].GetChild(k).localScale = new Vector3(1 / masterScaleFactor, 1 / masterScaleFactor, 1 / scaleFactor);
                    }
                }
            }
        }

        //Moves and Orients all Joints to the appropriate real Finger Positions
        private void UpdateHandModel()
        {
            //Calculates the Elbow Position if no suitable Data exists
            if (jointPositions[0][1] == Vector3.zero)
                jointPositions[0][1] = jointPositions[0][2] + (jointPositions[0][2] - jointPositions[3][0]).normalized * (handJointLengths[0][1] * masterScaleFactor);

            //Calculates the Shoulder Position if no suitable Data exists
            if (jointPositions[0][0] == Vector3.zero)
                jointPositions[0][0] = jointPositions[0][1] + (jointPositions[0][1] - jointPositions[0][2]).normalized * (handJointLengths[0][0] * masterScaleFactor);

            //Calcualtes the Reference Side Vector
            Vector3 forward, upwards;
            Vector3 referenceSide;

            if (bodySide == BodySide.left)
                referenceSide = (jointPositions[2][0] - jointPositions[5][0]).normalized;
            else
                referenceSide = (jointPositions[5][0] - jointPositions[2][0]).normalized;

            //Orients all Joints to the appropriate real Finger Positions
            for (int i = 0; i < handJoints.Length; i++)
            {
                for (int j = 0; j < handJoints[i].Length; j++)
                {
                    if (j < handJoints[i].Length - 1)
                    {
                        if (j == 0) //Shoulder and Meta Joints
                            handJoints[i][0].parent.position = jointPositions[i][0];

                        //Shoulder, Elbow, Meta, Proximal and Distal Joints
                        forward = jointPositions[i][j + 1] - jointPositions[i][j];
                    }
                    else
                    {
                        if (i == 0) //Wrist Joint
                            forward = jointPositions[3][0] - jointPositions[i][j];
                        else //Tip Joints
                            forward = jointPositions[i][j] - jointPositions[i][j - 1];
                    }

                    upwards = Vector3.Cross(forward, referenceSide);
                    handJoints[i][j].parent.rotation = Quaternion.LookRotation(forward, upwards);
                }
            }
        }

        //Draws the received raw Joint Poisitions for Debugging
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;

            for (int i = 0; i < jointPositions.Length; i++)
            {
                for (int j = 0; j < jointPositions[i].Length; j++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(jointPositions[i][j], 0.01f);
                }
            }
        }

    }

}