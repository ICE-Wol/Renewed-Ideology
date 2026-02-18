using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Prota.Unity;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Prota.Editor
{
    internal static class EnhancedInspectorSettings
    {
        
        // ====================================================================================================
        // ====================================================================================================
        
        internal static EditorPrefEntryBool useOriginalInspector = new("Prota:UseOriginalTransformInspector", true);
        [MenuItem("ProtaFramework/Functionality/Use Original Transform Inspector", false, priority = 2500)]
        static void ToggleEnhancedInspector()
        {
            useOriginalInspector.value = !useOriginalInspector.value;
            Menu.SetChecked("ProtaFramework/Functionality/Use Original Transform Inspector", useOriginalInspector.value);
        }
        [MenuItem("ProtaFramework/Functionality/Use Original Transform Inspector", true)]
        static bool ToggleEnhancedInspectorValidate()
        {
            Menu.SetChecked("ProtaFramework/Functionality/Use Original Transform Inspector", useOriginalInspector.value);
            return true;
        }
        
        
    }
}
