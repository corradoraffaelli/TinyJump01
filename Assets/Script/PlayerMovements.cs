using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour {

	public enum PlayerState
	{
		OnPlanet,
		OnAir,
		Default
	}

	public PlayerState playerState;

	public float jumpFactor = 2.0f;
	public float maxFallingSpeed = 5.0f;
	
	bool jump = false;

	Rigidbody2D rigidbody;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	void Update () 
	{
		//se premo il bottone oppportuno, dico di saltare
		if (Input.GetButtonUp("Jump"))
		    jump = true;

		//limite velocità di caduta
		if (rigidbody.velocity.y < -maxFallingSpeed)
			rigidbody.velocity = new Vector2 (rigidbody.velocity.x, -maxFallingSpeed);

		//controllo la rotazione del player se è in aria
		if (playerState == PlayerState.OnAir) {
			Vector3 oldAngles = transform.localEulerAngles;
			transform.localEulerAngles = new Vector3(oldAngles.x, oldAngles.y, oldAngles.z + Time.deltaTime* 60.0f);
		}
	}

	void FixedUpdate()
	{
		//se la variabile del salto è attiva, aggiungo la forza)
		if (jump) {
			rigidbody.isKinematic = false;
			transform.SetParent(null);
			jump = false;
			//rigi
			Vector2 jumpDirection = new Vector2(transform.up.x, transform.up.y);
			jumpDirection.Scale(new Vector2(jumpFactor, jumpFactor));
			Debug.Log (jumpDirection);
			rigidbody.AddForce(jumpDirection);

			playerState = PlayerState.OnAir;

			//rigidbody.AddForce(new Vector2(transform.up.x, transform.up.y * jumpFactor));
		}
	}

	//mette il player come figlio del pianeta
	void setPlanetChild()
	{

	}
}
