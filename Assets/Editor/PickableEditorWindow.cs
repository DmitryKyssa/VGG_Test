using UnityEditor;
using UnityEngine;

public class PickableEditorWindow : EditorWindow
{
    private PickableData pickableData;
    private string pickableName;
    private string pickableDescription;
    private Sprite pickableIcon;
    private PickableType pickableType;
    private int pickableValue;

    [MenuItem("Window/Pickable Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<PickableEditorWindow>("Pickable Data Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Pickable Data", EditorStyles.boldLabel);

        pickableName = EditorGUILayout.TextField("Name", pickableData.pickableName);
        pickableDescription = EditorGUILayout.TextField("Description", pickableData.pickableDescription);
        pickableIcon = (Sprite)EditorGUILayout.ObjectField("Icon", pickableData.pickableIcon, typeof(Sprite), false);
        pickableType = (PickableType)EditorGUILayout.EnumPopup("Type", pickableData.pickableType);

        if (pickableType == PickableType.Weapon)
        {
        }

        pickableValue = EditorGUILayout.IntField("Value", pickableData.pickableValue);

        if (GUILayout.Button("Save"))
        {
            SavePickableData();
        }
    }

    private void SavePickableData()
    {
        if (pickableData != null)
        {
            pickableData.pickableName = pickableName;
            pickableData.pickableDescription = pickableDescription;
            pickableData.pickableIcon = pickableIcon;
            pickableData.pickableType = pickableType;
            pickableData.pickableValue = pickableValue;

            EditorUtility.SetDirty(pickableData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}