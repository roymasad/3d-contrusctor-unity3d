using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class menu : MonoBehaviour {

	public static menu Me;

	public GameObject propertiesPanel;

	public GameObject pencilSlider;
	public GameObject objectLock;

	public bool toggling; //fix

	private ArrayList searchedGroup;

	public Material selectedMat;


	// Use this for initialization
	void Start () {


		propertiesPanel.SetActive(false);

		Me = this;

		//selectedMat = app.Me.selectedMat;
	
	}
	
	// Update is called once per frame
	void Update () {


		if (app.Me.selectedObj != null && app.Me.selectedObj.GetComponent<pencil>() != null) {

			pencilSlider.SetActive(true);
		}
		else pencilSlider.SetActive(false);

	
	}

	public void Clear() {

		//Application.LoadLevel("1");
		app.Me.ClearQalamsila ();

	}

	public void ToggleSnapping() {
		

		if (app.Me.snap == true)
			app.Me.snap = false;
		else
			app.Me.snap = true;

	}


	public void ToggleWarning() {
		
		
		if (app.Me.warning == true)
		{
			app.Me.warning = false;
			app.Me.selectedMat = app.Me.connectorMat;
		}
		else
		{
			app.Me.warning = true;
			app.Me.selectedMat = this.selectedMat;
		}
		
	}



	public void Simulate() {


		foreach (GameObject obj in app.Me.hubs) {


			//obj.AddComponent<Rigidbody>();

			//obj.GetComponent<Rigidbody>().drag = 0.2f;

			obj.GetComponent<Rigidbody>().isKinematic = false;

			obj.GetComponent<Rigidbody>().useGravity = true;

		}

	}


	public void setGizmoTransform() {

		gizmo.Me.setType("T");

	}

	public void setGizmoRotate() {
		
		gizmo.Me.setType("R");
		
	}

	public void showProperties(bool val) {

		if (val == true) propertiesPanel.SetActive(true);
		else propertiesPanel.SetActive(false);

	}

	public void toggleLock() {


		if (!toggling) app.Me.selectedObj.GetComponent<hub>().locked = !app.Me.selectedObj.GetComponent<hub>().locked;

	}

	public void trunkSlider() {


		app.Me.selectedObj.GetComponent<pencil>().setTrunkLength( pencilSlider.GetComponent<Slider>().value );

	}

	public void deleteBlock() {


		atomAction action = new atomAction ();
		
		action.actiontype = actionList.delete;
		action.obj = app.Me.selectedObj;
		action.objtype = app.Me.selectedObj.GetComponent<hub>().type;
		action.position = app.Me.selectedObj.transform.position;
		action.rotation = app.Me.selectedObj.transform.rotation;
		action.locked = app.Me.selectedObj.GetComponent<hub>().locked;
		if (action.objtype == hubtype.Pencil) action.pencillength = (app.Me.selectedObj.GetComponent<pencil> ().trunk).GetComponent<bud> ().trunkLength;

		app.Me.undoStack.Push (action);

		//app.Me.selectedObj.SetActive(false);
						
		menu.Me.showProperties(false);
		
		app.Me.gizmo.SetActive(false);

		app.Me.hubs.Remove(app.Me.selectedObj);

		Destroy(app.Me.selectedObj);

		app.Me.selectedObj = null;
		
	}

	public void groupSelect() {

		if (app.Me.groupSelect == true) { deselectSelection(); return; }

		searchedGroup = new ArrayList();

		groupSubSelect(app.Me.selectedObj);


		highlightSelection();
	}

	public void groupSubSelect(GameObject node) {


		if (node.GetComponent<hub>() != null) {


			if (!searchedGroup.Contains(node)) searchedGroup.Add(node);
			
			
			foreach (GameObject obj in node.GetComponent<hub>().children) {


				if (obj.GetComponent<connector>().connectedTo != null && !searchedGroup.Contains((obj.GetComponent<connector>().connectedTo).GetComponent<connector>().parenthub) ) groupSubSelect((obj.GetComponent<connector>().connectedTo).GetComponent<connector>().parenthub);

			}
			
			
			
		}



	}

	public void highlightSelection() {

		app.Me.groupSelect = true;

		app.Me.selectedGroup.transform.position = ((GameObject) searchedGroup[0]).transform.position;

		menu.Me.showProperties(false);

		foreach (GameObject obj in searchedGroup) {

			obj.transform.parent = app.Me.selectedGroup.transform;

			app.Me.selectedObj = obj.transform.parent.gameObject;

			foreach (GameObject subobj in obj.GetComponent<hub>().children) {

				subobj.GetComponent<Renderer>().material = app.Me.highlightedMat;

			}

			
		}


	}

	public void deselectSelection() {
		

		menu.Me.showProperties(false);
		
		app.Me.gizmo.SetActive(false);

		foreach (GameObject obj in searchedGroup) {

			obj.transform.parent = null;

			app.Me.selectedObj = null;
			
			foreach (GameObject subobj in obj.GetComponent<hub>().children) {
				
				subobj.GetComponent<Renderer>().material = subobj.GetComponent<connector>().myMat;
				
			}
			
			
		}

		app.Me.groupSelect = false;



	}

	public void loadDialogue() {


		this.GetComponent<Browser>().OpenFile( Application.dataPath);

	}

	public void saveDialogue() {


		string FileName = Application.dataPath + "/QTempSaveFile";

		app.Me.SaveQalamsila (FileName);

		this.GetComponent<Browser>().SaveFile( FileName, Application.dataPath);

	}

	public void undoLastAction() {

		app.Me.undoLastAction();


	}

}
