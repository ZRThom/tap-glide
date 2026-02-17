using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "CustomSettings", menuName = "ScriptableObjects/CustomSettings")]
public class CustomSettings : ScriptableObject
{
    public List<string> devs; 

    public static CustomSettings Load()
    {
        return Resources.Load<CustomSettings>("CustomSettings");
    }

#if UNITY_EDITOR
    public static SerializedObject SerializedSettings()
    {
        return new SerializedObject(Load());
    }
#endif
}