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
                    UnitData unitData = actionSelector.GetComponent<Unit>().UnitData;
                    Locomotion locomotion = actionSelector.GetComponent<Locomotion>();

                    // Create a new instance of the selected behavior
                    ISteeringBehavior newBehavior =
                        (ISteeringBehavior)Activator.CreateInstance(
                            _steeringTypes[_selectedOption], unitData, locomotion
                            );

                    // Add the new behavior to the ActionSelector
                    actionSelector.AddBehavior(newBehavior);
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
