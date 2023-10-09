using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class connector : MonoBehaviour {


	public Material myMat;

	public Color myCol;

	public bool canPlace;

	public float angleHook;
	
	public GameObject parenthub;

	public GameObject connectedTo;


	// Use this for initialization
	void Start () {
	

		myMat = this.GetComponent<Renderer>().material;


		canPlace = false;

		myCol = this.GetComponent<Renderer>().material.color;


		parenthub = this.transform.parent.gameObject;


		parenthub.GetComponent<hub>().children.Add(this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {

		//this.gameObject.layer = parenthub.layer;
			
	}


	void OnMouseDown() {


		if (app.Me.SpawnReady == true) return;


		app.Me.selectedObj = parenthub;

		app.Me.mustFocus = true;

		app.Me.SmoothLook();

		app.Me.gizmo.transform.position = this.transform.parent.gameObject.transform.position;

		app.Me.gizmo.SetActive(true);

		menu.Me.toggling = true;

		menu.Me.objectLock.GetComponent<Toggle>().isOn = parenthub.GetComponent<hub>().locked;

		if (this.transform.parent.gameObject.GetComponent<pencil>() != null) menu.Me.pencilSlider.GetComponent<Slider>().value = this.transform.parent.gameObject.GetComponent<pencil>().trunk.transform.localScale.z;

		menu.Me.toggling = false;

		menu.Me.showProperties(true);



	}

	void OnTriggerStay(Collider other) {


		bool passed = false;

		GameObject referenceEdge = null;

		if (other.gameObject.transform.parent == this.transform.parent) return;

		if (other.gameObject.GetComponent<connector>() == null) return;

		if (other.gameObject.GetComponent<bud>() == null && this.GetComponent<bud>() == null) {flagBlock(); return;}

		if (this.GetComponent<bud>() != null && this.GetComponent<bud>().edgebud == false) return;

		if (other.gameObject.GetComponent<bud>() != null && other.gameObject.GetComponent<bud>().edgebud == false) return;


		if (this.GetComponent<bud>() != null && this.GetComponent<bud>().edgebud == true && other.gameObject.GetComponent<bud>() == null) { passed = true; referenceEdge = other.gameObject;}

		else if (this.GetComponent<bud>() == null && other.gameObject.GetComponent<bud>() != null && other.gameObject.GetComponent<bud>().edgebud == true ) {passed = true; referenceEdge = this.gameObject;}



		float forwardAngle = Mathf.Floor(Vector3.Angle(other.gameObject.transform.forward, this.gameObject.transform.forward));

		if (forwardAngle >=-1f && forwardAngle <=1f) forwardAngle = 0f;
		if (forwardAngle >=179f && forwardAngle <=181f) forwardAngle = 180f;

		if (forwardAngle == 0f || forwardAngle == 180f) passed = true; else passed = false;

		float upAnlge = Mathf.Floor(Vector3.Angle(other.gameObject.transform.up, this.gameObject.transform.up));


		if (referenceEdge == null || referenceEdge.GetComponent<connector>() == null) return;
		float angleToCheck = referenceEdge.GetComponent<connector>().angleHook;

		if ((Mathf.Abs(angleToCheck  - (upAnlge % angleToCheck)) < 2 || (upAnlge % angleToCheck) < 2 ) && passed == true) passed = true; else passed = false;


		if ( Vector3.Distance( this.gameObject.transform.position , other.gameObject.transform.position ) < 1.1f && Vector3.Distance( this.gameObject.transform.position , other.gameObject.transform.position ) > 0.69f && passed == true) passed = true; else passed = false;

		//Debug.Log( Vector3.Distance( this.gameObject.transform.position , other.gameObject.transform.position) );

		if (passed) {

			canPlace = true;
			
			if (!app.Me.groupSelect) this.GetComponent<Renderer>().material = myMat;

			connectedTo = other.gameObject;

			return;
		}

		else flagBlock();


		



	}

	void flagBlock() {

		canPlace = false;
		this.GetComponent<Renderer>().material = app.Me.selectedMat;

	}

	void OnTriggerExit(Collider other) {

		canPlace = true;

		this.GetComponent<Renderer>().material = myMat;
		
	}



}
