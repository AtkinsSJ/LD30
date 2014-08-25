using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Galaxy : MonoBehaviour {

	public List<string> planetNames = new List<string>();

	public GameObject planetPrefab;

	public int minPlanets = 5,
				maxPlanets = 10;

	public float planetPadding = 20;

	public float width, height;

	private List<Planet> planets = new List<Planet>();
	public Planet this[int index] {
		get {
			if (index < 0 || index >= planets.Count) {
				throw new System.IndexOutOfRangeException();
			}
			return planets[index];
		}
	}
	public int Count {
		get { return planets.Count; }
	}

	private Camera mainCamera;
	private LorriesList lorriesList;

	private Planet activePlanet;
	private int activePlanetWindow = -1;
	private Rect planetWindowRect = new Rect(0, 0, 250, 100);
	private Rect planetWindowCloseButtonRect = new Rect(230, 0, 20, 20);
	private bool justClosedPlanetWindow = false;

	// Use this for initialization
	void Start () {

		// Find things!
		mainCamera = transform.Find("/Camera").camera;
		lorriesList = transform.Find("/Lorries").GetComponent<LorriesList>();

		// Build the galaxy!
		int planetsToCreate = Random.Range(minPlanets, maxPlanets + 1);

		float minX = -width / 2f,
			maxX = width / 2f,
			minY = -height / 2f,
			maxY = height / 2f;

		for (int i = 0; i < planetsToCreate; i++) {
			GameObject planetGO = (GameObject) Instantiate(planetPrefab);
			Planet planet = planetGO.GetComponent<Planet>();
			planetGO.transform.parent = transform;

			bool foundPosition = false;

			while (!foundPosition) {
				foundPosition = true;
				planetGO.transform.localPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);

				foreach (Planet p in planets) {
					// Are we far enough away from it?
					if ( (p.transform.localPosition - planetGO.transform.localPosition).magnitude < (p.scale + planet.scale + planetPadding) ) {
						foundPosition = false;
						break;
					}
				}
			}

			// Add the new planet to planets
			planets.Add(planet);
		}
	}

	public void ShowWindowForPlanet(Planet planet) {
		if (planet == activePlanet || justClosedPlanetWindow) {
			return;
		}

		if (activePlanet != null) {
			activePlanet.highlighted = false;
		}

		activePlanet = planet;
		activePlanet.highlighted = true;
		activePlanetWindow = planets.IndexOf(planet);

		Vector3 screenPos = mainCamera.WorldToScreenPoint(activePlanet.transform.position);
		planetWindowRect.position = new Vector2(screenPos.x, mainCamera.pixelHeight - screenPos.y);
	}

	public void CloseWindow() {
		activePlanetWindow = -1;
		activePlanet.highlighted = false;
		activePlanet = null;
		justClosedPlanetWindow = true;
	}
	
	// Update is called once per frame
	void Update () {
		justClosedPlanetWindow = false;
	}

	void OnGUI() {

		planetWindowRect = WindowUtils.KeepOnScreen(planetWindowRect);
		if (activePlanetWindow != -1) {
			planetWindowRect = GUILayout.Window((int)WindowIDs.Planet, planetWindowRect, this.PlanetWindow, activePlanet.planetName);
			GUI.BringWindowToFront(1);
		}
	}

	void PlanetWindow(int windowID) {

		if (GUI.Button(planetWindowCloseButtonRect, "X")) {
			CloseWindow();
			return;
		}

		if (activePlanet.isOwnedByPlayer) {
			GUILayout.Label(string.Format("Earns you ${0} in tax", activePlanet.taxValue));
		} else {
			if (GUILayout.Button(string.Format("Buy planet for ${0}", activePlanet.costToBuy))) {
				BuyPlanet(activePlanet);
			}
		}

		GUILayout.Label("Goods for sale:");
		for (int i = 0; i < (int)GoodType.SIZE; i++) {
			if (activePlanet.goodValues[i] > 0) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(Good.GOODS[i].pluralName);
				GUILayout.FlexibleSpace();
				GUILayout.Label("$" + activePlanet.goodValues[i].ToString());
				GUILayout.EndHorizontal();
			}
		}

		GUILayout.Label("");

		GUILayout.Label("Goods wanted:");
		for (int i = 0; i < (int)GoodType.SIZE; i++) {
			if (activePlanet.goodValues[i] < 0) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(Good.GOODS[i].pluralName);
				GUILayout.FlexibleSpace();
				GUILayout.Label("$" + (activePlanet.goodValues[i] * -1).ToString());
				GUILayout.EndHorizontal();
			}
		}

		GUI.DragWindow();
	}

	private void BuyPlanet(Planet planet) {
		if (planet.isOwnedByPlayer) return;

		lorriesList.ModifyFunds(-planet.costToBuy);
		planet.isOwnedByPlayer = true;

		// Does the player own all the planets?
		bool ownAll = true;
		foreach (Planet p in planets) {
			if (!p.isOwnedByPlayer) {
				ownAll = false;
				break;
			}
		}

		if (ownAll) {
			// YOU WIN!
			CloseWindow();
			lorriesList.OnGameWon();
		}
	}
}
