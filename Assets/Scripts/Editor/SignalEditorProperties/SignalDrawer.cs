using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Signal))]
public class SignalDrawerer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect propertyFieldRect = new(position.x, position.y, position.width - 18, position.height);
        EditorGUI.PropertyField(propertyFieldRect, property, label);

        Rect postButtonRect = new(position.width, position.y, 22, position.height);
        if (GUI.Button(postButtonRect, new GUIContent("\u26B6"))) {
            Signal signal = property.objectReferenceValue as Signal;
            signal.Post();
        }
    }
}
