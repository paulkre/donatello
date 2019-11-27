﻿using System.Collections;
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
            Vector3 fingerTip = hand.GetWorldPosition(finger, 3);


            Vector3 lastPoint = point;
            point = (thumbTip + fingerTip) / 2;
            delta = point - lastPoint;
            state = Vector3.Distance(thumbTip, fingerTip) <= threshold;
            stateDown = !lastState && state;
            stateUp = lastState && !state;

            lastState = state;

            if (stateDown) OnDown?.Invoke(point, delta);
            if (stateUp) OnUp?.Invoke(point, delta);
        }
    }

}

