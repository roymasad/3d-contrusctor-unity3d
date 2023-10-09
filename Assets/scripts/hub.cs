using UnityEngine;
using System.Collections;

public class hub : MonoBehaviour {

	public ArrayList children;

	public bool locked = false;

	public bool canPlace;

	public bool isGhost;

	public hubtype type;

	// Use this for initialization
	void Start () {
	
		if (!isGhost) {
			app.Me.hubs.Add (this.gameObject);
			//Debug.Log (app.Me.hubs.Count);
		}
		
		//if (isGhost) this.gameObject.layer = 2;



		canPlace = true;

	}

	void Awake() {
		
		children = new ArrayList();
		
	}
	
	// Update is called once per frame
	void Update () {


		bool testplace = true;
		foreach (GameObject obj in children) {

			if (obj.GetComponent<connector>().canPlace == false) testplace = false;

		}
		if (testplace == true) canPlace = true;


	
	}





}
