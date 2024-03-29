﻿using UnityEngine;
using UnityEditor;
using StateOfClone.Core;

/// <summary>
/// Drawer for <see cref="HexCoordinates"/> values in the inspector.
/// </summary>
[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer
{

    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label
    )
    {
        HexCoordinates coordinates = new HexCoordinates(
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("z").intValue
        );

        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, coordinates.ToString());
    }
}