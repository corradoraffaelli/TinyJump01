using UnityEngine;
using System.Collections;

public class PlayerMovements : MonoBehaviour {

	public enum PlayerState
	{
		OnPlanet,
		OnAir,
		Default
	}

	public Transform playerBeginningPosition;

	//stati del player, attuale ed al frame precedente
	public PlayerState playerState = PlayerState.OnAir;
	//[HideInInspector]
	public PlayerState oldPlayerState = PlayerState.OnPlanet;

	bool jump = false;
	bool doubleJump = false;
	bool doubleJumpEnabled = true;

	bool death = false;

	//component
	Rigidbody2D rigidbody;
	Animator animator;

	//component esterni
	TrajectoryController trajectory;
	WorldCreator worldCreator;
	PointsController pointsController;

	//variabili che caratterizzano l'inizio del salto
	float ySpeed = -1.0f;
	float xSpeed = 0.0f;
	float jumpingTime = 0.0f;
	Vector3 jumpingPosition = Vector3.zero;

	float playerYSize = 0.0f;

	bool beginning = true;

	float doubleJumpSpeedMultiplier = 0.7f;

	void Start()
	{
		if (playerBeginningPosition != null)
			transform.position = playerBeginningPosition.position;

		//riferimenti ai component dell'oggetto
		rigidbody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		playerYSize = GetComponent<BoxCollider2D> ().bounds.size.y / 2.0f;

		//riferimenti ai component del gameController
		GameObject gameController = GameObject.FindGameObjectWithTag ("GameController");
		if (gameController != null) {
			trajectory = gameController.GetComponent<TrajectoryController>();
			worldCreator = gameController.GetComponent<WorldCreator>();
			pointsController = gameController.GetComponent<PointsController>();
		}

		jumpingPosition = transform.position;
	}

	void Update () 
	{
		InputManager ();

		OnAirMovements ();

		DoubleJumpManager ();
		JumpManager ();

		SetAnimations ();

		oldPlayerState = playerState;
	}

	//gestisce l'input utente, per ora non utilizzo una classe apposita
	void InputManager()
	{
		//se premo il tasto mentre sono su un pianeta, attivo la variabile per il salto
		if (Input.GetButtonUp("Jump") && playerState == PlayerState.OnPlanet)
			jump = true;

		//se premo il tasto mentre sono su in aria, e non ho già compiuto un doppio salto, attivo la variabile per il doppio salto
		if (Input.GetButtonUp ("Jump") && playerState == PlayerState.OnAir && doubleJumpEnabled) {
			doubleJump = true;
			doubleJumpEnabled = false;
		}
	}

	//gestisce i movimenti in aria del player
	void OnAirMovements()
	{
		//se sono in aria
		if (playerState == PlayerState.OnAir && trajectory != null) {
			
			//aggiorno le posizioni, basandomi sulle equazioni di una traiettoria parabolica
			float newXPos = jumpingPosition.x + xSpeed*(Time.time - jumpingTime);
			float newYPos = jumpingPosition.y + ySpeed*(Time.time - jumpingTime) - 1.0f/2.0f * trajectory.gravity * (Time.time - jumpingTime) * (Time.time - jumpingTime);
			transform.position = new Vector3(newXPos, newYPos, transform.position.z);
			
			//ruoto il player, metto il suo forward lungo la direzione della velocità
			//BUG!! da sistemare, probabilmmente basta un controllo sui segni
			if (!beginning)
			{
				float velY = ySpeed - trajectory.gravity * (Time.time - jumpingTime);
				transform.right = new Vector3(xSpeed, velY, transform.forward.z);
			}
			
		}
	}

	//gestisce il salto
	void JumpManager()
	{
		if (jump && trajectory!= null) {
			jump = false;
			
			transform.SetParent(null);

			//faccio in modo che il player salti lungo la sua verticale
			//assegno una velocità proporzianale alla relativa variabile di traiettoria
			Vector3 direction = transform.up;
			
			ySpeed = direction.y * trajectory.velocity;
			xSpeed = direction.x * trajectory.velocity;

			//setto i parametri di inizio salto
			jumpingTime = Time.time;
			jumpingPosition = transform.position;
			
			playerState = PlayerState.OnAir;
		}
	}

