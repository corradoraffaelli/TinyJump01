using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PointsController : MonoBehaviour {

	public Text textPoints;
	public Text textMultiplier;

	int points = 0;
	public int pointsAdded = 10;
	int multiplier = 1;

	bool pointsChanged = true;
	bool multiplierChanged = true;

	void Update () {
		ManagePointsChanged ();
		ManageMultiplierChanged ();
	}

	public void AddPoint()
	{
		points += pointsAdded * multiplier;
		pointsChanged = true;
	}

	public void AddMultiplier()
	{
		multiplier++;
		multiplierChanged = true;
	}

	public void ResetMultiplier()
	{
		multiplier = 1;
		multiplierChanged = true;
	}

	public void ResetPoints()
	{
		points = 0;
		pointsChanged = true;
	}

	void ManagePointsChanged()
	{
		if (pointsChanged) {
			pointsChanged = false;
			if (textPoints != null)
				textPoints.text = points.ToString();
		}
	}

	void ManageMultiplierChanged()
	{
		if (multiplierChanged) {
			multiplierChanged = false;
			if (textMultiplier != null)
				textMultiplier.text = "x"+multiplier.ToString();
		}
	}
}
