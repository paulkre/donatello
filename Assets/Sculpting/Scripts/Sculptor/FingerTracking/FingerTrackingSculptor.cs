using UnityEngine;
using FingerTracking;
using FingerTracking.UI.Picker;
using FingerTracking.UI.FingerSlider;
using FingerTracking.UI.Grip;

namespace VRSculpting.Sculptor.FingerTracking
{
    using Settings;
    using Tools;

    public class FingerTrackingSculptor : SculptorBehaviour
    {
        public TrackedHand rightHand;
        public TrackedHand leftHand;

        public UI.FingerSphere.FingerSphereUI fingerSphere;
        public UI.FingerMenu.FingerMenuUI fingerMenu;
        public UI.FingerToolSelection.FingerToolSelectionUI fingerToolSelection;

        public PickerBehaviour pickerPrefab;
        public FingerSliderBehaviour sliderPrefab;
        public MultiPickerBehaviour multiPickerPrefab;
        public GripBehaviour gripPrefab;

        private PickerBehaviour picker;
        private FingerSliderBehaviour slider;
        private MultiPickerBehaviour multiPicker;

        private GripBehaviour rightGrip;
        private GripBehaviour leftGrip;

        private TransformInputManager transformInputManager;

        private bool rightHandActive;
        private bool leftHandActive;

        public override void Init(SculptMesh.Modification.SculptMesh sculptMesh, Menu menu)
        {
            base.Init(sculptMesh, menu);

            rightHandActive = rightHand.gameObject.activeInHierarchy;
            leftHandActive = leftHand.gameObject.activeInHierarchy;

            if (rightHandActive)
            {
                fingerSphere.Init(menu);
                fingerSphere.hand = rightHand;

                fingerMenu.Init(menu);
                fingerMenu.hand = rightHand;

                picker = Instantiate(pickerPrefab, transform);
                picker.hand = rightHand;
                picker.finger = 4;
                picker.OnDown += (point, delta) => menu.AppMenuEnabled.Toggle();

                slider = Instantiate(sliderPrefab, transform);
                slider.hand = rightHand;
                slider.OnChange += (value, lastValue) =>
                    menu.SelectedParameter.RelativeValue += value - lastValue;

                rightGrip = Instantiate(gripPrefab, transform);
                rightGrip.hand = rightHand;
            }

            if (leftHandActive)
            {
                fingerToolSelection.Init(menu);
                fingerToolSelection.hand = leftHand;

                multiPicker = Instantiate(multiPickerPrefab, transform);
                multiPicker.hand = leftHand;
                multiPicker.OnDown += fingerId =>
                    menu.CurrentTool = MapFingerIdToTool(fingerId);

                leftGrip = Instantiate(gripPrefab, transform);
                leftGrip.hand = leftHand;
            }

            transformInputManager = new TransformInputManager(sculptMesh.Wrapper);
        }

        protected override SculptState GetState(SculptState prev)
        {
            bool bothHandsActive = rightHandActive && leftHandActive;

            if (bothHandsActive)
                transformInputManager.ManageInput(
                    rightGrip.Point,
                    leftGrip.Point,
                    rightGrip.State,
                    leftGrip.State,
                    rightGrip.StateDown,
                    leftGrip.StateDown,
                    rightGrip.StateUp,
                    leftGrip.StateUp
                );

            bool isTransforming = bothHandsActive && (rightGrip.State || leftGrip.State);

            return new SculptState
            {
                position = rightHandActive ? fingerSphere.Point : Vector3.zero,
                strength = 1f,

                drawing = !isTransforming && multiPicker.State,
                drawingDown = !isTransforming && multiPicker.StateDown,
                drawingUp = multiPicker.StateUp,
                drawingInverted = false,
            };
        }

        private ToolType MapFingerIdToTool(int fingerId)
        {
            switch (fingerId)
            {
                case 2:
                    return ToolType.Move;
                case 3:
                    return ToolType.Smooth;
                default:
                    return ToolType.Standard;
            }
        }

    }
}
