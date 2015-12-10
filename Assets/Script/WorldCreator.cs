using UnityEngine;
using System.Collections;

public class WorldCreator : MonoBehaviour {

	[System.Serializable]
	public class PlanetElement
	{
		public GameObject planet;
		public GameObject[] coins;
	}

	[SerializeField]
	public PlanetElement[] planetElements = new PlanetElement[0];

	public GameObject planetPrefab;
	public GameObject coinPrefab;

	public Transform beginningPosition;
	public int planetNumbers;

	public float spaceBetweenPlanets = 10.0f;
	public float randomSpaceBetweenPlanets = 2.0f;

	public float angle = 45.0f;
	public float randomAngle = 1.0f;

	TrajectoryController trajectory;

	void Awake()
	{
		CreatePlanets ();
	}

	void CreatePlanets()
	{
		planetElements = new PlanetElement[planetNumbers];

		for (int i = 0; i < planetNumbers; i++) {
			planetElements[i] = new PlanetElement();
			CreatePlanet(i);
		}
	}

	void CreatePlanet(int index)
	{
		Vector3 planetPosition = Vector3.zero;
		if (index == 0 && beginningPosition != null) 
		{
			planetPosition = beginningPosition.transform.position;
		} 
		else 
		{
			//trovo il punto di partenza della traiettoria
			float distance = 0.0f;
			float planetRadius = 0.0f;
			float playerYSize = GetPlayerYSize();
			if (planetElements[index-1].planet != null)
				planetRadius = GetPlanetRadius(planetElements[index-1].planet);

			distance = playerYSize + planetRadius;

			//a partire dal raggio del pianeta e della posizione del pianeta, calcolo il punto di partenza della traiettoria
			if (planetElements[index-1].planet != null)
			{
				float actualRandomAngle = Random.Range(-randomAngle, randomAngle);
				float actualAngle = angle + actualRandomAngle;
				float angleRad = Mathf.Deg2Rad*actualAngle;


				Vector3 oldPlanetPosition = planetElements[index-1].planet.transform.position;

				Vector3 startingPosition = new Vector3(oldPlanetPosition.x + distance*Mathf.Cos(angleRad), 
				                                       oldPlanetPosition.y + distance*Mathf.Sin(angleRad),
				                                       0.0f);


				//calcolo un punto lungo la traiettoria
				GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
				float velocity = 0.0f;

				if (gameController != null)
					trajectory  = gameController.GetComponent<TrajectoryController>();

				if (trajectory != null)
					velocity = trajectory.velocity;

				float randomSpace = Random.Range(-randomSpaceBetweenPlanets, randomSpaceBetweenPlanets);
				float deltaTime = (spaceBetweenPlanets + randomSpace) / velocity;

				float newXPos = startingPosition.x + velocity*Mathf.Cos(angleRad)* deltaTime;
				float newYPos = startingPosition.y + velocity*Mathf.Sin(angleRad)* deltaTime - 1.0f/2.0f * trajectory.gravity * deltaTime * deltaTime;

				planetPosition = new Vector3(newXPos, newYPos, 0.0f);
				float lastSpace = planetPosition.x - 2.0f;

				CreateCoins(startingPosition, 1.2f, lastSpace, new Vector3(velocity*Mathf.Cos(angleRad), velocity*Mathf.Sin(angleRad), 0.0f));
			}
		}

		if (planetElements [index] == null)
			planetElements [index] = new PlanetElement ();
		planetElements [index].planet = (GameObject)Instantiate (planetPrefab, planetPosition, Quaternion.identity);
	}

	void CreateCoins(Vector3 startingPosition, float space, float lastX, Vector3 velocity)
	{
		float beginningX = startingPosition.x + 1.0f;
		float actualX = beginningX;


		float actualSpace = space;
		while (actualX < lastX) {
			//float deltaTime = actualSpace / velocity.magnitude;
			float deltaTime = (actualX - startingPosition.x) / velocity.x;



			//float newXPos = startingPosition.x + velocity.x * deltaTime;
			float newYPos = startingPosition.y + velocity.y * deltaTime - 1.0f/2.0f * trajectory.gravity * deltaTime * deltaTime;

			Instantiate(coinPrefab, new Vector3(actualX, newYPos, 0.0f), Quaternion.identity);

			//actualSpace += actualSpace;
			actualX += space;
		}
	}

	float GetPlanetRadius(GameObject planetOB)
	{
		Collider2D collider = planetOB.GetComponent<Collider2D> ();
		float radius = -1.0f;
		if (collider != null)
			radius = collider.bounds.size.x / 2.0f;

		return radius;
	}

	float GetPlayerYSize()
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
