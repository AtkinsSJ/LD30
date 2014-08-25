using UnityEngine;

public enum WindowIDs {
	Planet,
	LorriesList,
	Lorry,
	AddOrder,
	GameOver,
	GameStart
}

public class WindowUtils {
	public static Rect KeepOnScreen(Rect windowRect) {
		windowRect.x = Mathf.Clamp(windowRect.x, 0, Screen.width - windowRect.width);
		windowRect.y = Mathf.Clamp(windowRect.y, 0, Screen.height - windowRect.height);
		return windowRect;
	}
}