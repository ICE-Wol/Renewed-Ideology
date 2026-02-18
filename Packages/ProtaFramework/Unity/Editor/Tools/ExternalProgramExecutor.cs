using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using UnityEngine.UIElements;
using NUnit.Framework;
using UnityEditor.Graphs;
using System.Text;

namespace Prota.Editor
{
    public class ExternalProgramExecutor : EditorWindow
    {
        public static class ConfigPaths
        {
            const string sep = "[:~:]";
            
            static EditorPrefEntryString configPaths = new("Prota.ExternalProgramExecutor.ConfigPaths", "");
            
            static List<string> cache;
            
            public static IReadOnlyList<string> paths
            {
                get
                {
                    if(cache != null) return cache;
                    cache = configPaths.value.Split(sep, StringSplitOptions.None).ToList();
                    if(!cache.Any(x => x == "")) cache.Add("");
                    cache.Sort();
                    return cache;
                }
            }
            
            public static void ChangePath(int i, string newPath)
            {
                var _ = paths;
                cache[i] = newPath;
                SavePaths(cache);
            }
            
            public static void AddPath()
            {
                var _ = paths;
                cache.Add("new path");
                SavePaths(cache);
            }
            
            public static void RemovePath(int i)
            {
                var _ = paths;
                cache.RemoveAt(i);
                SavePaths(cache);
            }
            
            public static void SavePaths(List<string> strings)
            {
                configPaths.value = string.Join(sep, strings);
                cache = null;
            }
        }
        
        [Serializable]
        public struct AliasEntry
        {
            public string name;
            public string str;
            public Func<string> getter;
            public string replacement => getter == null ? str : getter();
        }
        
        [Serializable]
        public struct ExecutionEntry
        {
            public string name;
            public string command;
        }
        
        [Serializable]
        public class Config
        {
            public bool dirty = false;
            public string createdFromPath = "";       // "" 表示程序生成,不由json加载. null 不合法.
            public List<AliasEntry> aliasEntries = new();
            public List<ExecutionEntry> commands = new();
            public static AliasEntry[] builtinAliasEntries = Array.Empty<AliasEntry>();
            public string ToJson() => JsonUtility.ToJson(this, true);
            
            public string ReplaceWithAlias(string str)
            {
                foreach(var alias in aliasEntries)
                    str = str.Replace(alias.name, alias.replacement);
                foreach(var alias in builtinAliasEntries)
                    str = str.Replace(alias.name, alias.replacement);
                return str;
            }
            
            public void AddAlias(string name)
            {
                if(aliasEntries.Any(a => a.name == name)) return;
                aliasEntries.Add(new AliasEntry() { name = name, str = "" });
                dirty = true;
            }
            
            public bool RemoveAlias(string name)
            {
                dirty = aliasEntries.RemoveAll(a => a.name == name) > 0;
                return true;
            }
            
            public void AddCommand(string name)
            {
                if(commands.Any(a => a.name == name)) return;
                commands.Add(new ExecutionEntry() { name = name, command = "" });
                dirty = true;
            }
            
            public bool RemoveCommand(string name)
            {
                dirty = commands.RemoveAll(a => a.name == name) > 0;
                return true;
            }
            
            public static Config FromJsonFile(string path)
            {
                var json = File.ReadAllText(path);
                var config = FromJson(json);
                config.createdFromPath = path;
                return config;
            }
            
            public static Config FromJson(string json)
            {
                var config = JsonUtility.FromJson<Config>(json);
                config.createdFromPath = null;
                return config;
            }
            
            static Config()
            {
                var dataPath = Application.dataPath;
                var projectPath = dataPath.AsDirectoryInfo().Parent.FullName.ToStandardPath();
                builtinAliasEntries = new AliasEntry[] {
                    new AliasEntry {
                        name = "$Select",
                        getter = () => AssetDatabase.GetAssetPath(Selection.activeInstanceID)
                    },
                    new AliasEntry {
                        name = "$AssetPath",
                        getter = () => dataPath
                    },
                    new AliasEntry {
                        name = "$ProjectPath",
                        getter = () => projectPath
                    },
                };
            }
        }
        
        public static EditorPrefEntryString configContentNoPath = new("Prota.ExternalProgramExecutor.ConfigContentNoPath", null);
        public Config config;
        
        
        [MenuItem("ProtaFramework/Window/External Program Executor")]
        public static void ShowWindow()
        {
            GetWindow<ExternalProgramExecutor>("External Program Executor");
        }
        
        static Vector2 scrollPos;
        void OnGUI()
        {
            DrawConfigsUI();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawCommandsUI();
            DrawAliasUI();
            EditorGUILayout.EndScrollView();
            TrySaveConfig();
        }
        
        
        
