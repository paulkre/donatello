using UnityEngine;

namespace VRSculpting.UI.FingerMenu
{

    [RequireComponent(typeof(BoxCollider))]
    public class Button : MonoBehaviour
    {
        private static Vector3 standardColor = new Vector3(.85f, .85f, .85f);
        private static Vector3 activeColor = new Vector3(.65f, .65f, .85f);
        private static Vector3 enabledColor = new Vector3(.65f, .85f, .65f);
        private static float pressingColorInfl = .15f;

        public string label;
        public float travelDistance = 12f;

        public RectTransform plate;
        public UnityEngine.UI.Text labelElement;

        public delegate void ClickHandler();
        public event ClickHandler OnClick;

        private float activationThreshold;

        private float pressForce;
        private float pressForceGoal;

        private UnityEngine.UI.Image background;

        public bool Enabled { get; set; }

        private enum State
        {
            None,
            Pressing,
            Active
        }

        private State state;

        private bool initialized;

        public void Init()
        {
            var coll = GetComponent<BoxCollider>();
            var trm = transform as RectTransform;
            var parent = transform.parent as RectTransform;
            var vlg = parent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();

            coll.size = new Vector3(
                parent.rect.width - vlg.padding.left - vlg.padding.right,
                trm.rect.height,
                travelDistance
            );

            coll.center = Vector3.forward * .5f * travelDistance;
            activationThreshold = travelDistance * .8f;

            state = State.None;
            labelElement.text = label;
            background = plate.GetComponent<UnityEngine.UI.Image>();

            UpdateBgColor();

            initialized = true;
        }

        private void FixedUpdate()
        {
            if (!initialized) return;

            pressForce += (pressForceGoal - pressForce) / 2;
            plate.localPosition = Vector3.forward * pressForce;
            UpdateBgColor();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!initialized) return;

            var pointer = other as SphereCollider;
            var point = pointer.transform.position;
            var plane = new Plane(transform.forward, transform.position);
            pressForceGoal = (plane.GetDistanceToPoint(point) + pointer.transform.localScale.x * pointer.radius) / transform.lossyScale.x;

            if (pressForceGoal >= activationThreshold)
            {
                if (state == State.Pressing)
                {
                    state = State.Active;
                    OnClick?.Invoke();
                }
            }
            else state = State.Pressing;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!initialized) return;

            pressForceGoal = 0f;
            state = State.None;
        }

        private void UpdateBgColor()
        {
            var color = GetBaseColor() * (1f - pressingColorInfl * (pressForce / travelDistance));
            background.color = new Color(color.x, color.y, color.z);
        }

        private Vector3 GetBaseColor()
        {
            if (state == State.Active) return activeColor;
            else if (Enabled) return enabledColor;
            else return standardColor;
        }

    }

}
