﻿using UnityEngine;

namespace FingerTracking.UI.Picker
{

    public class ExampleListener : MonoBehaviour
    {
        public PickerBehaviour picker;

        private Vector3 lastHandPosition;

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();

            picker.OnDown += (point, delta) =>
            {
                rb.isKinematic = true;
                lastHandPosition = picker.hand.transform.position;
            };

            picker.OnUp += (point, delta) =>
            {
                rb.isKinematic = false;
            };
        }

        private void Update()
        {
            if (picker.State)
            {
                var handTransform = picker.hand.transform;
                transform.position += handTransform.position - lastHandPosition;
                lastHandPosition = handTransform.position;
            }
        }
    }

}
