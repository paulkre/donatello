using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.UI.Grip
{

    public class GripBehaviour : MonoBehaviour
    {
        public float threshold = .02f;

        public TrackedHand hand;

        public delegate void Handler(Vector3 point, Vector3 delta);

        public event Handler OnDown;
        public event Handler OnUp;

        public Vector3 Point { get { UpdateState(); return point; } }
        public Vector3 Delta { get { UpdateState(); return delta; } }
        public bool State { get { UpdateState(); return state; } }
        public bool StateDown { get { UpdateState(); return stateDown; } }
        public bool StateUp { get { UpdateState(); return stateUp; } }

        private Vector3 point;
        private Vector3 delta;
        private bool state;
        private bool stateUp;
        private bool stateDown;

        private bool lastState;

        private int timeStamp;

        private void Update() { UpdateState(); }

        private void UpdateState()
        {
            if (Time.frameCount == timeStamp || !hand.Calibrated) return;
            timeStamp = Time.frameCount;

            Vector3 thumbTip = hand.GetWorldPosition(0, 3);
            Vector3 indexTip = hand.GetWorldPosition(1, 3);
            Vector3 middleTip = hand.GetWorldPosition(2, 3);

            float mag0 = (indexTip - thumbTip).magnitude;
            float mag1 = (middleTip - indexTip).magnitude;
            float mag2 = (thumbTip - middleTip).magnitude;

            Vector3 lastPoint = point;
            point = (thumbTip + indexTip + middleTip) / 3;
            delta = point - lastPoint;
            state = mag0 <= threshold && mag1 <= threshold && mag2 <= threshold;
            stateDown = !lastState && state;
            stateUp = lastState && !state;

            lastState = state;

            if (stateDown) OnDown?.Invoke(point, delta);
            if (stateUp) OnUp?.Invoke(point, delta);
        }
    }

}
