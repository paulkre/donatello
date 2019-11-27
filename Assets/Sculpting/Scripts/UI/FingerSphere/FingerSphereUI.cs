using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSculpting.Settings;

namespace VRSculpting.UI.FingerSphere
{

    public class FingerSphereUI : UI
    {
        public FingerTracking.TrackedHand hand;

        private Vector3[] fingerTips;

        private Menu menu;

        public Vector3 Point { get; private set; }

        public override void Init(Menu menu)
        {
            this.menu = menu;
            fingerTips = new Vector3[5];
        }

        private void Update()
        {
            for (int i = 0; i < fingerTips.Length; i++)
                fingerTips[i] = hand.GetWorldPosition(i, 3);

            Vector3 mid = Vector3.zero;
            for (int i = 0; i < fingerTips.Length; i++)
                mid += fingerTips[i];
            mid /= fingerTips.Length;

            mid += (fingerTips[0] - mid) / 4;

            float radius = 0;
            for (int i = 0; i < fingerTips.Length; i++)
                radius += Vector3.Distance(mid, fingerTips[i]);
            radius /= fingerTips.Length;

            transform.localPosition = Point = mid;
            transform.localScale = 2 * radius * Vector3.one;

            menu.ToolSize.Value = 2 * radius;
        }

    }

}

