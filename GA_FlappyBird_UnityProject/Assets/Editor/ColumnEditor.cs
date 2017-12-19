using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Column))]
public class ColumnEditor : Editor
{
	private Column colTarget;

	void Awake ()
	{
		colTarget = (Column)target;
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		colTarget.distBetween = EditorGUILayout.Slider ("Dist", colTarget.distBetween, 0, 10);
		colTarget.ApplyDistBetweenColumns ();
	}
}
