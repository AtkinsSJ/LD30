﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LorriesList : MonoBehaviour {

	public int funds = 1000;

	public GameObject lorryPrefab;

	private Galaxy galaxy;

	private List<Lorry> lorries = new List<Lorry>();

	private Rect windowRect;
	private int selectedLorry = -1;
	private Rect lorryWindowRect;
	private Rect closeButtonRect;

	private bool showAddOrderWindow = false;
	private Rect addOrderWindowRect;
	private int addOrderPlanetSelected = -1;
	private int addOrderActionSelected = -1;

	private Rect modalWindowRect;

	private string companyName = "Lorries In SPAAAAAACE!";

	private bool started = false;
	private bool playing = false;

	// Use this for initialization
	void Start () {
		windowRect = new Rect(10, 10, 250, 100);
		lorryWindowRect = new Rect(310, 30, 250, 100);
		closeButtonRect = new Rect(230, 0, 20, 20);
		addOrderWindowRect = new Rect(610, 50, 250, 100);

		galaxy = transform.Find("/Galaxy").GetComponent<Galaxy>();

		modalWindowRect = new Rect(0, 0, 500, 300);
	}

	void OnGUI() {

		if (!started) {

			modalWindowRect.position = new Vector2((Screen.width - modalWindowRect.width) / 2f, (Screen.height - modalWindowRect.height) / 2f);
			GUI.ModalWindow((int)WindowIDs.GameStart, modalWindowRect, this.GameStartWindow, "Create Your Company");

		} else {
			if (playing) {
				windowRect = GUILayout.Window((int)WindowIDs.LorriesList, windowRect, this.LorriesWindow, "Lorries");

				if (selectedLorry != -1) {
					lorryWindowRect = GUILayout.Window((int)WindowIDs.Lorry, lorryWindowRect, this.LorryInspectionWindow, lorries[selectedLorry].lorryName);

					if (showAddOrderWindow) {
						addOrderWindowRect = GUILayout.Window((int)WindowIDs.AddOrder, addOrderWindowRect, this.AddOrderWindow, "Add order");
					}
				}
			} else {
				modalWindowRect.position = new Vector2((Screen.width - modalWindowRect.width) / 2f, (Screen.height - modalWindowRect.height) / 2f);
				GUI.ModalWindow((int)WindowIDs.GameOver, modalWindowRect, this.GameOverWindow, "Game Over!");
			}

			// Company Info
			GUI.Box(new Rect(0, 0, Screen.width, 20), companyName + " | Funds: $" + funds);
		}
	}

	void GameStartWindow(int windowID) {
		GUILayout.Label("Welcome to LORRIES IN SPAAAAAACE!");
		GUILayout.Label("Buy space-lorries, give them orders to buy/sell goods at different planets, and they'll go off and make you money!");
		GUILayout.Label("If you go broke, you lose! Try and see how rich you can get. (An actual goal is coming later, hopefully.)");
		GUILayout.FlexibleSpace();
		GUILayout.Label("CONTROLS");
		GUILayout.Label("Arrows/WASD to move the camera, zoom with the mouse wheel. Left-click on planets to inspect them.");
		GUILayout.FlexibleSpace();
		GUILayout.Label("CHOOSE YOUR COMPANY NAME!");
		companyName = GUILayout.TextField(companyName);

		if (GUILayout.Button("Begin!")) {
			started = true;
			playing = true;
		}
	}

	void LorriesWindow(int windowId) {

		string[] lorryNames = new string[lorries.Count];
		for (int i = 0; i < lorries.Count; i++) {
			lorryNames[i] = lorries[i].lorryName;
		}

		selectedLorry = GUILayout.SelectionGrid(selectedLorry, lorryNames, 1);

		GUILayout.Label("");

		if (GUILayout.Button("Buy new Lorry ($200)")) {
			CreateNewLorry();
		}

		GUI.DragWindow();
	}

	void CreateNewLorry() {
		ModifyFunds(-200);
		GameObject newLorry = (GameObject)Instantiate(lorryPrefab);
		newLorry.transform.parent = transform;
		Lorry lorry = newLorry.GetComponent<Lorry>();
		lorries.Add(lorry);
		lorry.lorryName = "Lorry " + lorries.Count;

		// Open the selection window for the new lorry!
		selectedLorry = lorries.Count - 1;
	}

	void LorryInspectionWindow(int windowId) {

		if (GUI.Button(closeButtonRect, "X")) {
			selectedLorry = -1;
			return;
		}

		Lorry lorry = lorries[selectedLorry];
		Route route = lorry.route;
		Route.Stop routeStop = lorry.routeStop;
		Route.Stop[] stops = route.Stops;

		switch (lorry.getState()) {
			case Lorry.State.Idle:
				GUILayout.Label("Currently idle");
				break;

			case Lorry.State.Goto:
				GUILayout.Label(string.Format("Heading for {0}", routeStop.planet.planetName));
				break;

			case Lorry.State.Trade:
			case Lorry.State.Wait:
				if (lorry.routeStop.stopType == Route.StopType.Buy) {
					GUILayout.Label(string.Format("Buying {0} from {1}", Good.GOODS[(int)routeStop.goodType].pluralName, routeStop.planet.planetName));
				} else {
					GUILayout.Label(string.Format("Selling {0} to {1}", Good.GOODS[(int)routeStop.goodType].pluralName, routeStop.planet.planetName));
				}
				break;
		}

		// List the orders!

		GUILayout.Label("Orders:");

		GUILayout.BeginVertical();


		for (int i = 0; i < stops.Length; i++) {
			Route.Stop stop = stops[i];
			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("{0} {1} at {2}", stop.stopType, Good.GOODS[(int)stop.goodType].pluralName, stop.planet.planetName));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("X")) {
				route.RemoveStop(i);
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();

		if (GUILayout.Button("Add Order")) {
			showAddOrderWindow = true;

			addOrderPlanetSelected = -1;
			addOrderActionSelected = -1;
		}

		GUI.DragWindow();
	}

	void AddOrderWindow(int windowId) {
		if (GUI.Button(closeButtonRect, "X")) {
			showAddOrderWindow = false;
			return;
		}

		GUILayout.Label("Go to:");

		// Planet selector!
		int oldPlanetSelected = addOrderPlanetSelected;
		addOrderPlanetSelected = GUILayout.SelectionGrid(addOrderPlanetSelected, galaxy.planetNames.ToArray(), 1);

		if (oldPlanetSelected != addOrderPlanetSelected) {
			addOrderActionSelected = -1;
		}

		if (addOrderPlanetSelected != -1) {
			// Good selector (and buy/sell)
			GUILayout.Label("And then:");

			Planet planet = galaxy[addOrderPlanetSelected];

			// TODO: Cache the goods lists!
			int[] sellableGoods = planet.BuyingGoods;
			int[] buyableGoods = planet.SellingGoods;

			string[] actionLabels = new string[buyableGoods.Length + sellableGoods.Length];

			for (int i = 0; i < sellableGoods.Length; i++) {
				actionLabels[i] = string.Format("Sell {0} at ${1} each", Good.GOODS[sellableGoods[i]].pluralName, -planet.goodValues[sellableGoods[i]]);
			}

			for (int i = 0; i < buyableGoods.Length; i++) {
				actionLabels[i + sellableGoods.Length] = string.Format("Buy {0} at ${1} each", Good.GOODS[buyableGoods[i]].pluralName, planet.goodValues[buyableGoods[i]]);
			}

			addOrderActionSelected = GUILayout.SelectionGrid(addOrderActionSelected, actionLabels, 1);

			if (addOrderActionSelected != -1) {
				// DONE!
				if (addOrderActionSelected < sellableGoods.Length) {
					// Sell a thing!
					lorries[selectedLorry].route.AddStop(
						new Route.Stop(planet, (GoodType)sellableGoods[addOrderActionSelected], Route.StopType.Sell)
					);
				} else {
					// Buy a thing!
					lorries[selectedLorry].route.AddStop(
						new Route.Stop(planet, (GoodType)buyableGoods[addOrderActionSelected-sellableGoods.Length], Route.StopType.Buy)
					);
				}

				showAddOrderWindow = false;
			}
		}

		GUI.DragWindow();
	}

	public void ModifyFunds(int change) {
		funds += change;

		if (funds < 0) {
			OnBankrupt();
		}
	}

	void OnBankrupt() {
		playing = false;
		transform.Find("/Galaxy").GetComponent<Galaxy>().CloseWindow();
	}

	void GameOverWindow(int windowID) {
		GUILayout.Label("You went bankrupt! Sorry to hear about that.");

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Start New Game")) {
			Application.LoadLevel("playscene");
			return;
		}
	}
}