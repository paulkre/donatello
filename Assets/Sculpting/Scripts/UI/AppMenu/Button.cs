using UnityEngine;
using UnityEngine.UI;

namespace VRSculpting.UI.AppMenu
{

    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour
    {
        private static Color standardColor = new Color(.85f, .85f, .85f);
        private static Color enabledColor = new Color(.65f, .85f, .65f);
        private static float hoverColorInfl = .95f;

        public string label;

        public Text labelReference;

        public delegate void OnClickHandler();
        public event OnClickHandler OnClick;

        private Color baseColor;

        public void Click()
        {
            OnClick?.Invoke();
        }

        private bool hover;
        public bool Hover
        {
            get { return hover; }
            set
            {
                if (hover == value) return;
                hover = value;
                UpdateColor();
            }
        }

        private new bool enabled;
        public bool Enabled
        {
            set
            {
                if (value == enabled) return;
                enabled = value;
                baseColor = value ? enabledColor : standardColor;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            GetComponent<Image>().color = Hover ? baseColor : hoverColorInfl * baseColor;
        }

        private void Awake()
        {
            labelReference.text = label;

            var coll = gameObject.AddComponent<BoxCollider>();

            float width = (transform.parent.parent as RectTransform).sizeDelta.x;
            float height = (transform as RectTransform).sizeDelta.y;

            coll.size = new Vector3(width, height);

            Hover = true;
            Hover = false;

            baseColor = standardColor;
            UpdateColor();
        }

    }

}
