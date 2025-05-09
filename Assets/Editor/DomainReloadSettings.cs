using UnityEditor;
using UnityEngine;

public class DomainReloadSettings
{
    [InitializeOnLoadMethod]
    static void SetDomainReload()
    {
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
    }

    [MenuItem("Tools/Toggle Domain Reload")]
    static void ToggleDomainReload()
    {
        bool currentState = EditorSettings.enterPlayModeOptionsEnabled &&
                           (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) != 0;

        EditorSettings.enterPlayModeOptionsEnabled = true;

        if (currentState)
        {
            EditorSettings.enterPlayModeOptions &= ~EnterPlayModeOptions.DisableDomainReload;
            Debug.Log("Domain Reload enabled");
        }
        else
        {
            EditorSettings.enterPlayModeOptions |= EnterPlayModeOptions.DisableDomainReload;
            Debug.Log("Domain Reload disabled");
        }
    }
}