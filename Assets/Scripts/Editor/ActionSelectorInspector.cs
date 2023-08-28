using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using StateOfClone.Units;

namespace StateOfClone
{
    [CustomEditor(typeof(ActionSelector))]
    public class ActionSelectorInspector : Editor
    {
        private SteeringType _steeringTypeSelection = 0;
        private bool _showBehaviors = true;

        public override void OnInspectorGUI()
        {
            ActionSelector actionSelector = (ActionSelector)target;

            _steeringTypeSelection = (SteeringType)EditorGUILayout.EnumPopup(
                "Add Steering Behavior", _steeringTypeSelection
                );

            if (GUILayout.Button("Add Behavior"))
            {
                // Add the selected behavior
                actionSelector.AddBehavior(_steeringTypeSelection);
            }

            // Display the list of behaviors in a collapsible box
            _showBehaviors = EditorGUILayout.Foldout(_showBehaviors, "Behaviors:");
            if (_showBehaviors)
            {
                foreach (ISteeringBehavior behavior in actionSelector.Behaviors)
                {
                    EditorGUILayout.LabelField("- " + behavior.GetType().Name);
                }

                if (GUILayout.Button("Remove Behavior"))
                {
                    // Add the new behavior to the ActionSelector
                    actionSelector.RemoveBehavior(_steeringTypeSelection);
                }
            }

            DrawDefaultInspector();
        }
    }
}
