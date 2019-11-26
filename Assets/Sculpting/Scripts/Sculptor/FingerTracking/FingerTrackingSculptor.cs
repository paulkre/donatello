using FingerTracking;
using FingerTracking.UI.Picker;
using FingerTracking.UI.FingerSlider;
using VRSculpting.SculptMesh.Modification;
using VRSculpting.Settings;

namespace VRSculpting.Sculptor.FingerTracking
{

    public class FingerTrackingSculptor : SculptorBehaviour
    {
        public TrackedHand rightHand;

        public PickerBehaviour pickerPrefab;
        public FingerSliderBehaviour sliderPrefab;

        private PickerBehaviour picker;
        private FingerSliderBehaviour slider;

        public override void Init(SculptMesh.Modification.SculptMesh sculptMesh, Menu menu)
        {
            base.Init(sculptMesh, menu);

            picker = Instantiate(pickerPrefab, transform);
            picker.hand = rightHand;

            slider = Instantiate(sliderPrefab, transform);
            slider.hand = rightHand;

            slider.OnChange += (value, lastValue) =>
            {
                menu.SelectedParameter.RelativeValue += value - lastValue;
            };
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

    }
}
