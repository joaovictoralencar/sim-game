using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private SerializedProperty _iconProperty;

    private void OnEnable()
    {
        _iconProperty = serializedObject.FindProperty("_icon");
    }

    public override void OnInspectorGUI()
    {
        // Exclude the icon and script properties.
        serializedObject.Update();

        // Show the script field as read-only.
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ItemData)target), typeof(ItemData),
            false);
        EditorGUI.EndDisabledGroup();
        
        DrawPropertiesExcluding(serializedObject, "_icon", "m_Script");


        // Draw the icon property separately.
        EditorGUILayout.Space();
        _iconProperty.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField(
            "Icon", _iconProperty.objectReferenceValue, typeof(Sprite), false);
        serializedObject.ApplyModifiedProperties();
    }
}