using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColumnControl : MonoBehaviour {
	public static ColumnControl instance;

	[SerializeField] private Transform _spawnPoint = null;
	[SerializeField] private Transform _endPoint = null;
	[SerializeField] private GameObject _columnPrefab = null;
	[SerializeField] private Transform _columsStorage = null;

	[SerializeField] private Transform _minColYPoint = null;
	[SerializeField] private Transform _maxColYPoint = null;

	[SerializeField] private float _distBetweenCols = 2f;

	[HideInInspector] public List<Column> columns = new List<Column> ();

	public float xColMaxDelta { get { return _spawnPoint.position.x - BirdPopulation.instance.transform.position.x; } }
	public float yColMaxDelta { get { return _maxColYPoint.position.y - _minColYPoint.position.y; } }

	private void Awake()
	{
		instance = this;
	}

	void Start () {
		foreach (Transform child in _columsStorage)
			Destroy (child.gameObject);
	}

	void Update () {
		_UpdateColums ();
	}

	//Get the column that's currently facing the target
	public Column GetColumnFacingObj (Transform target)
	{
		Column result = null;
		float min = Mathf.Infinity;
		float x;

		foreach (var col in columns)
		{
			x = col.center.transform.position.x;
			if (col.gameObject.activeSelf && x > target.position.x && x - target.position.x < min)
			{
				min = x - target.position.x;
				result = col;
			}
		}
		return result;
	}

	public void RemoveAll()
	{
		foreach (var column in columns)
			_ResetColumn(column);
	}

	private void _UpdateColums () {
		Column rightMost = _RightMostColumn ();

		if (rightMost == null || _DistToSpawn (rightMost) >= _distBetweenCols) {
			Column freeColumn = _GetFreeColumn ();

			freeColumn.gameObject.SetActive(true);

			freeColumn.transform.position = new Vector2 (
				x: _spawnPoint.position.x,
				y: Random.Range (_minColYPoint.position.y, _maxColYPoint.position.y)
			);
		}
	}

	private Column _GetFreeColumn () {
		Column result = columns.FirstOrDefault ((column) => column.IsOutOfGame(_endPoint.position.x));

		if (result == null) {
			result = Instantiate (_columnPrefab, _columsStorage).GetComponent<Column> ();
			columns.Add (result);
		}

		return result;
	}

	private Column _RightMostColumn () {
		if (columns.Count == 0)
			return null;

		return columns.Aggregate((curMin, x) =>
		{
			if (!x.gameObject.activeSelf)
				return curMin;
			if (curMin == null)
				return x;
			return (_DistToSpawn(x) < _DistToSpawn(curMin)) ? x : curMin;
		});
	}

	private float _DistToSpawn (Column col) {
		return _spawnPoint.position.x - col.transform.position.x;
	}

	private void _ResetColumn (Column col)
	{
		col.gameObject.SetActive(false);
		col.transform.position = _spawnPoint.position + Vector3.left * 1000;
	}
}
