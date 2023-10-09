using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public enum hubtype {

	HubL, HubT, HubV, HubX, Pencil

}

[XmlRoot("HubCollection")]
public class hubCollection {

	public List<hubStruct> hubstream;

	public hubCollection () {

		hubstream = new List<hubStruct>();

	}

}


public class hubStruct {


	public Vector3 transformPOS;

	public Quaternion tranformROT;

	public bool locked;

	public hubtype type;

	public float pencilLength;


}


public enum actionList {

	transform, place, delete, lockhub, pencilresize

}

public class atomAction {


	public actionList actiontype;

	public GameObject obj;

	public hubtype objtype;

	public Vector3 position;

	public Quaternion rotation;

	public bool locked;

	public float pencillength;

}


public class app : MonoBehaviour {

	public GameObject gizmo;

	public GameObject hubV;

	public GameObject hubL;

	public GameObject hubT;
	
	public GameObject hubX;

	public GameObject pencil;

	public Material selectedMat;

	public Material highlightedMat;

	public Material connectorMat;

	public GameObject selectedObj;

	public static app Me;

	private bool mouseClickjudge;

	public float zoomlevel;

	public bool mustFocus;

	public ArrayList hubs;

	public hubCollection liveHubs;

	public bool SpawnReady;

	public GameObject ghost;

	private string TypeToSpawn;

	public bool groupSelect;

	public GameObject selectedGroup;

	public Stack<atomAction> undoStack;

	public bool snap;

	public bool warning;

	public float snapValuePOS;
	public float snapValueROT;

	// Use this for initialization
	void Start () {
	
		Me = this;

		selectedObj = null;

		zoomlevel = 10f;

		mustFocus = false;

		gizmo.SetActive(false);

		SpawnReady = false;

		ghost = null;

		groupSelect = false;

		snap = false;

		warning = true;

		snapValuePOS = 0.2f;

		snapValueROT = 15f;

		menu.Me.selectedMat = this.selectedMat;

	}

	void Awake() {

		//hubstream = new List<hubStruct>();

		liveHubs = new hubCollection ();

		hubs = new ArrayList ();

		undoStack = new Stack<atomAction> ();
	}
	
	// Update is called once per frame
	void Update () {


		//Debug.Log (Vector3.Distance(selectedObj.gameObject.transform.position, this.transform.position));

		if (selectedObj != null && Input.GetAxis("Mouse ScrollWheel") > 0 ) { transform.Translate(Vector3.forward * 1); zoomlevel = Vector3.Distance(selectedObj.transform.position, this.transform.position);}

		if (selectedObj != null && Input.GetAxis("Mouse ScrollWheel") < 0 ) { transform.Translate(Vector3.forward * -1); zoomlevel = Vector3.Distance(selectedObj.transform.position, this.transform.position);}



		if (selectedObj != null && Input.GetMouseButton(1))	{


			if (mustFocus) FocusToBlock();

			transform.LookAt(selectedObj.gameObject.transform);

			transform.RotateAround( selectedObj.transform.position, this.transform.right , Input.GetAxis( "Mouse Y" ) * -5 );

			transform.RotateAround( selectedObj.transform.position, this.transform.up , Input.GetAxis( "Mouse X" ) * 5 );



		}


		if (selectedObj != null && Input.GetMouseButton(2))	{

			transform.Translate(Input.GetAxis( "Mouse X" ) * -1 ,Input.GetAxis( "Mouse Y" ) * -1,0);

		}


		if (ghost != null) 
		{
			if (SpawnReady ==  true && Input.GetMouseButtonDown(0) && ghost.GetComponent<hub>().canPlace)
			{

				GameObject obj = new GameObject();

				//hubStruct objStream = new hubStruct(); 

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					
					Vector3 hitPos = hit.point;
					
					hitPos.y += 0.51f;
					
					if (hitPos.y < 0.51f ) hitPos.y = 0.51f;
					
					if (TypeToSpawn == "hubV") {obj = (GameObject) Instantiate(hubV, hitPos, Quaternion.identity);  obj.GetComponent<hub>().type = hubtype.HubV; }
					if (TypeToSpawn == "hubL") { obj = (GameObject) Instantiate(hubL, hitPos, Quaternion.identity); obj.GetComponent<hub>().type = hubtype.HubL; }
					if (TypeToSpawn == "hubT") { obj = (GameObject) Instantiate(hubT, hitPos, Quaternion.identity); obj.GetComponent<hub>().type = hubtype.HubT; }
					if (TypeToSpawn == "hubX") { obj = (GameObject) Instantiate(hubX, hitPos, Quaternion.identity); obj.GetComponent<hub>().type = hubtype.HubX; }
					if (TypeToSpawn == "pencil") { obj = (GameObject) Instantiate(pencil, hitPos, Quaternion.identity); obj.GetComponent<hub>().type = hubtype.Pencil; }
					
					SpawnReady = false;
					
					Destroy(ghost);



					atomAction action = new atomAction ();
					
					action.actiontype = actionList.place;
					action.obj = obj;

					app.Me.undoStack.Push (action);


					//objStream.transform = obj.transform;


					//hubs.Add(obj);

					//hubstream.Add(objStream);



				}



			}

			if (SpawnReady ==  true)
			{

				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {

					if (hit.collider.gameObject.transform.parent != null && hit.collider.gameObject.transform.parent.gameObject == ghost.gameObject) { hit.collider.gameObject.layer =2; return;}

					Vector3 hitPos = hit.point;

					hitPos.y += 0.51f;

					if (hitPos.y < 0.51f ) hitPos.y = 0.51f;
					
					ghost.transform.position = hitPos;
				}


			}



		}

