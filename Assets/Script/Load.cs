using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (2f);
		Application.LoadLevel ("01"); 	
	}
	

}
