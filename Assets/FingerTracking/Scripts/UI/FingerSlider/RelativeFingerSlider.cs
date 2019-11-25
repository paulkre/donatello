namespace FingerTracking.UI.FingerSlider
{

    public class RelativeFingerSlider : FingerSliderBehaviour
    {
        public bool zeroOrHigher = false;

        public override event ChangeHandler OnChange;

        public override float Value
        {
            get
            {
                UpdateValue();
                return relValue;
            }
        }

        private float lastRelValue;
        private float relValue;

        protected override void HandleValue(float value, float lastValue)
        {
            if (float.IsNaN(lastValue)) return;

            lastRelValue = relValue;
            relValue += value - lastValue;

            if (zeroOrHigher && relValue < 0)
                relValue = 0;

            if (relValue != lastRelValue)
                OnChange?.Invoke(relValue, lastRelValue);
        }
    }

}
