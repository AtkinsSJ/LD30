using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

	public Material[] planetMaterials;

	public string planetName = "A Planet";
	public float scale = 1;
	public float rotationSpeed = 1;

	private Transform highlightTransform;
	private bool _highlighted = false;
	public bool highlighted {
		get { return _highlighted; }
		set {
			_highlighted = value;
			highlightTransform.gameObject.SetActive(value);
		}
	}

	public int[] goodValues = new int[(int)GoodType.SIZE];

	private int[] buying;
	public int[] BuyingGoods {
		get { return buying; }
	}
	private int[] selling;
	public int[] SellingGoods {
		get { return selling; }
	}

	// Use this for initialization
	void Start() {
		// Find the highlight!
		highlightTransform = transform.Find("Highlight");
		highlightTransform.gameObject.SetActive(false);

		// Random size!
		scale = Random.Range(0.5f, 2.5f);
		transform.localScale += new Vector3(scale-1, scale-1, scale-1);

		// Random texture
		renderer.material = planetMaterials[Random.Range(0, planetMaterials.Length)];
		switch (Random.Range(0, 3)) {
			case 0:
				renderer.material.color = new Color(Random.Range(0f, 0.5f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
				break;
			case 1:
				renderer.material.color = new Color(Random.Range(0.5f, 1f), Random.Range(0f, 0.5f), Random.Range(0.5f, 1f));
				break;
			case 2:
				renderer.material.color = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0f, 0.5f));
				break;
		}

		// Random tilt
		transform.Rotate(Random.Range(-20f, 20f), Random.Range(0f, 360f), Random.Range(-20f, 20f));
		// Random rotation speed
		rotationSpeed = Random.Range(-30f, 30f);

		// Random goods available and desired, with prices
		for (int i = 0; i < (int)GoodType.SIZE; i++) {

			switch (Random.Range(0, 3)) {
				case 0: // We don't buy or sell this good
					goodValues[i] = 0;
					break;

				case 1: // We sell this good
					goodValues[i] = Random.Range(2, 10);
					break;

				case 2: // We buy this good
					goodValues[i] = -Random.Range(2, 10);
					break;
			}
		}
		GenerateBuyingSellingLists();

		// Random name!
		Galaxy galaxy = GetComponentInParent<Galaxy>();
		string pn = PlanetNameGenerator.GeneratePlanetName();
		while (galaxy.planetNames.Contains(pn)) {
			pn = PlanetNameGenerator.GeneratePlanetName();
		}
		this.planetName = pn;
		galaxy.planetNames.Add(pn);
	}

	private void GenerateBuyingSellingLists() {
		List<int> buyingList = new List<int>();
		List<int> sellingList = new List<int>();

		for (int i = 0; i < goodValues.Length; i++) {
			if (goodValues[i] < 0) {
				buyingList.Add(i);
			} else if (goodValues[i] > 0) {
				sellingList.Add(i);
			}
		}

		buying = buyingList.ToArray();
		selling = sellingList.ToArray();
	}
	
	// Update is called once per frame
	void Update() {
		transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
	}

	void OnMouseUpAsButton() {
		GetComponentInParent<Galaxy>().ShowWindowForPlanet(this);
	}

	/**
	 * Buy goods from planet. Returns the cost as a negative number.
	 */
	public int BuyGoodsFrom(GoodType goodType, int quantity) {
		// TODO: Implement buying from planet
		int result = quantity * -goodValues[(int)goodType];
		if (result >= 0) throw new System.ArgumentException("This planet refuses to sell you " + goodType + "!");
		return result;
	}

	/**
	 * Sell goods to planet. Returns the income.
	 */
	public int SellGoodsTo(GoodType goodType, int quantity) {
		// TODO: Implement selling to planet
		int result = quantity * -goodValues[(int)goodType];
		if (result <= 0) throw new System.ArgumentException("This planet refuses to buy " + goodType + "!");
		return result;
	}
}
