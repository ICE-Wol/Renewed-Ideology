using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        static MethodInfo clearConsoleMethod;
        [MenuItem("ProtaFramework/Tools/Clear Console", priority = 100)]
        public static void ClearConsole()
        {
            if(clearConsoleMethod == null)
            {
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                clearConsoleMethod = type.GetMethod("Clear");
            }
            clearConsoleMethod.Invoke(null, null);
        }
    }
}
