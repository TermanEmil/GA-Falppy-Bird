using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour {
	public Transform botColumn = null;
	public Transform topColumn = null;

	[HideInInspector] public float distBetween = 0;

	public BoxCollider2D center = null;

	public void ApplyDistBetweenColumns () {
		botColumn.localPosition = new Vector2 (0, -distBetween / 2);
		topColumn.localPosition = new Vector2 (0, distBetween / 2);
		center.size = new Vector2 (center.size.x, distBetween);
	}

	public bool IsOutOfGame (float endPointX) {
		return !gameObject.activeSelf || transform.position.x - endPointX <= 0;
	}
}
