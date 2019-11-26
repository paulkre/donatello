using FingerTracking;
using FingerTracking.UI.Picker;
using FingerTracking.UI.FingerSlider;
using VRSculpting.SculptMesh.Modification;
using VRSculpting.Settings;

namespace VRSculpting.Sculptor.FingerTracking
{
    using Tools;

    public class FingerTrackingSculptor : SculptorBehaviour
    {
        public TrackedHand rightHand;
        public TrackedHand leftHand;

        public PickerBehaviour pickerPrefab;
        public FingerSliderBehaviour sliderPrefab;
        public MultiPickerBehaviour multiPickerPrefab;

        private PickerBehaviour picker;
        private FingerSliderBehaviour slider;
        private MultiPickerBehaviour multiPicker;

        public override void Init(SculptMesh.Modification.SculptMesh sculptMesh, Menu menu)
        {
            base.Init(sculptMesh, menu);

            picker = Instantiate(pickerPrefab, transform);
            picker.hand = rightHand;

            slider = Instantiate(sliderPrefab, transform);
            slider.hand = rightHand;

            multiPicker = Instantiate(multiPickerPrefab, transform);
            multiPicker.hand = leftHand;

            slider.OnChange += (value, lastValue) =>
                menu.SelectedParameter.RelativeValue += value - lastValue;

            multiPicker.OnDown += fingerId =>
                menu.CurrentTool = MapFingerIdToTool(fingerId);
        }

        protected override SculptState GetState(SculptState prev)
        {
            return new SculptState
            {
                position = rightHand.transform.position,
                strength = 1f,

                drawing = picker.State,
                drawingDown = picker.StateDown,
                drawingUp = picker.StateUp,
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
