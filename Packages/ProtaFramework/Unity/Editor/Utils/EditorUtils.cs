using UnityEngine;
using UnityEditor;
using Prota.Editor;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prota.Editor
{
    
    public partial struct Guard
    {
        
    }
    
    public static partial class ProtaEditorUtils
    {
        static GUILayoutOption[] seperateLineOptions = null;
        static GUILayoutOption[] verticalSeperateLineOptions = null;
    
        public static void SeperateLine(float width, Color color)
        {
            if(seperateLineOptions == null)
            {
                seperateLineOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true) };
            }
            
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(width, width, seperateLineOptions), color);
        }
        
        public static void SeperateLine(float width)
        {
            SeperateLine(width, new Color(.1f, .1f, .1f, 1f));
        }
        
        public static void VerticalSeperateLine(float width, Color color)
        {
            if(verticalSeperateLineOptions == null)
            {
                verticalSeperateLineOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false) };
            }
            
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(width, width, verticalSeperateLineOptions), color);
        }
        
        public static void VerticalSeperateLine(float width)
        {
            VerticalSeperateLine(width, new Color(.1f, .1f, .1f, 1f));
        }
        
        public static T WithoutSelectAll<T>(Func<T> guiCall)
        {
            bool preventSelection = Event.current.type != EventType.Repaint;

            Color oldCursorColor = GUI.skin.settings.cursorColor;

            if (preventSelection)
                GUI.skin.settings.cursorColor = new Color(0, 0, 0, 0);

            T value = guiCall();

            if (preventSelection)
                GUI.skin.settings.cursorColor = oldCursorColor;

            return value;
        }
        
        
        // =================================================================================================
        // =================================================================================================
        #region FileSystemListeners
        
        static List<Action> onUpdateInternal = new();
        [InitializeOnLoadMethod]
        static void InitFileListeners()
        {
            EditorApplication.update += () => {
                foreach(var action in onUpdateInternal) action();
            };
        }
        
        public static void AddFolderMapListener(DirectoryInfo from, DirectoryInfo to, DirectoryMapCallback onUpdate)
        {
            AddFolderMapListener(250, from, to, onUpdate);
        }
        
        public static void AddFileMapListener(FileInfo from, FileInfo to, FileMapCallback onUpdate)
        {
            AddFileMapListener(250, from, to, onUpdate);
        }
        
        public static void AddFolderListener(DirectoryInfo from, DirectoryCallback onUpdate)
        {
            AddFolderListener(250, from, onUpdate);
        }
        
        public static void AddFileListener(FileInfo from, FileCallback onUpdate)
        {
            AddFileListener(250, from, onUpdate);
        }
        
        public static void AddFolderMapListener(float checkDelayInMs, string from, string to, DirectoryMapCallback onUpdate)
        {
            AddFolderMapListener(checkDelayInMs, from.AsDirectoryInfo(), to.AsDirectoryInfo(), onUpdate);
        }
        
        public static void AddFileMapListener(float checkDelayInMs, string from, string to, FileMapCallback onUpdate)
        {
            AddFileMapListener(checkDelayInMs, from.AsFileInfo(), to.AsFileInfo(), onUpdate);
        }
        
        public static void AddFolderListener(float checkDelayInMs, string from, DirectoryCallback onUpdate)
        {
            AddFolderListener(checkDelayInMs, from.AsDirectoryInfo(), onUpdate);
        }
        
        public static void AddFileListener(float checkDelayInMs, string from, FileCallback onUpdate)
        {
            AddFileListener(checkDelayInMs, from.AsFileInfo(), onUpdate);
        }
        
        public static void AddFolderMapListener(string from, string to, DirectoryMapCallback onUpdate)
        {
            AddFolderMapListener(from.AsDirectoryInfo(), to.AsDirectoryInfo(), onUpdate);
        }
        
        public static void AddFileMapListener(string from, string to, FileMapCallback onUpdate)
        {
            AddFileMapListener(from.AsFileInfo(), to.AsFileInfo(), onUpdate);
        }
        
        public static void AddFolderListener(string from, DirectoryCallback onUpdate)
        {
            AddFolderListener(from.AsDirectoryInfo(), onUpdate);
        }
        
        public static void AddFileListener(string from, FileCallback onUpdate)
        {
            AddFileListener(from.AsFileInfo(), onUpdate);
        }
        
        
        
        public delegate void DirectoryMapCallback(DirectoryInfo from, DirectoryInfo to);
        static HashSet<DirectoryMapCallback> registeredDirs = new();
        public static void AddFolderMapListener(float checkDelayInMs, DirectoryInfo from, DirectoryInfo to, DirectoryMapCallback onUpdate)
        {
            if(registeredDirs.Contains(onUpdate))
            {
                Debug.LogError($"Folder listener already registered: {from}");
                return;
            }
            
            registeredDirs.Add(onUpdate);
            var delay = TimeSpan.FromMilliseconds(checkDelayInMs);
            var nextCheckTime = DateTime.MinValue;
            onUpdateInternal.Add(() => {
                var now = DateTime.UtcNow;
                if(nextCheckTime >= now) return;
                if(!from.Exists)
                {
                    Debug.LogWarning($"File does not exist: {from}");
                    return;
                }
                nextCheckTime = now + delay;
                if(to.Exists && from.LastWriteTimeRecursive() < to.LastWriteTimeRecursive() + delay / 2) return;
                onUpdate(from, to);
            });
        }
        
        public delegate void FileMapCallback(FileInfo from, FileInfo to);
        static HashSet<FileMapCallback> registeredFiles = new();
        public static void AddFileMapListener(float checkDelayInMs, FileInfo from, FileInfo to, FileMapCallback onUpdate)
        {
            if(registeredFiles.Contains(onUpdate))
            {
                Debug.LogError($"File listener already registered: {from}");
                return;
            }
            
            registeredFiles.Add(onUpdate);
            var delay = TimeSpan.FromMilliseconds(checkDelayInMs);
            var nextCheckTime = DateTime.MinValue;
            onUpdateInternal.Add(() => {
                var now = DateTime.UtcNow;
                if(nextCheckTime >= now) return;
                if(!from.Exists)
                {
                    Debug.LogWarning($"File does not exist: {from}");
                    return;
                }
                nextCheckTime = now + delay;
                if(to.Exists && from.LastWriteTime < to.LastWriteTime + delay / 2) return;
                onUpdate(from, to);
            });
        }
        
        public delegate void DirectoryCallback(DirectoryInfo dir);
        static Dictionary<DirectoryCallback, DateTime> lastUpdateTimeDir = new();
        public static void AddFolderListener(float checkDelayInMs, DirectoryInfo from, DirectoryCallback onUpdate)
        {
            if(lastUpdateTimeDir.ContainsKey(onUpdate))
            {
                Debug.LogError($"Folder listener already registered: {from}");
                return;
            }
            
            lastUpdateTimeDir.Add(onUpdate, DateTime.MinValue);
            
            var lastCheckTime = DateTime.Now;
            onUpdateInternal.Add(() => {
                if ((DateTime.Now - lastCheckTime).TotalMilliseconds < checkDelayInMs) return;
                lastCheckTime = DateTime.Now;
                
                var lastWriteTime = from.LastWriteTimeRecursive();
                if(lastUpdateTimeDir[onUpdate] >= lastWriteTime) return;
                if(!from.Exists)
                {
                    Debug.LogWarning($"Directory does not exist: {from}");
                    return;
                }
                lastUpdateTimeDir[onUpdate] = lastWriteTime;
                onUpdate(from);
            });
        }
        
        public delegate void FileCallback(FileInfo file);
        static Dictionary<FileCallback, DateTime> lastUpdateTimeFile = new();
        public static void AddFileListener(float checkDelayInMs, FileInfo from, FileCallback onUpdate)
        {
            if(lastUpdateTimeFile.ContainsKey(onUpdate))
            {
                Debug.LogError($"File listener already registered: {from}");
                return;
            }
            
            lastUpdateTimeFile.Add(onUpdate, DateTime.MinValue);
            
            var lastCheckTime = DateTime.Now;
            onUpdateInternal.Add(() => {
                if ((DateTime.Now - lastCheckTime).TotalMilliseconds < checkDelayInMs) return;
                lastCheckTime = DateTime.Now;
                
                var lastWriteTime = from.LastWriteTime;
                if(lastUpdateTimeFile[onUpdate] >= lastWriteTime) return;
                if(!from.Exists)
                {
                    Debug.LogWarning($"File does not exist: {from}");
                    return;
                }
                lastUpdateTimeFile[onUpdate] = lastWriteTime;
                onUpdate(from);
            });
        }
        
        #endregion
        
        // =================================================================================================
        // =================================================================================================
        #region GUI Utilities
        
        public static Color toggleButtonColor = new Color(0.3f, 0.6f, 0.3f, 1f);
        
        // 通用的展开-折叠按钮函数
        public static bool DrawToggleButton(bool enabled, string enabledText, string disabledText, params GUILayoutOption[] options)
        {
            bool clicked = false;
            
            // 使用绿色背景色当展开时
            if(enabled)
            {
                using(new BackgroundColorScope(toggleButtonColor))
                {
                    clicked = GUILayout.Button(enabledText, options);
                }
            }
            else
            {
                clicked = GUILayout.Button(disabledText, options);
            }
            
            return clicked ? !enabled : enabled;
        }
		
		public static bool DrawToggleButton(Rect rect, bool enabled, string enabledText, string disabledText)
        {
			bool clicked = false;
			if(enabled)
			{
				using(new BackgroundColorScope(toggleButtonColor))
				{
					clicked = GUI.Button(rect, enabledText);
				}
			}
			else
			{
				clicked = GUI.Button(rect, disabledText);
			}
			return clicked ? !enabled : enabled;
		}
		
        // 仅传入一个字符串的方法重载（展开时显示该字符串,折叠时显示"折叠"）
        public static bool DrawToggleButton(bool enabled, string text, params GUILayoutOption[] options)
        {
            return DrawToggleButton(enabled, text, text, options);
        }
        
        public static int DrawSelectionButtonGroup(int currentSelection, string[] buttonTexts, params GUILayoutOption[] layoutOptions)
        {
            int newSelection = currentSelection;
            
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                bool isSelected = (currentSelection == i);
                var newIsSelected = DrawToggleButton(isSelected, buttonTexts[i], layoutOptions);
                if((newIsSelected != isSelected) && i != currentSelection) newSelection = i;
            }
            
            return newSelection;
        }
        
        public static int ToggleButtonGroup(int currentSelection, string[] buttonTexts, params GUILayoutOption[] layoutOptions)
        {
            return DrawSelectionButtonGroup(currentSelection, buttonTexts, layoutOptions);
        }
        
        // 多选一选择器按钮组（带自定义按钮宽度）
        public static int DrawRadioButtonGroup(int currentSelection, string[] buttonTexts, int buttonWidth)
        {
            return DrawSelectionButtonGroup(currentSelection, buttonTexts, GUIPreset.width[buttonWidth]);
        }
        
        // 多选一选择器按钮组（使用给定的 Rect，按钮均分该区域）
        public static int DrawSelectionButtonGroup(Rect rect, int currentSelection, string[] buttonTexts, bool isHorizontal = true)
        {
            if(buttonTexts == null || buttonTexts.Length == 0) return currentSelection;
            
            int newSelection = currentSelection;
            int buttonCount = buttonTexts.Length;
            
            if(isHorizontal)
            {
                float buttonWidth = rect.width / buttonCount;
                for(int i = 0; i < buttonCount; i++)
                {
                    Rect buttonRect = new Rect(rect.x + i * buttonWidth, rect.y, buttonWidth, rect.height);
                    bool isSelected = (currentSelection == i);
                    var newIsSelected = DrawToggleButton(buttonRect, isSelected, buttonTexts[i], buttonTexts[i]);
                    if((newIsSelected != isSelected) && i != currentSelection) newSelection = i;
                }
            }
            else
            {
                float buttonHeight = rect.height / buttonCount;
                for(int i = 0; i < buttonCount; i++)
                {
                    Rect buttonRect = new Rect(rect.x, rect.y + i * buttonHeight, rect.width, buttonHeight);
                    bool isSelected = (currentSelection == i);
                    var newIsSelected = DrawToggleButton(buttonRect, isSelected, buttonTexts[i], buttonTexts[i]);
                    if((newIsSelected != isSelected) && i != currentSelection) newSelection = i;
                }
            }
            
            return newSelection;
        }
        
        #endregion
    }
}
