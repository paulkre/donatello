using UnityEngine;

namespace FingerTracking.UI.Picker
{

    public class MultiPickerBehaviour : MonoBehaviour
    {
        public float threshold = .02f;

        public TrackedHand hand;

        public delegate void Handler(int id);

        public event Handler OnDown;
        public event Handler OnUp;

        public int Selection { get { UpdateState(); return selection; } }
        public bool State { get { UpdateState(); return state; } }
        public bool StateDown { get { UpdateState(); return stateDown; } }
        public bool StateUp { get { UpdateState(); return stateUp; } }

        private int selection;
        private bool state;
        private bool stateUp;
        private bool stateDown;

        private bool lastState;

        private int timeStamp;

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (Time.frameCount == timeStamp || !hand.Calibrated) return;
            timeStamp = Time.frameCount;

            Vector3 thumbTip = hand.GetWorldPosition(0, 3);

            float smallestDistance = Mathf.Infinity;
            int closestFinger = -1;
            for (int i = 1; i < 5; i++)
            {
                Vector3 fingerTip = hand.GetWorldPosition(i, 3);
                float dist = Vector3.Distance(thumbTip, fingerTip);
                if (dist <= threshold && dist < smallestDistance)
                {
                    smallestDistance = dist;
                    closestFinger = i;
                }
            }

            selection = closestFinger;
            state = closestFinger > -1;
            stateDown = !lastState && state;
            stateUp = lastState && !state;

            lastState = state;

            if (stateDown) OnDown?.Invoke(selection);
            if (stateUp) OnUp?.Invoke(selection);
        }
    }

}

