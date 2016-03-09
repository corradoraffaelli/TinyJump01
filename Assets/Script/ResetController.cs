using UnityEngine;
using System.Collections;

public class ResetController : MonoBehaviour {

	WorldCreator worldCreator;
	PointsController pointsController;
	PlayerMovements playerMovements;

	void Start () {
		GameObject controller = GameObject.FindGameObjectWithTag ("GameController");
		if (controller != null) {
			worldCreator = controller.GetComponent<WorldCreator>();
			pointsController = controller.GetComponent<PointsController>();
		}

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null) {
			playerMovements = player.GetComponent<PlayerMovements>();
		}
	
	}

	public void ResetScene()
	{
		if (worldCreator != null) {
			worldCreator.ResetCoins();
		}

		if (pointsController != null) {
			pointsController.ResetMultiplier();
			pointsController.ResetPoints();
		}
	}
}
