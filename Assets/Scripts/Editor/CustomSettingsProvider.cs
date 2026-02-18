using UnityEngine;
using UnityEditor;

public class CustomSettingsProvider : SettingsProvider
{
    private SerializedObject serializedSettings;
    public const string SETTINGS_PATH = "Project/Custom Settings";

    public CustomSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
        : base(path, scope) { }

    public override void OnGUI(string searchContext)
    {
        if (serializedSettings == null || serializedSettings.targetObject == null)
        {
            serializedSettings = CustomSettings.SerializedSettings();
        }

        if (serializedSettings != null)
        {
            serializedSettings.Update();
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("devs"), true);
            serializedSettings.ApplyModifiedProperties();
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateCustomSettingsProvider()
    {
        return new CustomSettingsProvider(SETTINGS_PATH, SettingsScope.Project);
    }
}