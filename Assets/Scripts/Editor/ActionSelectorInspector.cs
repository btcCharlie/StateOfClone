using System;
using UnityEditor;
using UnityEngine;
using StateOfClone.Units;

namespace StateOfClone
{
    [CustomEditor(typeof(ActionSelector))]
    public class ActionSelectorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            ActionSelector actionSelector = (ActionSelector)target;

            string[] options = new string[] { "SteeringSeek", "SteeringArrival" };
            int currentOption =
                actionSelector.CurrentBehavior != null ?
                Array.IndexOf(
                    options, actionSelector.CurrentBehavior.GetType().Name
                    ) :
                -1;
            int selectedOption = EditorGUILayout.Popup("Steering Behavior", currentOption, options);

            if (selectedOption != currentOption)
            {
                switch (selectedOption)
                {
                    case 0:
                        actionSelector.SetBehavior(typeof(SteeringSeek));
                        break;
                    case 1:
                        actionSelector.SetBehavior(typeof(SteeringArrival));
                        break;
                }
            }

            DrawDefaultInspector();
        }
    }
}
