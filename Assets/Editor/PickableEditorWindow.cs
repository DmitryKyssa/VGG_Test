using UnityEditor;
using UnityEngine;

public class PickableEditorWindow : EditorWindow
{
    private const string PICKABLE_PATH = "Assets/Resources/Pickables/";

    private PickableData pickableData;
    private string pickableName;
    private string pickableDescription;
    private Sprite pickableIcon;
    private PickableType pickableType;
    private int pickableValue;
    private WeaponType weaponType;

    private PickableData existingPickableData;
    private int existingPickableValue;
    private WeaponType existingWeaponType;
    private string existingPickableName;
    private string existingPickableDescription;
    private Sprite existingPickableIcon;
    private PickableType existingPickableType;

    [MenuItem("Tools/Pickable Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<PickableEditorWindow>("Pickable Data Editor");
    }

    private void OnGUI()
    {
        #region Create New Pickable Data
        GUILayout.Label("Pickable Data", EditorStyles.boldLabel);

        pickableName = EditorGUILayout.TextField("Name", pickableName);
        pickableDescription = EditorGUILayout.TextField("Description", pickableDescription);
        pickableIcon = (Sprite)EditorGUILayout.ObjectField("Icon", pickableIcon, typeof(Sprite), false);
        pickableType = (PickableType)EditorGUILayout.EnumPopup("Type", pickableType);

        if (pickableType == PickableType.Health)
        {
            pickableValue = EditorGUILayout.IntField("Health Value", pickableValue);
            pickableValue = Mathf.Clamp(pickableValue, 10, 50);
        }
        else if (pickableType == PickableType.Magazines)
        {
            pickableValue = EditorGUILayout.IntField("Magazine Count", pickableValue);
            pickableValue = Mathf.Clamp(pickableValue, 1, 10);
        }
        else if (pickableType == PickableType.Weapon)
        {
            weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
            pickableValue = 1;
        }

        if (GUILayout.Button("Save"))
        {
            SavePickableData();
        }
        #endregion

        #region Edit Existing Pickable Data
        GUILayout.Label("Edit Existing Pickable Data", EditorStyles.boldLabel);
        existingPickableData = (PickableData)EditorGUILayout.ObjectField("Existing Pickable Data", existingPickableData, typeof(PickableData), false);
        if (existingPickableData != null)
        {
            existingPickableName = existingPickableData.pickableName;
            existingPickableDescription = existingPickableData.pickableDescription;
            existingPickableIcon = existingPickableData.pickableIcon;
            existingPickableType = existingPickableData.pickableType;
            existingPickableValue = existingPickableData.pickableValue;
            existingWeaponType = existingPickableData.weaponType;

            existingPickableName = EditorGUILayout.TextField("Name", existingPickableName);
            existingPickableDescription = EditorGUILayout.TextField("Description", existingPickableDescription);
            existingPickableIcon = (Sprite)EditorGUILayout.ObjectField("Icon", existingPickableIcon, typeof(Sprite), false);
            existingPickableType = (PickableType)EditorGUILayout.EnumPopup("Type", existingPickableType);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (existingPickableType == PickableType.Health)
            {
                EditorGUILayout.HelpBox("Health value must be between 10 and 50", MessageType.Info); //Bug => This message is not shown
                existingPickableValue = EditorGUILayout.IntField("Health Value", existingPickableValue);              
                existingPickableValue = Mathf.Clamp(existingPickableValue, 10, 50);
            }
            else if (existingPickableType == PickableType.Magazines)
            {
                existingPickableValue = EditorGUILayout.IntField("Magazine Count", existingPickableValue);
                EditorGUILayout.HelpBox("Magazine count must be between 1 and 10", MessageType.Info);
                existingPickableValue = Mathf.Clamp(existingPickableValue, 1, 10);
            }
            else if (existingPickableType == PickableType.Weapon)
            {
                existingWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", existingWeaponType);
                EditorGUILayout.HelpBox("Weapon type must be selected", MessageType.Info);
                existingPickableValue = 1;
            }
            EditorGUILayout.EndVertical();  

            if (GUILayout.Button("Save Changes"))
            {
                SaveExistingPickableData();
            }
            #endregion
        }
    }

    private void SaveExistingPickableData()
    {
        if (existingPickableData == null)
        {
            Debug.LogError("Existing pickable data cannot be null.");
            EditorGUILayout.HelpBox("Existing pickable data cannot be null.", MessageType.Error);
            return;
        }

        existingPickableData.pickableName = existingPickableName;
        existingPickableData.pickableDescription = existingPickableDescription;
        existingPickableData.pickableIcon = existingPickableIcon;
        existingPickableData.pickableType = existingPickableType;
        existingPickableData.pickableValue = existingPickableValue;
        existingPickableData.weaponType = existingWeaponType;

        EditorUtility.SetDirty(existingPickableData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SavePickableData()
    {
        if (string.IsNullOrEmpty(pickableName) || pickableIcon == null)
        {
            Debug.LogError("Pickable name and icon cannot be empty.");
            EditorGUILayout.HelpBox("Pickable name and icon cannot be empty.", MessageType.Error);
            return;
        }

        if (!AssetDatabase.IsValidFolder(PICKABLE_PATH))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Pickables");
        }
        string path = AssetDatabase.GenerateUniqueAssetPath(PICKABLE_PATH + pickableName + ".asset");
        pickableData = CreateInstance<PickableData>();
        pickableData.pickableName = pickableName;
        pickableData.pickableDescription = pickableDescription;
        pickableData.pickableIcon = pickableIcon;
        pickableData.pickableType = pickableType;
        pickableData.pickableValue = pickableValue;
        pickableData.weaponType = weaponType;
        AssetDatabase.CreateAsset(pickableData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}