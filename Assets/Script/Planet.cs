using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	public float rotationSpeed = 2.0f;

	void Start () {
	
	}

	void Update () {
		Vector3 oldAngles = transform.localEulerAngles;
		transform.localEulerAngles = new Vector3 (oldAngles.x, oldAngles.y, oldAngles.z + Time.deltaTime * rotationSpeed);
	}
}
