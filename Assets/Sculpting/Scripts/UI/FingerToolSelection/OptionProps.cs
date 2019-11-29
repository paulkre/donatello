using UnityEngine;

namespace VRSculpting.UI.FingerToolSelection
{
    using Tools;

    [System.Serializable]
    public struct OptionProps
    {
        [SerializeField]
        public string label;

        [SerializeField]
        public ToolType tool;

        [SerializeField]
        public Texture icon;

        [SerializeField]
        public int fingerId;
    }

}
