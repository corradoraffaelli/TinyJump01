using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(WorldCreator))]
public class WorldCreatorEditor : Editor {
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector ();

		WorldCreator myScript = (WorldCreator)target;
		if(GUILayout.Button("Build Object"))
		{
			myScript.DeletePlanets();

			myScript.CreatePlanets();

		}
	}

}
