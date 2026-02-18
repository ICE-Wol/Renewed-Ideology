using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
	public static class LocalizationCommand
	{
		[MenuItem("ProtaFramework/Localization/Refresh Database")]
		public static void RefreshDatabase()
		{
			LocalizationDatabase.Refresh();
		}
	}
}