        static EditorPrefEntryString selectedConfigPath = new("Prota.ExternalProgramExecutor.SelectedConfigPath", "");
        static EditorPrefEntryString allConfigPaths = new("Prota.ExternalProgramExecutor.AllConfigPaths", "");
        private void DrawConfigsUI()
        {
            EditorGUILayout.LabelField(">>> Config <<<", titleStyle);
            if(config == null || selectedConfigPath.value != config.createdFromPath)
            {
                config = null;
                selectedConfigPath.value = "";
            }
            
            var p = ConfigPaths.paths;
            for(int i = 0; i < p.Count; i++)
            {
                var configPath = p[i];
                using(var _ = new EditorGUILayout.HorizontalScope())
                {
                    var cc = selectedConfigPath.value != configPath ? "-"
                        : config == null ? "x" 
                        : config.dirty ? "?" 
                        : "√";
                    EditorGUILayout.LabelField(cc, GUIPreset.width[16]);
                    
                    var newPath = EditorGUILayout.TextField(p[i]);
                    if(p[i] != newPath) ConfigPaths.ChangePath(i, newPath);
                    
                    if(!(configPath.NullOrEmpty() || configPath.AsFileInfo().IsFile())) GUI.enabled = false;
                    if(GUILayout.Button("Load", GUIPreset.width[60])) LoadConfig(configPath);
                    GUI.enabled = true;
                    
                    if(config == null) GUI.enabled = false;
                    if(GUILayout.Button("Save", GUIPreset.width[60])) TrySaveConfig(configPath, true);
                    GUI.enabled = true;
                    
                    if(configPath.NullOrEmpty()) GUI.enabled = false;
                    if(GUILayout.Button("Remove", GUIPreset.width[60])) RemoveConfig(i);
                    GUI.enabled = true;
                }
            }
            
            if(GUILayout.Button("Add")) ConfigPaths.AddPath();
            
            void RemoveConfig(int i)
            {
                var configPath = ConfigPaths.paths[i];
                ConfigPaths.RemovePath(i);
                if(selectedConfigPath.value == configPath)
                {
                    config = null;
                    selectedConfigPath.value = "";
                }
            }
            
            bool LoadConfig(string path)
            {
                selectedConfigPath.value = path;
                
                if(!path.NullOrEmpty())     // 从配置文件创建
                {
                    if(!path.AsFileInfo().Exists)
                    {
                        Debug.LogError($"[Prota.ExternalProgramExecutor] Config file not found [{path}]");
                        return false;
                    }
                    
                    try
                    {
                        config = Config.FromJsonFile(path);
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"[Prota.ExternalProgramExecutor] Failed to load config from [{path}]: {e}");
                        return false;
                    }
                    return true;
                }
                
                // 从配置内容创建.
                var content = configContentNoPath.value;
                try
                {
                    config = Config.FromJson(content);
                    config.createdFromPath = "";
                    return true;
                }
                catch(Exception e)
                {
                    Debug.LogWarning($"[Prota.ExternalProgramExecutor] Failed to load config from content, created a new one.\n[{e}]");
                    Debug.Log("[Prota.ExternalProgramExecutor] Create new config.");
                    config = new Config() { createdFromPath = "" };
                    return true;
                }
            }
            
        }
        
