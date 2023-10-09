using UnityEngine;
using System.Collections;

public class pencil : MonoBehaviour {

	public GameObject trunk;
	public GameObject edge1;
	public GameObject edge2;

	public float budUnit;


	// Use this for initialization
	void Start () {

		if (trunk.GetComponent<bud>().trunkLength == 0) trunk.GetComponent<bud>().trunkLength = budUnit;

		//Vector3 tempRot = new Vector3(0f,180f,0f);

		//this.transform.eulerAngles = tempRot;
	
	}
	
	// Update is called once per frame
	void Update () {
	

		//Vector3 tempedge1Pos = trunk.transform.position - (trunk.transform.forward * budUnit) ;
		Vector3 tempedge2Pos = trunk.transform.position + (trunk.transform.forward * trunk.GetComponent<bud>().trunkLength) ;

		Vector3 temptrunkScale;

		//edge1.transform.position  = tempedge1Pos;
		edge2.transform.position  = tempedge2Pos;

		temptrunkScale.z = trunk.GetComponent<bud>().trunkLength / budUnit;
		temptrunkScale.x = 1;
		temptrunkScale.y = 1;


		trunk.transform.localScale = temptrunkScale;


	}

	public void setTrunkLength(float val) {
		
		
		
		trunk.GetComponent<bud>().trunkLength = val * budUnit;
		
	}
}
