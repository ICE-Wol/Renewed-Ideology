using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    public static class LocalizationEditorCommands
    {
        [MenuItem("ProtaFramework/Localization/Reload Localization")]
        public static void ReloadLocalization()
        {
            LocalizationDatabase.Refresh();
            Debug.Log("Localization database reloaded.");
        }
    }
}