        static EditorPrefEntryBool showAlias = new("Prota.ExternalProgramExecutor.ShowAlias", false);
        static EditorPrefEntryString selectAlias = new("Prota.ExternalProgramExecutor.SelectAlias", "");
        private void DrawAliasUI()
        {
            if(config == null) return;
            EditorGUILayout.Space(4);
            ProtaEditorUtils.SeperateLine(3);
            using(var _ = new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(">>> Alias <<<", titleStyle);
                showAlias.value = EditorGUILayout.Toggle(showAlias.value, GUIPreset.width[20]);
            }
            
            if(!showAlias.value) return;
        
            using(new EditorGUILayout.HorizontalScope())
            {
                selectAlias.value = EditorGUILayout.TextField(selectAlias.value);
                
                if(config.aliasEntries.Any(x => x.name == selectAlias.value)) GUI.enabled = false;
                if(GUILayout.Button("+", GUIPreset.width[20]))
                {
                    config?.AddAlias(selectAlias.value);
                    Repaint();
                }
                GUI.enabled = true;
                
                if(!config.aliasEntries.Any(x => x.name == selectAlias.value)) GUI.enabled = false;
                if(GUILayout.Button("-", GUIPreset.width[20]))
                {
                    config?.RemoveAlias(selectAlias.value);
                    Repaint();
                }
                GUI.enabled = true;
            }
            
            EditorGUI.BeginChangeCheck();
            for(int i = 0; i < config.aliasEntries.Count; i++)
            {
                var alias = config.aliasEntries[i];
                using var _ = new EditorGUILayout.HorizontalScope();
                alias.name = EditorGUILayout.TextField(alias.name);
                alias.str = EditorGUILayout.TextField(alias.str);
                config.aliasEntries[i] = alias;
            }
            if(EditorGUI.EndChangeCheck()) config.dirty = true;
            
            foreach(var alias in Config.builtinAliasEntries)
            {
                using var _ = new EditorGUILayout.HorizontalScope();
                using var __ = new EditorGUI.DisabledGroupScope(true);
                EditorGUILayout.LabelField(alias.name);
                EditorGUILayout.TextField(alias.replacement);
            }
        }
        
        
        private void DrawCommandsUI()
        {
            if(config == null) return;
            
            EditorGUILayout.Space(4);
            ProtaEditorUtils.SeperateLine(3);
            EditorGUILayout.LabelField($">>> Commands[{selectedConfigPath.value}] <<<", titleStyle);
            
            for(int i = 0; i < config.commands.Count; i++)
            {
                var command = config.commands[i];
                ProtaEditorUtils.SeperateLine(1, new Color(.16f, .16f, .16f, .8f));
                using(var _ = new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(GetIName(i), EditorStyles.boldLabel, GUIPreset.width[20]);
                    
                    EditorGUI.BeginChangeCheck();
                    command.name = EditorGUILayout.TextField(command.name);
                    if(EditorGUI.EndChangeCheck()) config.dirty = true;
                    
                    if(i == 0) GUI.enabled = false;
                    if(GUILayout.Button("↑", GUIPreset.width[20]))
                    {
                        config.commands.Swap(i, i - 1);
                        config.dirty = true;
                        Repaint();
                    }
                    GUI.enabled = true;
                    
                    if(i == config.commands.Count - 1) GUI.enabled = false;
                    if(GUILayout.Button("↓", GUIPreset.width[20]))
                    {
                        config.commands.Swap(i, i + 1);
                        config.dirty = true;
                        Repaint();
                    }
                    GUI.enabled = true;
                    
                    if(command.command.NullOrEmpty() && GUILayout.Button("Remove", GUIPreset.width[140]))
                    {
                        config.RemoveCommand(command.name);
                        Repaint();
                        return;
                    }
                    if(!command.command.NullOrEmpty() && GUILayout.Button("Execute", GUIPreset.width[200]))
                        ExecuteCommand(command.command);
                }
                command.command = EditorGUILayout.TextArea(command.command);
                config.commands[i] = command;
            }
            
            if(GUILayout.Button("Add"))
            {
                config?.AddCommand("new command");
                Repaint();
            }
        }

        private void ExecuteCommand(string command)
        {
            command = config.ReplaceWithAlias(command);
            
            var cwd = "./".AsDirectoryInfo().FullName;
            Debug.Log($"[Prota.ExternalProgramExecutor] Execute command [{command}] at [{cwd}]");
            var parts = command.Split(' ');
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.WorkingDirectory = cwd;
            proc.StartInfo.FileName = parts[0];
            proc.StartInfo.Arguments = string.Join(" ", parts.Skip(1));
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;
            // proc.StartInfo.StandardInputEncoding = Encoding.GetEncoding("gb2312");
            // proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("gb2312");
            proc.OutputDataReceived += (sender, e) =>
            {
                if(!e.Data.NullOrEmpty())
                    Debug.Log($"[Prota.ExternalProgramExecutor] Get Output [{command}] {e.Data}");
            };
            proc.ErrorDataReceived += (sender, e) =>
            {
                if(!e.Data.NullOrEmpty())
                    Debug.LogError($"[Prota.ExternalProgramExecutor] Get Error [{command}] {e.Data}");
            };
            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
        }

        void TrySaveConfig(string path = null, bool force = false)
        {
            if(path == null) path = selectedConfigPath.value;
            if(config == null) return;
            if(!config.dirty && !force) return;
            if(path.NullOrEmpty())
            {
                // Debug.Log("[Prota.ExternalProgramExecutor] Save config to EditorPref.");
                configContentNoPath.value = config.ToJson();
            }
            else
            {
                // Debug.Log($"[Prota.ExternalProgramExecutor] Save config to [{path}]");
                File.WriteAllText(path, config.ToJson());
            }
            config.dirty = false;
        }
        
        
        static GUIStyle _titleStyle;
        static GUIStyle titleStyle
        {
            get
            {
                if(_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(EditorStyles.label);
                    _titleStyle.fontSize = 16;
                    _titleStyle.fontStyle = FontStyle.Bold;
                    _titleStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _titleStyle;
            }
        }
        
        public static Dictionary<int, string> namesCache = new();
        static string GetIName(int i) => namesCache.TryGetValue(i, out var name) ? name : (namesCache[i] = $"[{i}]");
    }
}
