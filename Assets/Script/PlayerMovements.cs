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

	bool jump = false;
	bool firstJump = true;
	
	Rigidbody2D rigidbody;
	TrajectoryController trajectory;

	//variabili che caratterizzano l'inizio del salto
	float ySpeed = 0.0f;
	float xSpeed = 0.0f;
	float jumpingTime = 0.0f;
	Vector3 jumpingPosition = Vector3.zero;

	float playerYSize = 0.0f;

	void Start()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		playerYSize = GetComponent<BoxCollider2D> ().bounds.size.y / 2.0f;
		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		if (gameController != null) {
			trajectory = gameController.GetComponent<TrajectoryController>();
		}
	}

	void Update () 
	{
		//se premo il bottone oppportuno, dico di saltare
		if (Input.GetButtonUp("Jump"))
		    jump = true;

		//se sono in aria
		if (playerState == PlayerState.OnAir && trajectory != null) {

			//aggiorno le posizioni
			float newXPos = jumpingPosition.x + xSpeed*(Time.time - jumpingTime);
			float newYPos = jumpingPosition.y + ySpeed*(Time.time - jumpingTime) - 1.0f/2.0f * trajectory.gravity * (Time.time - jumpingTime) * (Time.time - jumpingTime);
			transform.position = new Vector3(newXPos, newYPos, transform.position.z);

			//ruoto il player, metto il suo forward lungo la direzione della velocità
			//BUG!! da sistemare, probabilmmente basta un controllo sui segni
			float velY = ySpeed - trajectory.gravity * (Time.time - jumpingTime);
			transform.right = new Vector3(xSpeed, velY, transform.forward.z);
		}

		if (jump && trajectory!= null) {
			jump = false;

			transform.SetParent(null);

			Vector3 direction = transform.up;

			ySpeed = direction.y * trajectory.velocity;
			xSpeed = direction.x * trajectory.velocity;

			jumpingTime = Time.time;

			jumpingPosition = transform.position;

			playerState = PlayerState.OnAir;
		}
	}

	//mette il player come figlio del pianeta e fa le dovute operazioni
	void SetPlanetChild(GameObject planetObj)
	{
		//ruoto il player nella direzione del pianeta
		//copiato, col lookAt non riuscivo
		Quaternion rotation = Quaternion.LookRotation
			(planetObj.transform.position - transform.position, transform.TransformDirection(Vector3.forward));
		transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

		//cambio lo stato del player
		playerState = PlayerState.OnPlanet;
		//metto il player come figlio del pianeta
		transform.SetParent(planetObj.transform);

		SetPlayerDistance (planetObj);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Planet") {
			SetPlanetChild(other.gameObject);
		}	
	}

	void SetPlayerDistance(GameObject planetObj)
	{
		Planet planetScript = planetObj.GetComponent<Planet> ();

		if (planetScript != null) {
			//piazzo il player a distanza apposita
			float distance = planetScript.radius + playerYSize;

			Vector3 actualPositionNorm = transform.localPosition.normalized;

			transform.localPosition = new Vector3(actualPositionNorm.x * distance, actualPositionNorm.y*distance, 0.0f);
		}

	}

}
