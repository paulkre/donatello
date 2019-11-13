using System;
using UnityEngine;

namespace VRSculpting.Settings
{

    public class Parameter
    {
        public string Name { get; private set; }

        public string Label { get; private set; }

        public delegate void OnChangeHandler(Parameter parameter, float lastValue);

        public event OnChangeHandler OnChange;

        private float initialValue;
        public float RelativeValue
        {
            get { return Value / initialValue; }
            set { Value = value * initialValue; }
        }

        private float value;
        public float Value
        {
            get { return value; }
            set
            {
                if (this.value == value) return;

                float lastValue = this.value;

                this.value = Mathf.Max(0, value);

                OnChange?.Invoke(this, lastValue);
            }
        }

        public Parameter(string name, string label, float initialValue)
        {
            Name = name;
            Label = label;
            this.initialValue = value = initialValue;
        }

    }

}
