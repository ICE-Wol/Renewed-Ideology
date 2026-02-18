using System;
using System.Collections.Generic;
using Prota;
using Prota.Unity;
using UnityEngine;

public static class AppQuit
{
    public static bool isQuitting { get; private set; } = false;
    
    public static readonly List<Action> onAppQuit = new();
    
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Load()
    {
        isQuitting = false;
		Application.quitting -= TagQuit;
		Application.quitting += TagQuit;
    }
	
	static void TagQuit()
    {
		onAppQuit.InvokeAll();
		onAppQuit.Clear();
        isQuitting = true;
    }
}
