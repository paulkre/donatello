using UnityEngine;

namespace VRSculpting.Sculptor.Odyssey
{
    using Settings;

    public class ParameterInputManager
    {

        private static float inputStrength = .02f;
        private static float inputThreshold = .05f;

        private Menu menu;

        private bool lastOffsetState;

        public ParameterInputManager(Menu menu)
        {
            this.menu = menu;
        }

        public void ManageInput(float thumbstickRightHorizontal)
        {
            if (Mathf.Abs(thumbstickRightHorizontal) < inputThreshold) return;

            menu.SelectedParameter.RelativeValue += inputStrength * thumbstickRightHorizontal;
        }

    }

}

