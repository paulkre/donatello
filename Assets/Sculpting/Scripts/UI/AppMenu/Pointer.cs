using UnityEngine;

namespace VRSculpting.UI.AppMenu
{

    [RequireComponent(typeof(LineRenderer))]
    public class Pointer : MonoBehaviour
    {

        private static float lineWidth = .001f;

        public float Length { set { transform.localScale = new Vector3(1, 1, value); } }

        public Vector3 Forward { get { return transform.forward; } }

        public bool Enabled
        {
            set { GetComponent<LineRenderer>().enabled = value; }
        }

        private void Awake()
        {
            var lr = GetComponent<LineRenderer>();
            lr.startWidth = lr.endWidth = lineWidth;
        }

    }

}
