using UnityEditor;
using UnityEngine;

public class PickableEditorWindow : EditorWindow
{
    private const string PICKABLE_PATH = "Assets/Resources/PickablesDatas/";

    private PickableData pickableData;
    private string pickableName;
    private string pickableDescription;
    private PickableType pickableType;
    private int pickableValue;
    private WeaponType weaponType;
    private PatronType patronType;

    private PickableData existingPickableData;
    private int existingPickableValue;
    private WeaponType existingWeaponType;
    private PatronType existingPatronType;
    private string existingPickableName;
    private string existingPickableDescription;
    private PickableType existingPickableType;

    private bool isPickableDataLoaded = false;

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
        pickableType = (PickableType)EditorGUILayout.EnumPopup("Type", pickableType);

        if (pickableType == PickableType.Health)
        {
            EditorGUILayout.HelpBox("Health value must be between 10 and 50", MessageType.Info);
            pickableValue = EditorGUILayout.IntField("Health Value", pickableValue);
            pickableValue = Mathf.Clamp(pickableValue, 10, 50);
            weaponType = WeaponType.None;
            patronType = PatronType.None;
        }
        else if (pickableType == PickableType.Magazines)
        {
            EditorGUILayout.HelpBox("Magazine count must be between 1 and 10", MessageType.Info);
            pickableValue = EditorGUILayout.IntField("Magazine Count", pickableValue);
            pickableValue = Mathf.Clamp(pickableValue, 1, 10);
            weaponType = WeaponType.None;
            patronType = PatronType.None;
        }
        else if (pickableType == PickableType.Weapon)
        {
            weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weaponType);
            pickableValue = 1;
            patronType = PatronType.None;

            if (weaponType == WeaponType.None)
            {
                EditorGUILayout.HelpBox("Weapon type must be selected", MessageType.Info);
            }
        }
        else if (pickableType == PickableType.Patron)
        {
            patronType = (PatronType)EditorGUILayout.EnumPopup("Patron Type", patronType);
            pickableValue = 1;
            weaponType = WeaponType.None;

            if (patronType == PatronType.None)
            {
                EditorGUILayout.HelpBox("Patron type must be selected", MessageType.Info);
            }
        }

        if (GUILayout.Button("Save"))
        {
            SavePickableData();
        }
        #endregion

        #region Edit Existing Pickable Data
        GUILayout.Label("Edit Existing Pickable Data", EditorStyles.boldLabel);
        existingPickableData = (PickableData)EditorGUILayout.ObjectField("Existing Pickable Data", existingPickableData, typeof(PickableData), false);
        if (existingPickableData != null && !isPickableDataLoaded)
        {
            LoadExistingData();
        }

        if (isPickableDataLoaded)
        {
            existingPickableName = EditorGUILayout.TextField("Name", existingPickableName);
            existingPickableDescription = EditorGUILayout.TextField("Description", existingPickableDescription);
            existingPickableType = (PickableType)EditorGUILayout.EnumPopup("Type", existingPickableType);

            if (existingPickableType == PickableType.Health)
            {
                EditorGUILayout.HelpBox("Health value must be between 10 and 50", MessageType.Info);
                existingPickableValue = EditorGUILayout.IntField("Health Value", existingPickableValue);
                existingPickableValue = Mathf.Clamp(existingPickableValue, 10, 50);
                existingWeaponType = WeaponType.None;
                existingPatronType = PatronType.None;
            }
            else if (existingPickableType == PickableType.Magazines)
            {
                existingPickableValue = EditorGUILayout.IntField("Magazine Count", existingPickableValue);
                EditorGUILayout.HelpBox("Magazine count must be between 1 and 10", MessageType.Info);
                existingPickableValue = Mathf.Clamp(existingPickableValue, 1, 10);
                existingWeaponType = WeaponType.None;
                existingPatronType = PatronType.None;
            }
            else if (existingPickableType == PickableType.Weapon)
            {
                existingWeaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", existingWeaponType);

                if (existingWeaponType == WeaponType.None)
                {
                    EditorGUILayout.HelpBox("Weapon type must be selected", MessageType.Info);
                }

                existingPickableValue = 1;
                existingPatronType = PatronType.None;
            }
            else if (existingPickableType == PickableType.Patron)
            {
                existingPatronType = (PatronType)EditorGUILayout.EnumPopup("Patron Type", existingPatronType);

                if (existingPatronType == PatronType.None)
                {
                    EditorGUILayout.HelpBox("Patron type must be selected", MessageType.Info);
                }

                existingPickableValue = 1;
                existingWeaponType = WeaponType.None;
            }

            if (GUILayout.Button("Save Changes"))
            {
                SaveExistingPickableData();
            }
        }

        void LoadExistingData()
        {
            existingPickableName = existingPickableData.pickableName;
            existingPickableDescription = existingPickableData.pickableDescription;
            existingPickableType = existingPickableData.pickableType;
            existingPickableValue = existingPickableData.pickableValue;
            existingWeaponType = existingPickableData.weaponType;
            existingPatronType = existingPickableData.patronType;
            isPickableDataLoaded = true;
        }
        #endregion
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
        existingPickableData.pickableType = existingPickableType;
        existingPickableData.pickableValue = existingPickableValue;
        existingPickableData.weaponType = existingWeaponType;
        existingPickableData.patronType = existingPatronType;

        EditorUtility.SetDirty(existingPickableData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        isPickableDataLoaded = false;
    }

    private void SavePickableData()
    {
        if (string.IsNullOrEmpty(pickableName))
        {
            EditorGUILayout.HelpBox("Pickable name cannot be empty.", MessageType.Error);
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
        pickableData.pickableType = pickableType;
        pickableData.pickableValue = pickableValue;
        pickableData.weaponType = weaponType;
        pickableData.patronType = patronType;
        AssetDatabase.CreateAsset(pickableData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
    }
}