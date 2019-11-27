using UnityEngine;

namespace FingerTracking.UI.Grip
{

    public class ExampleListener : MonoBehaviour
    {
        public GripBehaviour grip;

        private Vector3 lastHandPosition;

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();

            grip.OnDown += (point, delta) =>
            {
                rb.isKinematic = true;
                lastHandPosition = grip.hand.transform.position;
            };

            grip.OnUp += (point, delta) =>
            {
                rb.isKinematic = false;
            };
        }

        private void Update()
        {
            if (grip.State)
            {
                var handTransform = grip.hand.transform;
                transform.position += handTransform.position - lastHandPosition;
                lastHandPosition = handTransform.position;
            }
        }

        private void OnDrawGizmos()
        {
            if (grip == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(grip.Point, .01f);
        }
    }

}
