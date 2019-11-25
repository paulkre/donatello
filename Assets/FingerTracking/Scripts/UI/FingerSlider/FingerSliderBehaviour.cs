using UnityEngine;

namespace FingerTracking.UI.FingerSlider
{

    public class FingerSliderBehaviour : MonoBehaviour
    {
        public float maxPointerDistance = .02f;

        public TrackedHand hand;
        public int fingerIndex;

        private Vector3[] jointPositions;
        private FingerSliderProjector projector;

        public delegate void ChangeHandler(float value, float lastValue);
        public virtual event ChangeHandler OnChange;

        private float lastValue;

        private int timeStamp;

        private float value;
        public virtual float Value
        {
            get
            {
                UpdateValue();
                return value;
            }
        }

        private void Start()
        {
            jointPositions = new Vector3[4];
            projector = new FingerSliderProjector(jointPositions, maxPointerDistance);
        }

        private void Update()
        {
            UpdateValue();
        }

        protected void UpdateValue()
        {
            if (Time.frameCount == timeStamp) return;
            timeStamp = Time.frameCount;

            for (int i = 0; i < jointPositions.Length; i++)
                jointPositions[i] = hand.GetWorldPosition(fingerIndex, i);

            value = 1f - projector.GetValue(hand.GetWorldPosition(0, 3));

            if (!float.IsNaN(value) && lastValue != value)
                HandleValue(value, lastValue);

            lastValue = value;
        }

        protected virtual void HandleValue(float value, float lastValue)
        {
            OnChange?.Invoke(value, lastValue);
        }
    }

}
