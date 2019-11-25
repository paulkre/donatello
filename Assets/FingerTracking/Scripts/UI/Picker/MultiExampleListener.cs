using UnityEngine;

namespace FingerTracking.UI.Picker
{

    public class MultiExampleListener : MonoBehaviour
    {
        public MultiPickerBehaviour picker;

        private Vector3 lastHandPosition;

        private Color[] colors = new Color[]
        {
            Color.yellow,
            Color.green,
            Color.blue,
            Color.magenta
        };

        private void Start()
        {
            var renderer = GetComponent<Renderer>();

            SetQuadColor(1);

            picker.OnDown += id => SetQuadColor(id);
        }

        private void SetQuadColor(int fingerId)
        {
            GetComponent<Renderer>().material.color = colors[fingerId - 1];
        }
    }

}
