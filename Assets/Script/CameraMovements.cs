using UnityEngine;
using System.Collections;

public class CameraMovements : MonoBehaviour {

	//indicano lo spazio lateralmente i pianeti, sopra il pianeta più alto e sopra il coin più alto
	public float extraLateralSpace = 4.0f;
	public float extraSpaceOverPlanet = 4.0f;
	public float extraSpaceOverCoins = 4.0f;

	public float lerpSpeedMovement = 2.0f;
	public float lerpSpeedSize = 2.0f;

	WorldCreator worldCreator;
	public PlayerMovements playerMovements;

	bool followingPlayer = false;
	Vector3 diffPosition = Vector3.zero;

	PlayerMovements.PlayerState oldPlayerState;

	GameObject player;

	void Start () {
		GameObject controller = GameObject.FindGameObjectWithTag ("GameController");
		if (controller != null) {
			worldCreator = controller.GetComponent<WorldCreator>();
		}

		player = GameObject.FindGameObjectWithTag ("Player");
		//playerMovements = player.GetComponent<PlayerMovements> ();
	}

	void Update () {
		if (worldCreator != null)
		{

			bool followPlayer = false;
			if (playerMovements != null)
			{
				Debug.Log ("cambiato");
				if (playerMovements.playerState != oldPlayerState)
				{
					Debug.Log ("cambiato");
					if (playerMovements.playerState == PlayerMovements.PlayerState.OnPlanet)
						followPlayer = false;
					if (playerMovements.playerState == PlayerMovements.PlayerState.OnAir)
					{
						followPlayer = true;
						Debug.Log ("follow player");
					}
				}
			}

			//si prendono il pianeta attuale e quello successivo (se esiste) e si passano alle funzioni che gestiscono la camera
			if (!followPlayer)
			{
				//resetto la variabile
				followingPlayer = false;

				int actualIndex = worldCreator.GetActivePlanetIndex();
				if (actualIndex != -1)
				{
					GameObject activePlanet = worldCreator.GetPlanetObject(actualIndex);
					GameObject nextPlanet = worldCreator.GetPlanetObject(actualIndex+1);
					
					if (activePlanet != null && nextPlanet != null)
					{
						SetCameraBetweenObjects(activePlanet, nextPlanet);
						SetCameraSize(activePlanet, nextPlanet, actualIndex);
					}
				}
			}
			else
			{
				//la camera dovrebbe seguire il player
				if (!followingPlayer)
				{
					diffPosition = transform.position - player.transform.position;
					Debug.Log ("salvata pos");
				}

				followingPlayer = true;

				Vector3 destPosition = transform.position + diffPosition;

				transform.position = Vector3.Lerp(transform.position, destPosition, Time.deltaTime * 2.0f);
			}
		}

		if (playerMovements != null)
			oldPlayerState = playerMovements.playerState;
	}

	//fa muovere la camera tra i due pianeti
	void SetCameraBetweenObjects(GameObject obj01, GameObject obj02)
	{
		Vector3 firstPosition = obj01.transform.position;
		Vector3 secondPosition = obj02.transform.position;

		Vector3 centerPosition = (firstPosition + secondPosition) / 2.0f;
		centerPosition = new Vector3 (centerPosition.x, centerPosition.y, transform.position.z);

		transform.position = Vector3.Lerp(transform.position, centerPosition, Time.deltaTime* lerpSpeedMovement);
	}

	void SetCameraSize(GameObject obj01, GameObject obj02, int actualIndex)
	{
		Vector3 firstPosition = obj01.transform.position;
		Vector3 secondPosition = obj02.transform.position;

		//faccio in modo che i pianeti siano lateralmente visibili, prendo la loro distanza lungo x, sommo la soglia e la applico al size della camera
		float xDiff = Mathf.Abs(firstPosition.x - secondPosition.x);
		float maxXSize = xDiff + extraLateralSpace;
		float cameraSize = (maxXSize / Camera.main.aspect) / 2.0f;

		//faccio in modo che se, verticalmente, lo spazio non dovesse essere sufficiente, il size della camera si adatti
		//prendo i limiti superiori (pianeta o coin) ed inferiori. Faccio la differenza e la applico al size della camera
		float maxCoinY = worldCreator.GetMaxY (actualIndex + 1);
		float maxCoinYExtra = maxCoinY + extraSpaceOverCoins;

		float yLimit = maxCoinYExtra;
		if (firstPosition.y + extraSpaceOverPlanet / 2.0f > yLimit)
			yLimit = firstPosition.y + extraSpaceOverPlanet / 2.0f;
		if (secondPosition.y + extraSpaceOverPlanet / 2.0f > yLimit)
			yLimit = secondPosition.y + extraSpaceOverPlanet / 2.0f;

		float yBottomLimit = firstPosition.y - extraSpaceOverPlanet / 2.0f;
		if (secondPosition.y - extraSpaceOverPlanet / 2.0f < yLimit)
			yBottomLimit = secondPosition.y - extraSpaceOverPlanet / 2.0f;

		//float yDiff = Mathf.Abs(firstPosition.y - secondPosition.y);
		//float maxYSize = (yDiff + extraSpaceOverPlanet) / 2.0f;

		float yDiff = Mathf.Abs(yLimit - yBottomLimit);
		float maxYSize = yDiff / 2.0f;

		if (cameraSize < maxYSize)
			cameraSize = maxYSize;

		Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, cameraSize, Time.deltaTime * lerpSpeedSize);
	}

	/*
	void CameraOnPlayer(bool onPlayer)
	{
		if (onPlayer && player != null)
			transform.SetParent (player);
		else
			transform.SetParent (null);
	}
	*/
}
