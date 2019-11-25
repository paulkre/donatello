using UnityEngine;

namespace FingerTracking.HandModel
{

    [RequireComponent(typeof(TrackedHand))]
    public class HandModelDataLoader : MonoBehaviour
    {
        public enum Mode { live, testDataCalibration, testDataMove };
        public Mode mode;

        private enum BodySide { left, right };
        private BodySide bodySide;

        private TrackedHand trackedHand;
        private Vector3[] rawJointPositions;
        private Vector3[][] formattedJointData;


        private void Start()
        {
            rawJointPositions = new Vector3[22];
            formattedJointData = new Vector3[6][];

            for (int i = 0; i < formattedJointData.Length; i++)
            {
                if (i == 0)
                    formattedJointData[i] = new Vector3[3];
                else
                    formattedJointData[i] = new Vector3[4];
            }

            GetReferences();
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


        //Returns the current Joint Positions based on the active Mode
        public Vector3[][] GetJointPositions()
        {
            switch (mode)
            {
                case Mode.live:
                    trackedHand.GetCurrentJointData(ref rawJointPositions);
                    return ConvertDataStructure();

                case Mode.testDataCalibration:
                    if (bodySide == BodySide.left)
                        return leftHandCalibrationPose;
                    else
                        return rightHandCalibrationPose;

                case Mode.testDataMove:
                    if (bodySide == BodySide.left)
                        return leftHandMovementPose;
                    else
                        return rightHandMovementPose;

                default:
                    Debug.LogError("Can't get Joint Positions");
                    return new Vector3[0][];
            }
        }

        //Converts Input Data Structure to Target Data Structure
        private Vector3[][] ConvertDataStructure()
        {
            for (int i = 0; i < formattedJointData.Length; i++)
            {
                for (int j = 0; j < formattedJointData[i].Length; j++)
                {
                    if (i == 0)
                    {
                        if (j == 2)
                            formattedJointData[0][2] = (rawJointPositions[20] + rawJointPositions[21]) / 2;
                        else
                            formattedJointData[0][j] = Vector3.zero;
                    }
                    else
                    {
                        formattedJointData[i][j] = rawJointPositions[(i - 1) * formattedJointData[i].Length + j];
                    }
                }
            }
            return formattedJointData;
        }


        //Test Datasets
        private readonly Vector3[][] rightHandCalibrationPose = new Vector3[][]
        {
        new Vector3[]
        {
            new Vector3(0f,0f,0f),                              //Arm: Shoulder
            new Vector3(0f,0f,0f),                              //Arm: Elbow
            new Vector3(0.09175615f,1.21682750f,0.40807465f)    //Arm: Wrist
        },
        new Vector3[]
        {
            new Vector3(0.09365724f,1.25803300f,0.40115000f),   //Thumb: Meta
            new Vector3(0.07329019f,1.26978800f,0.43513730f),   //Thumb: Proximal
            new Vector3(0.07724127f,1.30003900f,0.46169100f),   //Thumb: Distal
            new Vector3(0.07948533f,1.31722100f,0.47677250f)    //Thumb: Tip
        },
        new Vector3[]
        {
            new Vector3(0.09267072f,1.25601400f,0.49023730f),   //Index: Meta
            new Vector3(0.10108240f,1.25228700f,0.53447720f),   //Index: Proximal
            new Vector3(0.10207010f,1.24941800f,0.56684520f),   //Index: Distal
            new Vector3(0.10056160f,1.24804000f,0.58297150f)    //Index: Tip
        },
        new Vector3[]
        {
            new Vector3(0.09798656f,1.23559200f,0.49496120f),   //Middle: Meta
            new Vector3(0.10227280f,1.23202700f,0.54387940f),   //Middle: Proximal
            new Vector3(0.09721236f,1.23143400f,0.58253500f),   //Middle: Distal
            new Vector3(0.09467520f,1.23113600f,0.60191580f)    //Middle: Tip
        },
        new Vector3[]
        {
            new Vector3(0.09178711f,1.19834500f,0.49671640f),   //Ring: Meta
            new Vector3(0.09660194f,1.21181700f,0.53773080f),   //Ring: Proximal
            new Vector3(0.09643099f,1.21439800f,0.57437940f),   //Ring: Distal
            new Vector3(0.09436695f,1.21574000f,0.59258350f)    //Ring: Tip
        },
        new Vector3[]
        {
            new Vector3(0.08232284f,1.18392200f,0.49217780f),   //Pinky: Meta
            new Vector3(0.09303662f,1.19093500f,0.52871720f),   //Pinky: Proximal
            new Vector3(0.09585937f,1.19362600f,0.55843430f),   //Pinky: Distal
            new Vector3(0.08797413f,1.19514500f,0.57108720f)    //Pinky: Tip
        }
        };
        private readonly Vector3[][] rightHandMovementPose = new Vector3[][]
        {
        new Vector3[]
        {
            new Vector3(0f,0f,0f),                              //Arm: Shoulder
            new Vector3(0f,0f,0f),                              //Arm: Elbow
            new Vector3(0.16315495f,1.29131300f,0.48968805f)    //Arm: Wrist
        },
        new Vector3[]
        {
            new Vector3(0.14331820f,1.32779000f,0.48344720f),   //Thumb: Meta
            new Vector3(0.08590446f,1.29174700f,0.51586500f),   //Thumb: Proximal
            new Vector3(0.07088860f,1.27728700f,0.55389120f),   //Thumb: Distal
            new Vector3(0.06102182f,1.26778600f,0.57887770f),   //Thumb: Tip
        },
        new Vector3[]
        {
            new Vector3(0.11659790f,1.29997300f,0.56616940f),   //Index: Meta
            new Vector3(0.11600920f,1.31001200f,0.61846480f),   //Index: Proximal
            new Vector3(0.09161062f,1.29028000f,0.60831520f),   //Index: Distal
            new Vector3(0.07935945f,1.28037100f,0.60321890f),   //Index: Tip
        },
        new Vector3[]
        {
            new Vector3(0.13177770f,1.28736300f,0.57070920f),   //Middle: Meta
            new Vector3(0.13834220f,1.27939300f,0.62873260f),   //Middle: Proximal
            new Vector3(0.10192630f,1.26458400f,0.62218630f),   //Middle: Distal
            new Vector3(0.08635801f,1.25977200f,0.61071700f),   //Middle: Tip
        },
        new Vector3[]
        {
            new Vector3(0.15001830f,1.24969600f,0.57128360f),   //Ring: Meta
            new Vector3(0.13789060f,1.25172600f,0.61854850f),   //Ring: Proximal
            new Vector3(0.11065770f,1.24618600f,0.59329650f),   //Ring: Distal
            new Vector3(0.09672185f,1.24335100f,0.58037440f),   //Ring: Tip
        },
        new Vector3[]
        {
            new Vector3(0.14992970f,1.23497900f,0.56470690f),   //Pinky: Meta
            new Vector3(0.12871360f,1.22194600f,0.59181360f),   //Pinky: Proximal
            new Vector3(0.11024890f,1.23020300f,0.56943290f),   //Pinky: Distal
            new Vector3(0.10600140f,1.23759400f,0.55699010f)    //Pinky: Tip
        }
        };
        private readonly Vector3[][] leftHandCalibrationPose = new Vector3[][]
        {
        new Vector3[]
        {
            new Vector3(0f,0f,0f),                              //Arm: Shoulder
            new Vector3(0f,0f,0f),                              //Arm: Elbow
            new Vector3(-0.34224420f,1.23466250f,0.40806675f)   //Arm: Wrist
        },
        new Vector3[]
        {
            new Vector3(-0.34152050f,1.26328600f,0.39835940f),  //Thumb: Meta
            new Vector3(-0.30904090f,1.28177600f,0.42919600f),  //Thumb: Proximal
            new Vector3(-0.30684350f,1.31134800f,0.45598040f),  //Thumb: Distal
            new Vector3(-0.30553940f,1.32889800f,0.47187630f),  //Thumb: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.32914230f,1.26532600f,0.48951300f),  //Index: Meta
            new Vector3(-0.33294980f,1.26374800f,0.53643190f),  //Index: Proximal
            new Vector3(-0.33157540f,1.26125400f,0.56911050f),  //Index: Distal
            new Vector3(-0.32723900f,1.25991600f,0.58487140f),  //Index: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.33434060f,1.24713300f,0.49644770f),  //Middle: Meta
            new Vector3(-0.33343200f,1.24427600f,0.54538420f),  //Middle: Proximal
            new Vector3(-0.32892210f,1.24450300f,0.58294910f),  //Middle: Distal
            new Vector3(-0.32001900f,1.24440700f,0.59964050f),  //Middle: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.33361440f,1.21362800f,0.49689780f),  //Ring: Meta
            new Vector3(-0.33185880f,1.22415700f,0.54048790f),  //Ring: Proximal
            new Vector3(-0.32702620f,1.23030800f,0.57453510f),  //Ring: Distal
            new Vector3(-0.32459010f,1.23340900f,0.59169880f),  //Ring: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.32637970f,1.19755900f,0.49260370f),  //Pinky: Meta
            new Vector3(-0.32846720f,1.20579300f,0.53160890f),  //Pinky: Proximal
            new Vector3(-0.32676630f,1.21110200f,0.55810690f),  //Pinky: Distal
            new Vector3(-0.32395010f,1.21369200f,0.57109380f)   //Pinky: Tip
        }
        };
        private readonly Vector3[][] leftHandMovementPose = new Vector3[][]
        {
        new Vector3[]
        {
            new Vector3(0f,0f,0f),                              //Arm: Shoulder
            new Vector3(0f,0f,0f),                              //Arm: Elbow
            new Vector3(-0.37114570f,1.24518650f,0.41403575f)   //Arm: Wrist
        },
        new Vector3[]
        {
            new Vector3(-0.37449880f,1.27370700f,0.40545480f),  //Thumb: Meta
            new Vector3(-0.33039870f,1.29669600f,0.42957300f),  //Thumb: Proximal
            new Vector3(-0.30653770f,1.28826400f,0.46283210f),  //Thumb: Distal
            new Vector3(-0.29127890f,1.28287100f,0.48410080f),  //Thumb: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.34644810f,1.27858300f,0.49169360f),  //Index: Meta
            new Vector3(-0.35453520f,1.28344000f,0.54394360f),  //Index: Proximal
            new Vector3(-0.32154700f,1.28678100f,0.54249940f),  //Index: Distal
            new Vector3(-0.30442140f,1.28851600f,0.54174970f),  //Index: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.34835990f,1.25695200f,0.49905430f),  //Middle: Meta
            new Vector3(-0.34413130f,1.25703100f,0.55212900f),  //Middle: Proximal
            new Vector3(-0.30720320f,1.26515200f,0.55073020f),  //Middle: Distal
            new Vector3(-0.28867380f,1.26922600f,0.55002840f),  //Middle: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.34490920f,1.22747600f,0.49848700f),  //Ring: Meta
            new Vector3(-0.33839880f,1.23166300f,0.54758860f),  //Ring: Proximal
            new Vector3(-0.30614840f,1.24375900f,0.53933360f),  //Ring: Distal
            new Vector3(-0.28914710f,1.25013600f,0.53498200f),  //Ring: Tip
        },
        new Vector3[]
        {
            new Vector3(-0.33710140f,1.21120400f,0.49326910f),  //Pinky: Meta
            new Vector3(-0.32083360f,1.20585300f,0.53010180f),  //Pinky: Proximal
            new Vector3(-0.29784060f,1.21868300f,0.52077880f),  //Pinky: Distal
            new Vector3(-0.28541810f,1.22561500f,0.51574180f)   //Pinky: Tip
        }
        };

    }

}