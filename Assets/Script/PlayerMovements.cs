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

	public float changeXSpeed = 5.0f;

	public float rotationAirSpeed = 3.0f;
	
	public bool jump = false;

	Rigidbody2D rigidbody;

	bool firstJump = true;

	public float gravity = -8.0f;
	public float velocity = 8.0f;

	float ySpeed = 0.0f;
	float xSpeed = 0.0f;
	float jumpingTime = 0.0f;
	Vector3 jumpingPosition = Vector3.zero;

	float playerYSize = 0.0f;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		playerYSize = GetComponent<BoxCollider2D> ().bounds.size.y / 2.0f;
	}

	void Update () 
	{
		//se premo il bottone oppportuno, dico di saltare
		if (Input.GetButtonUp("Jump"))
		    jump = true;


		if (playerState == PlayerState.OnAir) {
			float newXPos = jumpingPosition.x + xSpeed*(Time.time - jumpingTime);

			float newYPos = jumpingPosition.y + ySpeed*(Time.time - jumpingTime) - 1.0f/2.0f * gravity * (Time.time - jumpingTime) * (Time.time - jumpingTime);

			transform.position = new Vector3(newXPos, newYPos, transform.position.z);
		}

		if (jump) {
			jump = false;

			transform.SetParent(null);

			Vector3 direction = transform.up;

			ySpeed = direction.y * velocity;
			xSpeed = direction.x * velocity;

			jumpingTime = Time.time;

			jumpingPosition = transform.position;

			playerState = PlayerState.OnAir;
		}

		//limite velocità di caduta
		//if (rigidbody.velocity.y < -maxFallingSpeed)
		//	rigidbody.velocity = new Vector2 (rigidbody.velocity.x, -maxFallingSpeed);

		//if (playerState == PlayerState.OnAir) {
			//controllo la rotazione del player se è in aria
			//la rotazione del player deve essere coerente con la parabola che sta percorrendo
			/*
			Vector2 velocityVector = rigidbody.velocity;
			Vector2 velocityNormal = new Vector2(-velocityVector.y, velocityVector.x);
			Vector3 tempUp = new Vector3(velocityNormal.x, velocityNormal.y, transform.up.z);
			transform.up = Vector3.Lerp(transform.up, tempUp, Time.deltaTime * rotationAirSpeed);
			*/

			//faccio tendere la velocità lungo l'asse X a 0. Non è una cosa strettamente necessaria, ma per lunghi salti può essere 
			//visivamente piacevole
			/*
			float oldHorizSpeed = rigidbody.velocity.x;
			float newHorizSpeed = 0.0f;
			if (oldHorizSpeed > 0)
			{
				newHorizSpeed = oldHorizSpeed - changeXSpeed*Time.deltaTime;
				if (newHorizSpeed < 0)
					newHorizSpeed = 0.0f;
			}
			if (oldHorizSpeed < 0)
			{
				newHorizSpeed = oldHorizSpeed + changeXSpeed*Time.deltaTime;
				if (newHorizSpeed > 0)
					newHorizSpeed = 0.0f;
			}

			rigidbody.velocity = new Vector2 (newHorizSpeed, rigidbody.velocity.y);
			*/
		//}
	}

	void FixedUpdate()
	{
		/*
		if (playerState == PlayerState.OnAir) {
			if (firstJump)
			{
				firstJump = false;
				Debug.Log (rigidbody.velocity.magnitude);
			}
		}
		*/

		//if (playerState == PlayerState.OnPlanet)
		//	rigidbody.velocity = Vector2.zero;

		//se la variabile del salto è attiva, aggiungo la forza)
		/*
		if (jump) {
			rigidbody.isKinematic = false;
			transform.SetParent(null);
			jump = false;


			//impongo una forza
			//Vector2 jumpDirection = new Vector2(transform.up.x, transform.up.y);
			//jumpDirection.Scale(new Vector2(jumpFactor, jumpFactor));
			//Debug.Log (jumpDirection);
			//rigidbody.AddForce(jumpDirection);


			//impongo una velocità iniziale (può essere utile per il calcolo della traiettoria
			Vector2 jumpDirection = (new Vector2(transform.up.x, transform.up.y)).normalized;
			jumpDirection.Scale(new Vector2(jumpFactor, jumpFactor));
			rigidbody.velocity = jumpDirection;


			playerState = PlayerState.OnAir;
		}
		*/


	}

	//mette il player come figlio del pianeta
	void SetPlanetChild(GameObject planetObj)
	{
		//Vector3 diff = planetObj.transform.position - transform.position;
		//diff = new Vector3 (diff.x, 0.0f, diff.z);
		//Quaternion rotation = Quaternion.LookRotation(diff);
		//transform.rotation = rotation;
		//transform.LookAt (planetObj.transform.position, Vector3.forward);

		Quaternion rotation = Quaternion.LookRotation
			(planetObj.transform.position - transform.position, transform.TransformDirection(Vector3.forward));
		transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

		playerState = PlayerState.OnPlanet;
		transform.SetParent(planetObj.transform);

		/*
		playerState = PlayerState.OnPlanet;
		transform.SetParent(planetObj.transform);


		//Vector2 direction = (new Vector2 (-transform.localPosition.x, -transform.localPosition.y)).normalized;
		Vector2 direction = (new Vector2 (-(transform.position.x - planetObj.transform.position.x), 
		                                  -(transform.position.y) - planetObj.transform.position.y)).normalized;
		transform.up = -direction;

		rigidbody.velocity = Vector2.zero;

		rigidbody.isKinematic = true;
		*/
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Planet") {
			SetPlanetChild(other.gameObject);
			Debug.Log ("colliso con pianeta");
		}
			
	}
}
