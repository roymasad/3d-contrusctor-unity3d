using UnityEngine;
using System.Collections;

public class axis : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnMouseDown() {


		this.transform.parent.gameObject.SendMessage ("AxisDown", this.name);

	}

	void OnMouseUp() {
		
		
		this.transform.parent.gameObject.SendMessage ("AxisUp", this.name);
		
	}

}
