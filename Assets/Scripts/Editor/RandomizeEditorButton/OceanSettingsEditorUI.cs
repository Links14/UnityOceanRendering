using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OceanSettings))]
public class OceanSettingsEditorUI : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw Randomize button at the top
        OceanSettings settings = (OceanSettings)target;

        if (GUILayout.Button("Randomize Seed"))
        {
            Undo.RecordObject(settings, "Randomize Seed");
            settings.RandomizeSeed();
            EditorUtility.SetDirty(settings);
        }

        // Draw the normal inspector fields
        base.OnInspectorGUI();
    }
}
