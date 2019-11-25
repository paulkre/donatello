using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.UI.Picker
{

    public class PickerBehaviour : MonoBehaviour
    {
        public float threshold = .02f;

        public TrackedHand hand;

        [Range(1, 3)]
        public int finger = 1;

        public delegate void Handler();

        public event Handler OnDown;
        public event Handler OnUp;

        public bool State { get { UpdateState(); return state; } }
        public bool StateDown { get { UpdateState(); return stateDown; } }
        public bool StateUp { get { UpdateState(); return stateUp; } }

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
            Vector3 fingerTip = hand.GetWorldPosition(finger, 3);

            state = Vector3.Distance(thumbTip, fingerTip) <= threshold;
            stateDown = !lastState && state;
            stateUp = lastState && !state;

            lastState = state;

            if (stateDown) OnDown?.Invoke();
            if (stateUp) OnUp?.Invoke();
        }
    }

}