		else if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				

				if (hit.collider.name == "Plane") 
				
				{

					app.Me.selectedObj = null;
					
					menu.Me.showProperties(false);
					
					app.Me.gizmo.SetActive(false);

				}
				

			}
	
		}
		
	}
	
	
	public void SmoothLook() {

		if (selectedObj == null)  return;
				
		this.GetComponent<SmoothLookAt>().target = selectedObj.gameObject.transform;
		
		//this.GetComponent<SmoothLookAt>().minDistance = zoomlevel;

	}


	public void FocusToBlock() {

		if (selectedObj == null)  return;

		while (Vector3.Distance(selectedObj.transform.position, this.transform.position) > zoomlevel)

		{

			transform.LookAt(selectedObj.transform);

			transform.Translate(Vector3.forward * 0.01f);		
			mustFocus = false;
		}


	}


	public void SpawnPencilAtMouse() {
		
		
		ghost = (GameObject) Instantiate(pencil, Vector3.zero, Quaternion.identity);
		
		ghost.GetComponent<hub>().isGhost = true;
		
		
		SpawnReady = true;		
		
		TypeToSpawn = "pencil";
		
		
	}

	public void SpawnHubVAtMouse() {


		ghost = (GameObject) Instantiate(hubV, Vector3.zero, Quaternion.identity);

		ghost.GetComponent<hub>().isGhost = true;


		SpawnReady = true;		

		TypeToSpawn = "hubV";
		
		
	}

	public void SpawnHubLAtMouse() {
		
		
		ghost = (GameObject) Instantiate(hubL, Vector3.zero, Quaternion.identity);
		
		ghost.GetComponent<hub>().isGhost = true;
		
		
		SpawnReady = true;		

		TypeToSpawn = "hubL";
		
	}

	public void SpawnHubTAtMouse() {
		
		
		ghost = (GameObject) Instantiate(hubT, Vector3.zero, Quaternion.identity);
		
		ghost.GetComponent<hub>().isGhost = true;
		
		
		SpawnReady = true;		
		
		TypeToSpawn = "hubT";
		
		
	}

	public void SpawnHubXAtMouse() {
		
		
		ghost = (GameObject) Instantiate(hubX, Vector3.zero, Quaternion.identity);
		
		ghost.GetComponent<hub>().isGhost = true;
		
		
		SpawnReady = true;		
		
		TypeToSpawn = "hubX";
		
		
	}

	public void OpenQalamsila(string filename) {

		ClearQalamsila ();

		liveHubs = new hubCollection ();

		XmlSerializer serializer = new XmlSerializer(typeof(hubCollection));
		FileStream stream = new FileStream(filename, FileMode.Open);
		hubCollection container = serializer.Deserialize(stream) as hubCollection;
		stream.Close();

		liveHubs = container;

		RebuildQalamsila ();

		Debug.Log (liveHubs.hubstream.Count);

	}

	public void SaveQalamsila(string filename) {
		
		
		liveHubs = new hubCollection ();

		foreach (GameObject obj in hubs) {


			hubStruct temphub = new hubStruct();


			temphub.transformPOS = obj.transform.position;

			temphub.tranformROT = obj.transform.rotation;

			temphub.type = obj.GetComponent<hub>().type;

			temphub.locked = obj.GetComponent<hub>().locked;

			if (temphub.type == hubtype.Pencil) {

				temphub.pencilLength = (obj.GetComponent<pencil>().trunk).GetComponent<bud>().trunkLength;

			}



			liveHubs.hubstream.Add(temphub);


		}


		XmlSerializer serializer = new XmlSerializer(typeof(hubCollection));
		FileStream stream = new FileStream(filename, FileMode.Create); 
		serializer.Serialize(stream, liveHubs);
		stream.Close();


		
	}

	public void ClearQalamsila() {

		liveHubs = new hubCollection ();

		foreach (GameObject obj in hubs) {

			Destroy(obj);

		}

		hubs = new ArrayList ();

		undoStack = new Stack<atomAction> ();

	}

	public void ResetQalamsila() {
		
		selectedObj.transform.rotation = Quaternion.identity;
		
	}

	public void RebuildQalamsila() {



		foreach (hubStruct hub in liveHubs.hubstream) {
			


			BuildHub(hub);


		}


	}

	public void BuildHub(hubStruct hub) {



		if (hub.type == hubtype.HubV) 
		{
			
			GameObject obj = new GameObject();
			
			obj = (GameObject) Instantiate(hubV, Vector3.zero, Quaternion.identity);  
			
			obj.GetComponent<hub>().type = hubtype.HubV;
			
			obj.transform.position = hub.transformPOS;
			
			obj.transform.rotation = hub.tranformROT;
			
			obj.GetComponent<hub>().locked = hub.locked;
			
			
			
		}
		
		if (hub.type == hubtype.HubL) 
		{
			
			GameObject obj = new GameObject();
			
			obj = (GameObject) Instantiate(hubL, Vector3.zero, Quaternion.identity);  
			
			obj.GetComponent<hub>().type = hubtype.HubL;
			
			obj.transform.position = hub.transformPOS;
			
			obj.transform.rotation = hub.tranformROT;
			
			obj.GetComponent<hub>().locked = hub.locked;
			
			
			
		}
		
		if (hub.type == hubtype.HubT) 
		{
			
			GameObject obj = new GameObject();
			
			obj = (GameObject) Instantiate(hubT, Vector3.zero, Quaternion.identity);  
			
			obj.GetComponent<hub>().type = hubtype.HubT;
			
			obj.transform.position = hub.transformPOS;
			
			obj.transform.rotation = hub.tranformROT;
			
			obj.GetComponent<hub>().locked = hub.locked;
			
			
			
		}
		
		if (hub.type == hubtype.HubX) 
		{
			
			GameObject obj = new GameObject();
			
			obj = (GameObject) Instantiate(hubX, Vector3.zero, Quaternion.identity);  
			
			obj.GetComponent<hub>().type = hubtype.HubX;
			
			obj.transform.position = hub.transformPOS;
			
			obj.transform.rotation = hub.tranformROT;
			
			obj.GetComponent<hub>().locked = hub.locked;
			
			
			
		}
		
		
		if (hub.type == hubtype.Pencil) 
		{
			
			GameObject obj = new GameObject();
			
			obj = (GameObject) Instantiate(pencil, Vector3.zero, Quaternion.identity);  
			
			obj.GetComponent<hub>().type = hubtype.Pencil;
			
			obj.transform.position = hub.transformPOS;
			
			obj.transform.rotation = hub.tranformROT;
			
			obj.GetComponent<hub>().locked = hub.locked;
			
			
			(obj.GetComponent<pencil>().trunk).GetComponent<bud>().trunkLength = hub.pencilLength;
			
		}
		
		
		
	}
	
	public void undoLastAction() {
		
		if (undoStack.Count == 0) return;
		
		atomAction lastaction = undoStack.Pop ();

		if (lastaction.actiontype == actionList.transform ) {


			lastaction.obj.transform.position = lastaction.position;
			lastaction.obj.transform.rotation = lastaction.rotation;

		}

	
		else if (lastaction.actiontype == actionList.lockhub ) {
			
			
			lastaction.obj.GetComponent<hub>().locked = lastaction.locked;
			
		}

		else if (lastaction.actiontype == actionList.pencilresize ) {
			
			
			(lastaction.obj.GetComponent<pencil>().trunk).GetComponent<bud>().trunkLength = lastaction.pencillength;
			
		}

		else if (lastaction.actiontype == actionList.place ) {
			
			
			hubs.Remove(lastaction.obj);

			Destroy (lastaction.obj);
			
		}

		else if (lastaction.actiontype == actionList.delete ) {
			

			hubStruct hub = new hubStruct();

			hub.transformPOS = lastaction.position;
			hub.tranformROT = lastaction.rotation;
			hub.type = lastaction.objtype;
			hub.pencilLength = lastaction.pencillength;
			hub.locked = lastaction.locked;

			BuildHub(hub);
			
		}

		
	}
	
	
}
