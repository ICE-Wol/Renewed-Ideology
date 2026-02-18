
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;

namespace Prota.Unity
{
//     //
//     // Summary:
//     //     This property is used to identify which native system this belongs to, or to
//     //     get the name of the managed system to show in the profiler.
//     public Type type;
// 
//     //
//     // Summary:
//     //     A list of sub systems which run as part of this item in the player loop.
//     public PlayerLoopSystem[] subSystemList;
// 
//     //
//     // Summary:
//     //     A managed delegate. You can set this to create a new C# entrypoint in the player
//     //     loop.
//     public UpdateFunction updateDelegate;
// 
//     //
//     // Summary:
//     //     A native engine system. To get a valid value for this, you must copy it from
//     //     one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.
//     public IntPtr updateFunction;
// 
//     //
//     // Summary:
//     //     The loop condition for a native engine system. To get a valid value for this,
//     //     you must copy it from one of the PlayerLoopSystems returned by PlayerLoop.GetDefaultPlayerLoop.
//     public IntPtr loopConditionFunction;

    public class PlayerLoopSystemNode
    {
        public Type type;
        public List<PlayerLoopSystemNode> subSystemList = new();
        public PlayerLoopSystem.UpdateFunction updateDelegate;
        public IntPtr updateFunction;
        public IntPtr loopConditionFunction;
        
        public PlayerLoopSystemNode parent;
        public PlayerLoopSystemNode(PlayerLoopSystem root, PlayerLoopSystemNode parent = null)
        {
            this.type = root.type;
            this.updateDelegate = root.updateDelegate;
            this.updateFunction = root.updateFunction;
            this.loopConditionFunction = root.loopConditionFunction;
            if(root.subSystemList != null)
            {
                foreach(var sub in root.subSystemList)
                {
                    subSystemList.Add(new PlayerLoopSystemNode(sub, this));
                }
            }
        }
        
        public PlayerLoopSystemNode this[Type type]
        {
            get
            {
                foreach(var sub in subSystemList)
                {
                    if(sub.type == type) return sub;
                }
                
                return null;
            }
        }
        
        public void PrintAll(StringBuilder sb = null, string indent = "")
        {
            var isRoot = sb == null;
            if(isRoot) sb = new StringBuilder();
            sb.AppendLine($"{indent}{this.type}");
            indent = indent + "  ";
            foreach(var sub in this.subSystemList) sub.PrintAll(sb, indent);
            // if(isRoot) Debug.Log(sb.ToString());
        }
        
        public void Add(Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            this.subSystemList.Add(new PlayerLoopSystemNode(new PlayerLoopSystem() {
                type = type,
                updateDelegate = updateDelegate
            }, this));
        }
        
        public void Insert(int index, Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            this.subSystemList.Insert(index, new PlayerLoopSystemNode(new PlayerLoopSystem() {
                type = type,
                updateDelegate = updateDelegate
            }, this));
        }
        
        public void AddBefore(Type beforeType, Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            var i = this.subSystemList.FindIndex(sub => sub.type == beforeType);  
            if(i == -1) throw new Exception($"PlayerLoopSystemNode.AddBefore: {beforeType} not found");
            this.Insert(i, type, updateDelegate);
        }
        
        public void AddBefore(string type, Type newType, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            var i = this.subSystemList.FindIndex(sub => sub.type.Name == type);  
            if(i == -1) throw new Exception($"PlayerLoopSystemNode.AddBefore: {type} not found");
            this.Insert(i, newType, updateDelegate);
        }
        
        public void AddAfter(Type afterType, Type type, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            var i = this.subSystemList.FindIndex(sub => sub.type == afterType);  
            if(i == -1) throw new Exception($"PlayerLoopSystemNode.AddAfter: {afterType} not found");
            this.Insert(i + 1, type, updateDelegate);
        }
        
        public void AddAfter(string type, Type newType, PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            var i = this.subSystemList.FindIndex(sub => sub.type.Name == type);  
            if(i == -1) throw new Exception($"PlayerLoopSystemNode.AddAfter: {type} not found");
            this.Insert(i + 1, newType, updateDelegate);
        }
        
        public int FindIndex(Type type)
        {
            return this.subSystemList.FindIndex(sub => sub.type == type);
        }
        
        public int FindIndex(string name)
        {
            return this.subSystemList.FindIndex(sub => sub.type.Name == name);
        }
        
        
        public PlayerLoopSystem ToSystem()
        {
            var result = new PlayerLoopSystem();
            result.type = type;
            result.updateDelegate = this.updateDelegate;
            result.updateFunction = this.updateFunction;
            result.loopConditionFunction = this.loopConditionFunction;
            result.subSystemList = new PlayerLoopSystem[this.subSystemList.Count];
            for(int i = 0; i < this.subSystemList.Count; i++)
                result.subSystemList[i] = this.subSystemList[i].ToSystem();
            return result;
        }
    }
    
    public static partial class PlayerLoopExt
    {
        public static PlayerLoopSystemNode ToNode(this PlayerLoopSystem system)
        {
            return new PlayerLoopSystemNode(system);
        }
    }
}
