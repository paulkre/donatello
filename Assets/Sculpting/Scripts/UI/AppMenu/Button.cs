using UnityEngine;
using UnityEngine.UI;

namespace VRSculpting.UI.AppMenu
{

    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour
    {

        public string label;

        public Text labelReference;

        public delegate void OnClickHandler();
        public event OnClickHandler OnClick;

        public void Click()
        {
            OnClick?.Invoke();
        }

        private bool hover;
        public bool Hover
        {
            set
            {
                if (hover == value) return;
                hover = value;
                GetComponent<Image>().color = value
                    ? new Color(.85f, .85f, .85f)
                    : new Color(.9f, .9f, .9f);
            }
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
        }

    }

}
