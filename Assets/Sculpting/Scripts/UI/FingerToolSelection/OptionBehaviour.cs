using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.UI.FingerToolSelection
{

    public class OptionBehaviour : MonoBehaviour
    {
        public Renderer quad;

        public int FingerId { get; private set; }

        public void Init(OptionProps props, float size, Color color)
        {
            var mat = quad.material;
            mat.mainTexture = props.icon;
            mat.SetColor("_Color", color);

            quad.transform.localScale = size * Vector3.one;
            quad.transform.localPosition = size * Vector3.forward;

            FingerId = props.fingerId;
        }

        public void ManualUpdate()
        {
            quad.transform.rotation = Quaternion.LookRotation(
                transform.position - Camera.main.transform.position, Vector3.up
            );
        }
    }

}
