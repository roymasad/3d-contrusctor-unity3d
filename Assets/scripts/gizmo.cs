using UnityEngine;
using System.Collections;

public class gizmo : MonoBehaviour {


	public GameObject XAxisT;
	public GameObject YAxisT;
	public GameObject ZAxisT;

	public GameObject XAxisR;
	public GameObject YAxisR;
	public GameObject ZAxisR;

	public string Type;
	private string TypeEvent;

	private string LastTypeEvent;

	private float accumulator;

	public static gizmo Me;

	// Use this for initialization
	void Start () {
	
		hideAll();

		setType("T");

		Me = this;


	}



	// Update is called once per frame
	void Update () {
	

		if (app.Me.selectedObj == null) {
			//hideAll ();
			return;
		}


		float camObjAngleZ = 0;
		float camObjAngleX = 0;
		float camObjAngleY = 0;
		
		float camObjRAngleZ = 0;
		float camObjRAngleX = 0;
		float camObjRAngleY = 0;

		if (TypeEvent != null) {

			LastTypeEvent = TypeEvent;

			if (Type == "T") {

				camObjAngleX = Quaternion.Angle (XAxisT.transform.rotation, app.Me.transform.rotation);

				camObjAngleZ = Quaternion.Angle (ZAxisT.transform.rotation, app.Me.transform.rotation);

				camObjAngleY = Quaternion.Angle (YAxisT.transform.rotation, app.Me.transform.rotation);

			}
			//camObjRAngleX =  Vector3.Angle(XAxisR.transform.forward, app.Me.transform.forward);
			
			//camObjRAngleZ =  Vector3.Angle(ZAxisR.transform.forward, app.Me.transform.forward);
			
			//camObjRAngleY =  Vector3.Angle(YAxisR.transform.forward, app.Me.transform.forward);

			//Debug.Log (camObjAngelX);

		}

		if (app.Me.selectedObj != null)
			this.transform.position = app.Me.selectedObj.transform.position;
		
		if (Type == "R")
			this.transform.rotation = app.Me.selectedObj.transform.rotation;
		if (Type == "T")
			this.transform.rotation = new Quaternion (0, 0, 0, 0);

		if (app.Me.selectedObj.GetComponent<hub> () != null && app.Me.selectedObj.GetComponent<hub> ().locked == true)
			return;

		atomAction action = new atomAction ();

		action.actiontype = actionList.transform;
		action.obj = app.Me.selectedObj;
		action.position = app.Me.selectedObj.transform.position;
		action.rotation = app.Me.selectedObj.transform.rotation;

		bool pushtostack = false;

		if (TypeEvent == "XAxisT" || TypeEvent == "YAxisT" || TypeEvent == "ZAxisT")
			pushtostack = true;

		if (TypeEvent == "XAxisT" && camObjAngleX <= 120) {
			app.Me.selectedObj.transform.Translate (Input.GetAxis ("Mouse X") * 0.5f, 0, 0, Space.World);
		}
		if (TypeEvent == "XAxisT" && camObjAngleX > 120) {
			app.Me.selectedObj.transform.Translate (Input.GetAxis ("Mouse X") * -0.5f, 0, 0, Space.World);
		}

		if (TypeEvent == "YAxisT") {
			app.Me.selectedObj.transform.Translate (0, Input.GetAxis ("Mouse Y") * 0.5f, 0, Space.World);
		}

		if (TypeEvent == "ZAxisT" && camObjAngleZ <= 120) {
			app.Me.selectedObj.transform.Translate (0, 0, Input.GetAxis ("Mouse X") * -0.5f, Space.World);
		}
		if (TypeEvent == "ZAxisT" && camObjAngleZ > 120) {
			app.Me.selectedObj.transform.Translate (0, 0, Input.GetAxis ("Mouse X") * 0.5f, Space.World);
		}




		//if (TypeEvent == "XAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(Input.GetAxis( "Mouse Y" ) * 2.0f ,0,0); }
		//if (TypeEvent == "YAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(0 ,Input.GetAxis( "Mouse X" ) * -2.0f,0); }
		//if (TypeEvent == "ZAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(0 ,0,Input.GetAxis( "Mouse Y" ) * -2.0f); }

		//if (TypeEvent == "XAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(Input.GetAxis( "Mouse X" ) * 2.0f ,0,0); }
		//if (TypeEvent == "YAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(0 ,Input.GetAxis( "Mouse Y" ) * -2.0f,0); }
		//if (TypeEvent == "ZAxisR") {pushtostack = true;	app.Me.selectedObj.transform.Rotate(0 ,0,Input.GetAxis( "Mouse X" ) * -2.0f); }


		int mouseX = 0, mouseY = 0;
		float incval = 4f;

		if (Input.GetAxis ("Mouse X") < 0)
			mouseX = -1;
		else if (Input.GetAxis ("Mouse X") > 0)
			mouseX = 1; 

		if (Input.GetAxis ("Mouse Y") < 0)
			mouseY = -1;
		else if (Input.GetAxis ("Mouse Y") > 0)
			mouseY = 1; 

		if (TypeEvent == "XAxisR" || TypeEvent == "YAxisR" || TypeEvent == "ZAxisR")
			pushtostack = true;

		if (app.Me.snap == true)
			incval = app.Me.snapValueROT;
		else
		{
			accumulator = 20f;
			incval = 4f;
		}

		if (TypeEvent == "XAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(mouseY * incval ,0,0); accumulator = 0;}
		if (TypeEvent == "YAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(0 ,mouseX * -incval,0); accumulator = 0;}
		if (TypeEvent == "ZAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(0 ,0,mouseY * -incval); accumulator = 0;}
		
		if (TypeEvent == "XAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(mouseX * incval ,0,0); accumulator = 0;}
		if (TypeEvent == "YAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(0 ,mouseY * -incval,0); accumulator = 0;}
		if (TypeEvent == "ZAxisR" && accumulator > 10f) {app.Me.selectedObj.transform.Rotate(0 ,0,mouseX * -incval); accumulator = 0;}


		if (TypeEvent != null) {
			accumulator += Mathf.Abs (Input.GetAxis ("Mouse X"));
			accumulator += Mathf.Abs (Input.GetAxis ("Mouse Y"));
		}
		else
			accumulator = 0;
		//Debug.Log (accumulator);




		if (Input.GetMouseButtonDown (0) && pushtostack == true) {

			app.Me.undoStack.Push (action);
		}



		if (Input.GetMouseButtonUp (0) && app.Me.snap == true) {




			Vector3 tempPOS = app.Me.selectedObj.transform.position;


			if (LastTypeEvent == "XAxisT")tempPOS.x = (Mathf.Floor(tempPOS.x / app.Me.snapValuePOS) * app.Me.snapValuePOS); 
			if (LastTypeEvent == "YAxisT")tempPOS.y = (Mathf.Floor(tempPOS.y / app.Me.snapValuePOS) * app.Me.snapValuePOS); 
			if (LastTypeEvent == "ZAxisT")tempPOS.z = (Mathf.Floor(tempPOS.z / app.Me.snapValuePOS) * app.Me.snapValuePOS); 
			 


			app.Me.selectedObj.transform.position = tempPOS;


			LastTypeEvent = "";



		}


	}

	void hideAll() {


		XAxisT.SetActive(false);
		YAxisT.SetActive(false);
		ZAxisT.SetActive(false);

		XAxisR.SetActive(false);
		YAxisR.SetActive(false);
		ZAxisR.SetActive(false);

	}

	public void setType(string type) {

		if (type == "T") {

			hideAll();

			XAxisT.SetActive(true);
			YAxisT.SetActive(true);
			ZAxisT.SetActive(true);
			Type = "T";

		}

		if (type == "R") {
			
			hideAll();
			
			XAxisR.SetActive(true);
			YAxisR.SetActive(true);
			ZAxisR.SetActive(true);
			Type = "R";
			
		}
	}

	public void AxisDown(string axis) {


		if (axis == "XAxisT") TypeEvent = "XAxisT";
		if (axis == "YAxisT") TypeEvent = "YAxisT";
		if (axis == "ZAxisT") TypeEvent = "ZAxisT";

		if (axis == "XAxisR") TypeEvent = "XAxisR";
		if (axis == "YAxisR") TypeEvent = "YAxisR";
		if (axis == "ZAxisR") TypeEvent = "ZAxisR";

	}

	public void AxisUp(string axis) {


		TypeEvent = null;
	}


}
