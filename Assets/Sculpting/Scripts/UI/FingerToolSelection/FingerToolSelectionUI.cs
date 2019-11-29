using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.UI.FingerToolSelection
{
    using Settings;
    using Tools;

    public class FingerToolSelectionUI : UI
    {
        private static float optionSize = .04f;

        public FingerTracking.TrackedHand hand;

        public OptionBehaviour optionPrefab;
        public List<OptionProps> optionProps;

        private OptionBehaviour[] options;
        private int selectedId;

        private void Update()
        {
            foreach (var option in options)
            {
                option.transform.position = hand.GetWorldPosition(option.FingerId, 3);
                option.transform.localRotation = Quaternion.LookRotation(hand.transform.up, Vector3.up);
                option.ManualUpdate();
            }
        }

        public override void Init(Menu menu)
        {
            CreateOptions();

            //menu.OnToolChange += OnToolChange;
            //OnToolChange(menu.CurrentTool);
        }

        private void OnToolChange(ToolType currentTool)
        {
            selectedId = optionProps.FindIndex((props) => props.tool == currentTool);

            //for (int i = 0; i < options.Length; i++)
                //options[i].Alpha = i == selectedId ? 1 : .15f;
        }

        private void CreateOptions()
        {
            options = new OptionBehaviour[optionProps.Count];

            for (int i = 0; i < optionProps.Count; i++)
            {
                var option = Instantiate(optionPrefab, transform, false);
                option.Init(optionProps[i], optionSize);

                options[i] = option;
            }
        }

    }

}
