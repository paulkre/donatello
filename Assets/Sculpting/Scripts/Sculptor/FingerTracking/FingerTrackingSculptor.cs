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

        public override void Init(SculptMesh.Modification.SculptMesh sculptMesh, Menu menu)
        {
            base.Init(sculptMesh, menu);

            fingerSphere.hand = rightHand;

            picker = Instantiate(pickerPrefab, transform);
            picker.hand = rightHand;

            slider = Instantiate(sliderPrefab, transform);
            slider.hand = rightHand;

            rightGrip = Instantiate(gripPrefab, transform);
            rightGrip.hand = rightHand;

            multiPicker = Instantiate(multiPickerPrefab, transform);
            multiPicker.hand = leftHand;

            leftGrip = Instantiate(gripPrefab, transform);
            leftGrip.hand = leftHand;

            slider.OnChange += (value, lastValue) =>
                menu.SelectedParameter.RelativeValue += value - lastValue;

            multiPicker.OnDown += fingerId =>
                menu.CurrentTool = MapFingerIdToTool(fingerId);

            transformInputManager = new TransformInputManager(sculptMesh.Wrapper);
        }

        protected override SculptState GetState(SculptState prev)
        {
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

            bool isTransforming = rightGrip.State || leftGrip.State;

            return new SculptState
            {
                position = fingerSphere.Point,
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
