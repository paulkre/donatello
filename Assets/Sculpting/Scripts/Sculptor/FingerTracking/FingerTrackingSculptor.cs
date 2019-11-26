using FingerTracking;
using FingerTracking.UI.Picker;

namespace VRSculpting.Sculptor.FingerTracking
{

    public class FingerTrackingSculptor : SculptorBehaviour
    {
        public TrackedHand rightHand;
        public PickerBehaviour picker;

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
