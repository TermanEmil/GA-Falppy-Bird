using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour {
	public static GameControl instance;

	[SerializeField] private bool _learningMode = false;
	public bool learningMode {get{ return _learningMode; }}

	[SerializeField] private float _scrollSpeed = 1.5f;
	public float scrollSpeed {get{ return _scrollSpeed; }}

	[SerializeField] private Transform _groundPoint = null;
	public Transform groundPoint { get { return _groundPoint; } }

	[SerializeField] private Transform _skyPoint = null;
	public Transform skyPoint { get { return _skyPoint; } }

	void Awake () {
		if (instance == null)
			instance = this;
		else {
			Destroy (gameObject);
			return;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ResetAll();
			BirdPopulation.instance.ReinitPopl();
		}
	}

	public void ResetAll()
	{
		ColumnControl.instance.RemoveAll();
		foreach (var obj in FindObjectsOfType<ScrollerGround>())
			obj.ResetPos();
	}
}
