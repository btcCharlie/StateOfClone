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
        private List<Type> _steeringTypes;
        private string[] _steeringTypeNames;
        private int _selectedOption = -1;
        private bool _showBehaviors = true;

        public override void OnInspectorGUI()
        {
            ActionSelector actionSelector = (ActionSelector)target;

            _selectedOption = EditorGUILayout.Popup(
                "Add Steering Behavior", _selectedOption, _steeringTypeNames
                );

            if (GUILayout.Button("Add Behavior"))
            {
                if (_selectedOption >= 0)
                {
                    // Add the selected behavior
                    actionSelector.AddBehavior(_steeringTypeNames[_selectedOption]);
                }
            }

            // // Display the list of behaviors
            // EditorGUILayout.LabelField("Behaviors:");
            // foreach (ISteeringBehavior behavior in actionSelector.Behaviors)
            // {
            //     EditorGUILayout.SelectableLabel("- " + behavior.GetType().Name);
            // }

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
                    if (_selectedOption >= 0)
                    {
                        // Add the new behavior to the ActionSelector
                        actionSelector.RemoveBehavior(_steeringTypeNames[_selectedOption]);
                    }
                }
            }

            DrawDefaultInspector();
        }

        private void OnEnable()
        {
            // Get all types that implement ISteeringBehavior
            _steeringTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (
                        typeof(ISteeringBehavior).IsAssignableFrom(type) &&
                        !type.IsInterface &&
                        type != typeof(SteeringBehavior)
                        )
                    {
                        _steeringTypes.Add(type);
                    }
                }
            }

            // Get the names of the types
            _steeringTypeNames = new string[_steeringTypes.Count];
            for (int i = 0; i < _steeringTypes.Count; i++)
            {
                _steeringTypeNames[i] = _steeringTypes[i].Name;
            }
        }
    }
}
