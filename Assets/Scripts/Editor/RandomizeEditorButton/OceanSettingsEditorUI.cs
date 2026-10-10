using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OceanSettings))]
public class OceanSettingsEditorUI : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal inspector fields
        base.OnInspectorGUI();

        OceanSettings settings = (OceanSettings)target;

        if (GUILayout.Button("Randomize"))
        {
            Undo.RecordObject(settings, "Randomize");
            settings.RandomizeSeed();
            EditorUtility.SetDirty(settings);
        }
    }
}
