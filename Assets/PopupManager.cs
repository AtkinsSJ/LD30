using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour {

	public Camera mainCamera;

	public GUIStyle incomeStyle,
					expenseStyle;

	private class Popup {
		public string text;
		public Vector3 worldPosition;
		public GUIStyle style;
		public float life;

		public Popup(string text, Vector3 worldPosition, GUIStyle style) {
			this.text = text;
			this.worldPosition = worldPosition;
			this.style = style;
			this.life = 2.5f;
		}
	}

	private List<Popup> popups = new List<Popup>();

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {

		// Update the life of each popup, and remove any dead ones.

		List<Popup> deadPopups = new List<Popup>();
		for (int i = 0; i < popups.Count; i++) {
			Popup p = popups[i];
			p.life -= Time.deltaTime;
			if (p.life <= 0) {
				deadPopups.Add(p);
			}
		}

		foreach (Popup p in deadPopups) {
			popups.Remove(p);
		}
	}

	void OnGUI() {
		foreach (Popup popup in popups) {
			Vector3 screenPos = mainCamera.WorldToScreenPoint(popup.worldPosition);
			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y + (popup.life * 10f) - 30f, 100, 20), popup.text, popup.style);
		}
	}

	public void AddIncomePopup(int income, Vector3 worldPosition) {
		popups.Add(new Popup(
			string.Format("+${0}", income),
			worldPosition,
			incomeStyle
		));
	}

	public void AddExpensePopup(int expense, Vector3 worldPosition) {
		popups.Add(new Popup(
			string.Format("-${0}", -expense),
			worldPosition,
			expenseStyle
		));
	}
}
