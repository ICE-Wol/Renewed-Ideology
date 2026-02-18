using UnityEngine;
using UnityEditor;

namespace Prota.Editor
{
    /// <summary>
    /// GUI工具类,提供常用的GUI操作功能
    /// </summary>
    public static class GUIUtils
    {
        /// <summary>
        /// 取消GUI聚焦状态
        /// 在按钮点击后调用此方法可以取消当前的GUI聚焦状态
        /// </summary>
        public static void ClearFocus()
        {
            GUI.FocusControl(null);
            EditorGUI.FocusTextInControl(null);
            GUIUtility.keyboardControl = 0;
        }
        
        /// <summary>
        /// 取消GUI聚焦状态并刷新GUI
        /// 在按钮点击后调用此方法可以取消当前的GUI聚焦状态并强制刷新
        /// </summary>
        public static void ClearFocusAndRepaint()
        {
            ClearFocus();
            GUI.changed = true;
            RepaintAllWindows();
        }
        
        /// <summary>
        /// 刷新所有窗口
        /// </summary>
        public static void RepaintAllWindows()
        {
            // 刷新当前窗口
            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }
            
            // 刷新Scene视图
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.Repaint();
            }
        }
        
        /// <summary>
        /// 检查是否应该取消聚焦
        /// 在按钮点击事件中调用,如果满足条件则自动取消聚焦
        /// </summary>
        /// <param name="autoClearFocus">是否自动取消聚焦</param>
        public static void HandleButtonClick(bool autoClearFocus = true)
        {
            if (autoClearFocus)
            {
                ClearFocus();
            }
        }
    }
}

