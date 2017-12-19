using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerObject : MonoBehaviour {
	[HideInInspector] public Vector3 startPos;
	[SerializeField] private float _scrollMult = 1;

	void Start () {
		startPos = transform.localPosition;
	}

	void FixedUpdate () {
		transform.Translate (Vector2.left * GameControl.instance.scrollSpeed * _scrollMult * Time.fixedDeltaTime);
	}

	public float DistScrolled () {
		return (transform.localPosition - startPos).magnitude;
	}

	public void ResetPos () {
		transform.localPosition = startPos;
	}
}
