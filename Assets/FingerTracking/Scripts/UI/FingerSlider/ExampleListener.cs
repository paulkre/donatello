using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FingerTracking.UI.FingerSlider
{

    public class ExampleListener : MonoBehaviour
    {

        public FingerSliderBehaviour slider;
        public Renderer mesh;
        public Color color;
        public float strength = 3;

        private void Start()
        {
            mesh.material.color = .5f * color;
        }

        private void Update()
        {
            transform.localScale = new Vector3(1, 1f + strength * slider.Value, 1);
        }

    }

}

