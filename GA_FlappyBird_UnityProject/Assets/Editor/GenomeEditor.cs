using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BirdGenome))]
public class GenomeEditor : Editor
{
	private BirdGenome myTarget;

	void Awake () {
		myTarget = (BirdGenome)target;
	}

	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		EditorGUILayout.LabelField ("Flaps", myTarget.Flaps.ToString ());
		EditorGUILayout.LabelField ("Blocks", myTarget.Blocks.ToString ());
		EditorGUILayout.LabelField ("Is dead", myTarget.IsDead.ToString ());
		EditorGUILayout.LabelField ("Fitness", myTarget.Fitness.ToString ());
		//EditorGUILayout.LabelField("Dist to ground", BirdGenome.YCoordGenome(myTarget).ToString());
	}
}