	//gestisce il doppio salto
	void DoubleJumpManager()
	{
		if (doubleJump && trajectory != null) {
			doubleJump = false;

			//faccio in modo che il player salti lungo la direzione inclinata di 45°
			Vector3 direction = new Vector3(1.0f,1.0f,0.0f);
			
			ySpeed = direction.y * trajectory.velocity * doubleJumpSpeedMultiplier;
			xSpeed = direction.x * trajectory.velocity * doubleJumpSpeedMultiplier;

			//setto i parametri di inizio salto
			jumpingTime = Time.time;
			jumpingPosition = transform.position;
		}
	}

	//gestisce le animazioni del player
	void SetAnimations()
	{
		if (animator != null) {
			if (playerState != oldPlayerState) {
				if (playerState == PlayerState.OnPlanet)
				{
					animator.SetBool("jump", false);
				}
				else if (playerState == PlayerState.OnAir)
				{
					animator.SetBool("jump", true);
				}
			}
		}

	}

	void DeathManager()
	{
		Application.LoadLevel ("01");
	}

	//mette il player come figlio del pianeta e fa le dovute operazioni
	void PlanetCollision(GameObject planetObj)
	{
		beginning = false;
		//ruoto il player nella direzione del pianeta
		//copiato, col lookAt non riuscivo
		Quaternion rotation = Quaternion.LookRotation
			(planetObj.transform.position - transform.position, transform.TransformDirection(Vector3.forward));
		transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

		//cambio lo stato del player
		playerState = PlayerState.OnPlanet;
		//metto il player come figlio del pianeta
		transform.SetParent(planetObj.transform);

		//mette il player alla giusta distanza dal pianeta (con le collisioni non è altrimenti preciso)
		SetPlayerDistance (planetObj);

		//controllo se ho raccolto tutti i coins precedenti, se così, dico al gestore di punti di aggiungere un moltiplicatore, altrimenti di resettare
		bool taken = worldCreator.VerifyCollectedCoins (planetObj);
		if (taken)
			pointsController.AddMultiplier ();
		else
			pointsController.ResetMultiplier ();

		//dico al controller del mondo qual è il pianeta attivo
		if (worldCreator != null)
			worldCreator.SetActivePlanet(planetObj);

		doubleJumpEnabled = true;
	}

	//se collido con un coin, aggiungo punti e lo nascondo
	void CoinCollision(GameObject coinOBJ)
	{
		Coin coinScript = coinOBJ.GetComponent<Coin>();
		if (coinScript != null)
		{
			coinScript.Hide();
		}
		if (pointsController != null)
		{
			pointsController.AddPoint();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Planet") {
			PlanetCollision(other.gameObject);
		}	

		if (other.tag == "Coin") {
			CoinCollision(other.gameObject);
		}	

		if (other.CompareTag ("Death"))
			DeathManager ();
	}

	//mette il player alla dovuta distanza da un pianeta, in base alla sua altezza ed al raggio del pianeta
	void SetPlayerDistance(GameObject planetObj)
	{
		Planet planetScript = planetObj.GetComponent<Planet> ();

		if (planetScript != null) {
			float distance = planetScript.radius + playerYSize;
			Vector3 actualPositionNorm = transform.localPosition.normalized;
			transform.localPosition = new Vector3(actualPositionNorm.x * distance, actualPositionNorm.y*distance, 0.0f);
		}

	}

	//restituisce la metà della grandezza del player, utile per piazzarlo alla giusta distanza dal pianeta e per iniziare la serie di coins
	public float GetPlayerYSize()
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		float playerYSize = -1.0f;
		if (player != null) {
			Collider2D collider = player.GetComponent<Collider2D> ();
			if (collider != null)
			{
				playerYSize = collider.bounds.size.y / 2.0f;
			}
		}
		return playerYSize;
	}

}
