using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	public float rotationSpeed = 2.0f;

	float radius = 0.0f;

	void Start () {
		BoxCollider2D collider = GetComponent<BoxCollider2D> ();
		if (collider != null)
			radius = collider.bounds.size.x / 2.0f;
	}

	void Update () {
		Vector3 oldAngles = transform.localEulerAngles;
		transform.localEulerAngles = new Vector3 (oldAngles.x, oldAngles.y, oldAngles.z + Time.deltaTime * rotationSpeed);
	}
}
