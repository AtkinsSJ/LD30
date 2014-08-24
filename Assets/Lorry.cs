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
	private bool carrying = false;
	public GoodType carriedGood;

	// Use this for initialization
	void Start() {
		lorriesList = GetComponentInParent<LorriesList>();
	}

	public State getState() {
		return state;
	}

	// Update is called once per frame
	void Update() {

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
		// Do nothing
		// TODO: Maybe tell the player that we're idle?

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
						lorriesList.ModifyFunds(routeStop.planet.BuyGoodsFrom(routeStop.goodType, capacity));
						carrying = true;
						carriedGood = routeStop.goodType;
					} else {
						Debug.Log( string.Format("Can't buy {0}, because we're full!", routeStop.goodType));
					}
					break;

				case Route.StopType.Sell:
					Debug.Log("Selling goods");
					if (carrying && carriedGood == routeStop.goodType) {
						lorriesList.ModifyFunds(routeStop.planet.SellGoodsTo(routeStop.goodType, capacity));
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
}
