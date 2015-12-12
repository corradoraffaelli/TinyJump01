using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldCreator : MonoBehaviour {

	//classe per gestire "sezioni" contenenti pianeta e coins
	[System.Serializable]
	public class PlanetElement
	{
		//pianeta
		public GameObject planet;
		//lista di coins precedenti al pianeta
		public List<GameObject> coins;

		//indica se il pianeta è quello attualmente attivo
		public bool active = false;
		//indica la Y più alta occupata da un coin
		public float maxY = Mathf.NegativeInfinity;
	}

	[SerializeField]
	public PlanetElement[] planetElements = new PlanetElement[0];

	//prefab di pianeti e coin
	public GameObject[] planetPrefabs;
	public GameObject coinPrefab;

	//oggetto che, se selezionato, fa da padre ai pianeti ed ai coins
	public GameObject father;

	//posizione del primo pianeta
	public Transform beginningPosition;

	//numero di pianeti da disegnare
	public int planetNumbers;

	//i seguenti due parametri sono attualmente inutilizzati, si randomizza il tempo tra i pianeti
	[HideInInspector]
	public float spaceBetweenPlanets = 10.0f;
	[HideInInspector]
	public float randomSpaceBetweenPlanets = 2.0f;

	//tempo necessario per passare da un pianeta all'altro, seguendo la traiettoria specificata nella classe apposita
	public float timeBetweenPlanets = 3.0f;
	public float randomTimeBetweenPlanets = 1.0f;

	//angolo di salto
	public float angle = 45.0f;
	public float randomAngle = 1.0f;

	//variabili utili per generare pianeti e coins, possono essere poste pubbliche
	float coinSpaceBeforePlanet = 2.0f;
	float coinSpaceAfterPlanet = 3.0f;
	float spaceBetweenCoins = 1.2f;

	TrajectoryController trajectory;

	//crea i pianeti, chiamata da inspector
	public void CreatePlanets()
	{
		//crea un array di planetElements, ognuno conterrà un oggetto pianeta ed una lista di coins
		planetElements = new PlanetElement[planetNumbers];

		for (int i = 0; i < planetNumbers; i++) {
			planetElements[i] = new PlanetElement();
			CreatePlanet(i);
		}
	}

	//cancella l'ENV
	public void DeletePlanets()
	{
		if (planetElements != null && planetElements.Length != 0) {
			for (int i = 0; i < planetElements.Length; i++)
			{
				if (planetElements[i] != null)
				{
					//se il planetElement ha un oggetto pianeta, lo cancella
					if (planetElements[i].planet != null)
						DestroyImmediate(planetElements[i].planet);

					//se il planetElement ha una lista di coins, li cancella
					if (planetElements[i].coins != null)
					{
						for (int j = 0; j < planetElements[i].coins.Count; j++)
						{
							if (planetElements[i].coins[j] != null)
							{
								DestroyImmediate(planetElements[i].coins[j]);
							}
						}

						planetElements[i].coins.Clear();
					}
				}
				
			}
		}
	}

	//crea un pianeta ed i coins
	void CreatePlanet(int index)
	{
		//scelgo la posizione del pianeta, se è il primo, scelgo la beginningPosition
		Vector3 planetPosition = Vector3.zero;
		if (index == 0 && beginningPosition != null) 
		{
			planetPosition = beginningPosition.transform.position;
		} 
		else 
		{
			//trovo il punto di partenza della traiettoria
			//distanza del player dal centro del pianeta, somma tra altezza del player e raggio del pianeta
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

				//posizione del pianeta già presente
				Vector3 oldPlanetPosition = planetElements[index-1].planet.transform.position;

				//posizione di partenza della traiettoria, calcolata a partire dall'angolo di partenza
				Vector3 startingPosition = new Vector3(oldPlanetPosition.x + distance*Mathf.Cos(angleRad), 
				                                       oldPlanetPosition.y + distance*Mathf.Sin(angleRad),
				                                       0.0f);


				//calcolo un punto lungo la traiettoria
				//in questo caso, poiché la funzione è chiamata da inspector, ricerco ogni volta l'oggetto,
				//altrimenti sarebbe stato utile salvarsi un'istanza nello start
				GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
				float velocity = 0.0f;

				if (gameController != null)
					trajectory  = gameController.GetComponent<TrajectoryController>();

				if (trajectory != null)
					velocity = trajectory.velocity;

				//float randomSpace = Random.Range(-randomSpaceBetweenPlanets, randomSpaceBetweenPlanets);
				//float oldDeltaTime = (spaceBetweenPlanets + randomSpace) / velocity;

				//calcolo il tempo necessario per percorrere la traiettoria
				float randomTime = Random.Range(-randomTimeBetweenPlanets, randomTimeBetweenPlanets);
				float deltaTime = timeBetweenPlanets + randomTimeBetweenPlanets;

				//calcolo le posizioni X ed Y, a partire dalle equazioni del moto parabolico e dal tempo appena trovato
				float newXPos = startingPosition.x + velocity*Mathf.Cos(angleRad)* deltaTime;
				float newYPos = startingPosition.y + velocity*Mathf.Sin(angleRad)* deltaTime - 1.0f/2.0f * trajectory.gravity * deltaTime * deltaTime;

				planetPosition = new Vector3(newXPos, newYPos, 0.0f);
				float lastSpace = planetPosition.x - coinSpaceBeforePlanet;

				//chiamo la funzione per generare i coins
				CreateCoins(planetElements [index], startingPosition, spaceBetweenCoins, lastSpace, new Vector3(velocity*Mathf.Cos(angleRad), velocity*Mathf.Sin(angleRad), 0.0f));
			}
		}

		//prendo un prefab casuale tra i pianeti
		int randomIndex = -1;
		while (randomIndex == -1 || planetPrefabs[randomIndex] == null) {
			randomIndex = Random.Range (0, planetPrefabs.Length);
		}
		GameObject planetPrefab = planetPrefabs [randomIndex];

		//istanzio il pianeta
		if (planetElements [index] == null)
			planetElements [index] = new PlanetElement ();
		planetElements [index].planet = (GameObject)Instantiate (planetPrefab, planetPosition, Quaternion.identity);
		if (father != null)
			planetElements [index].planet.transform.SetParent (father.transform);
	}

	//metodo utilizzato per generare i coins tra due pianeti
	void CreateCoins(PlanetElement planetElement, Vector3 startingPosition, float space, float lastX, Vector3 velocity)
	{
		//prima posizione X, considerando anche la soglia
		float beginningX = startingPosition.x + coinSpaceAfterPlanet;
		float actualX = beginningX;

		planetElement.coins = new List<GameObject> ();

		//creo coins finchè non ho raggiunto la massima X
		while (actualX < lastX) {
			float deltaTime = (actualX - startingPosition.x) / velocity.x;

			float newYPos = startingPosition.y + velocity.y * deltaTime - 1.0f/2.0f * trajectory.gravity * deltaTime * deltaTime;

			GameObject tempoCoin = (GameObject)Instantiate(coinPrefab, new Vector3(actualX, newYPos, 0.0f), Quaternion.identity);

			if (father != null)
				tempoCoin.transform.SetParent (father.transform);

			planetElement.coins.Add (tempoCoin);

			//se la posizione Y del coin è la più alta finora raggiunta, aggiorno la struttura
			if (newYPos > planetElement.maxY)
				planetElement.maxY = newYPos;

			actualX += space;
		}
	}

	//restituisce il raggio di un pianeta
	float GetPlanetRadius(GameObject planetOB)
	{
		Collider2D collider = planetOB.GetComponent<Collider2D> ();
		float radius = -1.0f;
		if (collider != null)
			radius = collider.bounds.size.x / 2.0f;

		return radius;
	}

	//restituisce il size del player
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

	//pone il pianeta attuale come attivo
	public void SetActivePlanet(GameObject inputPlanet)
	{
		for (int i = 0; i < planetElements.Length; i++)
		{
			if (planetElements[i] != null)
			{
				if (planetElements[i].planet == inputPlanet)
				{
					planetElements[i].active = true;
				}
				else
					planetElements[i].active = false;
			}
		}
	}

	//restituisce il pianeta attivo
	public int GetActivePlanetIndex()
	{
		for (int i = 0; i < planetElements.Length; i++)
		{
			if (planetElements[i] != null)
			{
				if (planetElements[i].active == true)
				{
					return i;
				}
			}
		}
		return -1;
	}

	//restituisce un pianeta a partire dall'indice nell'array di planetElements
	public GameObject GetPlanetObject(int index)
	{
		if (planetElements != null && planetElements.Length != 0 && planetElements.Length > index
		    && planetElements [index] != null && planetElements [index].planet != null)
			return planetElements [index].planet;
		return null;
	}

	//restituisce la Y massima di un coin
	public float GetMaxY(int index)
	{
		if (planetElements != null && planetElements.Length != 0 && planetElements.Length > index
		    && planetElements [index] != null && planetElements [index].maxY != Mathf.NegativeInfinity)
			return planetElements [index].maxY;
		return 0.0f;
	}

	//restituisce true se sono stati collezionati tutti i coins di un insieme
	public bool VerifyCollectedCoins(GameObject planet)
	{
		PlanetElement element = GetPlanetElement (planet);
		if (element != null &&  element.coins != null && element.coins.Count != 0)
		{
			for (int i = 0; i < element.coins.Count; i++)
			{
				Coin coinScript = element.coins[i].GetComponent<Coin>();
				if (coinScript != null && !coinScript.taken)
					return false;
			}

			return true;
		}
		return false;
	}

	//restituisce il planetElement a partire dall'oggetto pianeta
	public PlanetElement GetPlanetElement(GameObject planet)
	{
		if (planetElements != null) {
			for (int i = 0; i < planetElements.Length; i++)
			{
				if (planetElements[i] != null && planetElements[i].planet != null && planetElements[i].planet == planet)
					return planetElements[i];
			}
		}
		return null;
	}
}
