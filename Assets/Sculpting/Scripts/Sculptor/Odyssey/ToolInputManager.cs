namespace VRSculpting.Sculptor.Odyssey
{
    using Settings;

    public class ToolInputManager
    {

        private static float actionAreaThreshold = .85f;

        private Menu menu;

        private bool lastOffsetState;

        public ToolInputManager(Menu menu)
        {
            this.menu = menu;
        }

        public void ManageInput(float thumbstickLeftHorizontal)
        {
            int offset = 0;

            if (thumbstickLeftHorizontal > actionAreaThreshold) offset = 1;
            else if (thumbstickLeftHorizontal < -actionAreaThreshold) offset = -1;

            bool offsetState = offset != 0;
            bool offsetStateDown = offsetState && !lastOffsetState;

            if (offsetStateDown)
                menu.OffsetSelection(offset);

            lastOffsetState = offsetState;
        }

    }

}

