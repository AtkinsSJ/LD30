using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lorry : MonoBehaviour {

	public enum State {
		Idle,
		Goto,
		Trade,
		Wait
	}
	private State state = State.Idle;

	private LorriesList lorriesList;

	public Route route = new Route();
	private int routeStopIndex;
	internal Route.Stop routeStop;

	public float movementSpeed = 1f;
	public float planetWaitTime = 1f;

	public string lorryName = "Unnamed Lorry";
	public int capacity = 20;
	public bool carrying = false;
	public GoodType carriedGood;

	private bool running = false;
	public bool isRunning {
		get { return running; }
		set { running = value; }
	}

	private PopupManager popups;

	// Use this for initialization
	void Start() {
		lorriesList = GetComponentInParent<LorriesList>();
		popups = transform.Find("/PopupManager").GetComponent<PopupManager>();
	}

	public State getState() {
		return state;
	}

	// Update is called once per frame
	void Update() {

		if (!running) {

			return;
		}

		if (route == null || route.Length == 0) {
			state = State.Idle;
		}

		switch (state) {
			case State.Idle: IdleState(); break;
			case State.Goto: GotoState(); break;
			case State.Trade: TradeState(); break;
		}
	}

	void IdleState() {
		// If we have a valid route, go!
		if (route != null && route.Length > 0) {
			// Start the route!
			routeStopIndex = 0;
			routeStop = route[0];

			state = State.Goto;
		}
	}

	void GotoState() {

		// Are we there yet?
		if ((transform.position - routeStop.planet.transform.position).sqrMagnitude < 0.1f) {
			Debug.Log("You have reached your destination");
			state = State.Trade;
			return;
		}

		// Head for the target planet!
		Vector3 journey = routeStop.planet.transform.position - transform.position;
		transform.position += journey.normalized * Time.deltaTime * movementSpeed;
		transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(journey.y, journey.x));
	}

	void TradeState() {
		// Actually trade things!
		Debug.Log("Trading");

		try {
			switch (routeStop.stopType) {
				case Route.StopType.Buy:
					Debug.Log("Buying goods");
					if (!carrying) {
						int expense = routeStop.planet.BuyGoodsFrom(routeStop.goodType, capacity);
						lorriesList.ModifyFunds(expense);
						popups.AddExpensePopup(expense, transform.position);
						carrying = true;
						carriedGood = routeStop.goodType;
					} else {
						Debug.Log( string.Format("Can't buy {0}, because we're full!", routeStop.goodType));
					}
					break;

				case Route.StopType.Sell:
					Debug.Log("Selling goods");
					if (carrying && carriedGood == routeStop.goodType) {
						int income = routeStop.planet.SellGoodsTo(routeStop.goodType, capacity);
						lorriesList.ModifyFunds(income);
						popups.AddIncomePopup(income, transform.position);
						carrying = false;
					} else {
						Debug.Log( string.Format("Can't sell {0}, because we're not carrying any!", routeStop.goodType));
					}
					break;
			}
		} catch (System.ArgumentException ex) {
			Debug.LogError(ex);
		}

		// TODO: Maybe show a notification when things are traded?

		// Wait a little while before moving again
		StartCoroutine(WaitThenLeavePlanet());
	}

	IEnumerator WaitThenLeavePlanet() {
		state = State.Wait;
		Debug.Log("Waiting for a while");
		yield return new WaitForSeconds(planetWaitTime);
		routeStopIndex = route.NextStop(routeStopIndex);
		routeStop = route[routeStopIndex];
		state = State.Goto;
		Debug.Log("Going to next stop!");
	}

	internal void JettisonCargo() {
		carrying = false;
	}
}
