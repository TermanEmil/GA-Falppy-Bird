using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerGround : MonoBehaviour {
	[SerializeField] private Collider2D[] _blocks = new Collider2D[2];

	private ScrollerObject[] _blocksScrollerComp;
	private int _currentIndex = 0;

	void Start () {
		_blocksScrollerComp = new ScrollerObject[_blocks.Length];
		for (int i = 0; i < _blocks.Length; i++)
			_blocksScrollerComp [i] = _blocks [i].GetComponent<ScrollerObject> ();
		UpdatePos ();
	}

	void FixedUpdate () {
		if (_blocksScrollerComp [_currentIndex].DistScrolled () > 1.8f * _blocks [_currentIndex].bounds.extents.x) {
			_currentIndex = _NextIndex ();
			_blocksScrollerComp[_currentIndex].startPos = _blocksScrollerComp[_currentIndex].transform.position;
		}

		UpdatePos ();
			
	}

	public void ResetPos()
	{
		foreach (var obj in _blocksScrollerComp)
			obj.ResetPos();
	}

	private void UpdatePos() {
		float secondBlockXPos = _blocks [_currentIndex].transform.position.x + 2 * _blocks[_currentIndex].bounds.extents.x;
		_blocks[_NextIndex()].transform.position = new Vector2(secondBlockXPos, _blocks[_currentIndex].transform.position.y);
	}

	private int _NextIndex() {
		return (_currentIndex == 0) ? 1 : 0;
	}
}
