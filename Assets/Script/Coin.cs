using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	SpriteRenderer renderer;
	public bool taken;

	void Start()
	{
		renderer = GetComponent<SpriteRenderer> ();
	}

	//chiamato nel momento in cui si raccoglie un coin. Lo nasconde e setta il bool taken a true
	public void Hide()
	{
		taken = true;

		if (renderer != null) {
			Color oldColor = renderer.color;
			Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.0f);
			renderer.color = newColor;
		}
	}
}
