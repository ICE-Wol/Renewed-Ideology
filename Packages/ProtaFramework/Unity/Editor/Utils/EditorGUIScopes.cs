using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    public struct HandleColorScope : IDisposable
    {
        Color color;
        
        public HandleColorScope(Color colorReplace)
        {
            color = Handles.color;
            Handles.color = colorReplace;
        }
        
        public void Dispose()
        {
            Handles.color = color;
        }
    }
    
    public struct ColorScope : IDisposable
    {
        Color color;
        bool shouldReplace;
        
        public ColorScope(Color colorReplace, bool shouldReplace = true)
        {
            this.shouldReplace = shouldReplace;
            color = Color.white;
            if(!shouldReplace) return;
            color = GUI.color;
            GUI.color = colorReplace;
        }
        
        public void Dispose()
        {
            if(!shouldReplace) return;
            GUI.color = color;
        }
    }
    
    public struct BackgroundColorScope : IDisposable
    {
        Color color;
        
        public BackgroundColorScope(Color colorReplace)
        {
            color = GUI.backgroundColor;
            GUI.backgroundColor = colorReplace;
        }
        
        public void Dispose()
        {
            GUI.backgroundColor = color;
        }
    }
    
    public struct ContentColorScope : IDisposable
    {
        Color color;
        
        public ContentColorScope(Color colorReplace)
        {
            color = GUI.contentColor;
            GUI.contentColor = colorReplace;
        }
        
        public void Dispose()
        {
            GUI.contentColor = color;
        }
    }



}
