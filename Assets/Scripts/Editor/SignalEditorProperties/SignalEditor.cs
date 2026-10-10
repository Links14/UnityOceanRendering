using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Signal))]
public class SignalEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Signal signal = target as Signal;
        if (GUILayout.Button("Post"))
        {
            signal.Post();
        }
    }
}
