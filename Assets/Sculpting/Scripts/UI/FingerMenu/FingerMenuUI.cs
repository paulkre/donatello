using UnityEngine;

using FingerTracking;

namespace VRSculpting.UI.FingerMenu
{

    public class FingerMenuUI : UI
    {
        private static float viewDistance = .5f;

        public TrackedHand hand;

        public Transform pointer;
        public Canvas canvas;

        public Button symmetryButton;
        public Button exportButton;

        private void SetEnabled(bool value)
        {
            canvas.enabled = value;
            pointer.GetComponent<Collider>().enabled = value;
        }

        public override void Init(Settings.Menu menu)
        {
            menu.AppMenuEnabled.OnChange += value => SetEnabled(value);
            SetEnabled(menu.AppMenuEnabled.Value);

            symmetryButton.Init();
            menu.SymmetryEnabled.OnChange += value => symmetryButton.Enabled = value;
            symmetryButton.Enabled = menu.SymmetryEnabled.Value;
            symmetryButton.OnClick += () => menu.SymmetryEnabled.Toggle();

            exportButton.Init();
            exportButton.OnClick += () => menu.ExportAction.Do();
        }

        private void Update()
        {
            var cam = Camera.main.transform;
            var trm = canvas.transform;
            var targetPosition = cam.position + viewDistance * cam.forward;
            trm.position += (targetPosition - trm.position) * .05f;
            trm.rotation = Quaternion.LookRotation(cam.forward, cam.up);

            pointer.position = hand.GetWorldPosition(1, 3);
        }
    }

}
