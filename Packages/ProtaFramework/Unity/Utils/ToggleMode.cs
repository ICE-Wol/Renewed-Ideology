public enum ToggleMode
{
	Active = 1,
	Deactive = 2,
	Toggle = 3,
}

public static partial class MethodExt
{
	public static bool ApplyToggleModeTo(this ToggleMode toggleMode, ref bool target)
	{
		return target = toggleMode.GetToggleModeResult(target);
	}
	
	public static bool GetToggleModeResult(this ToggleMode toggleMode, bool currentValue)
	{
		switch (toggleMode)
		{
			case ToggleMode.Active:
				return true;
			case ToggleMode.Deactive:
				return false;
			case ToggleMode.Toggle:
				return !currentValue;
			default:
				return currentValue;
		}
	}
}