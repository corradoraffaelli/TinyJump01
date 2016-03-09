using UnityEngine;
using System.Collections;

public class DeathObjectMovements : MonoBehaviour {

	WorldCreator worldCreator;

	public float distance = 5.0f;

	int oldIndex = -1;

	void Start()
	{
		GameObject controller = GameObject.FindGameObjectWithTag ("GameController");
		if (controller != null)
			worldCreator = controller.GetComponent<WorldCreator> ();
	}

	void Update () {
		if (worldCreator != null) {
			int index = worldCreator.GetActivePlanetIndex();

			if (index != oldIndex)
			{
				Vector3 planetPosition = worldCreator.GetActivePlanet().transform.position;

				Vector3 deathPosition = new Vector3(planetPosition.x, planetPosition.y - distance, planetPosition.z);

				transform.position = deathPosition;
			}

			oldIndex = index;
		}

	}
}
