using UnityEngine;
using System.Collections;

public class LorryClickable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUpAsButton() {
		transform.parent.parent.GetComponent<LorriesList>().SelectLorry(GetComponentInParent<Lorry>());
	}
}
