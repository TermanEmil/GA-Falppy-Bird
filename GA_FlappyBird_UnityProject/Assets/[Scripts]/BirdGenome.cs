using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BirdGenome : MonoBehaviour
{
	// A neural network inputs.Length inputs, one hidden layer and only one output.
	private Genome _genome { get { return BirdPopulation.instance.Genomes[Id]; } }
	public int Id { get; set; }

	private Animator _animator;
	private Rigidbody2D _rb;
	private float _startTime;
	[SerializeField] private UnityEngine.UI.Text _fitnessLabel = null;

	public bool IsDead { get { return !gameObject.activeSelf; } }

	private int _flaps;
	public int Flaps { get { return _flaps; } }

	private int _blocks = 0;
	public int Blocks { get { return _blocks; } }

	public float Fitness { get { return (!IsDead) ? _Fitness() : _genome.Fitness; } }

	// Aliases to some global data.
	private static Column _facingCol { get { return BirdPopulation.instance.FacingColumn; } }
	private static float _groundPointY { get { return GameControl.instance.groundPoint.position.y; } }
	private static float _skyPointY { get { return GameControl.instance.skyPoint.position.y; } }

	public static readonly List<Func<BirdGenome, float>> inputs = new List<Func<BirdGenome, float>>(2) {
		XDistToNextColumn,
		YDistToNextColumn,
	};

	void Start () {
		_animator = GetComponent<Animator> ();
		_rb = GetComponent<Rigidbody2D> ();

		Init();
	}

	private void FixedUpdate()
	{
		if (IsDead)
			return;

		if (!GameControl.instance.learningMode)
		{
			// User input click
			if (Input.GetMouseButtonDown(0))
				_Flap();
		}
		else
		{
			// AI
			if (_DoIFlap())
				_Flap();
		}

		if (_fitnessLabel != null)
			_fitnessLabel.text = Fitness.ToString();
	}

	void OnTriggerEnter2D (Collider2D other) {
		// Increment the number of blocks (columns) passed.
		if (other.CompareTag ("ScoreZone"))
			_blocks++;
	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.layer != LayerMask.NameToLayer("Deadly"))
			return;

		Die();
	}

	public void Init()
	{
		_flaps = 0;
		_blocks = 0;
		_startTime = Time.time;
		_rb.velocity = Vector2.zero;
		transform.localPosition = Vector2.up * UnityEngine.Random.Range(_groundPointY + 1, _skyPointY - 1f);

		gameObject.SetActive(true);
	}

	public void Die()
	{
		if (IsDead)
			return;

		_genome.Fitness = _Fitness();
		gameObject.SetActive(false);
		BirdPopulation.instance.OnGenomeDeath();
	}

	public float _Fitness()
	{
		float distToCenterOfNextCol;

		// Prevent calculating the fitness when the application is not playing.
		// Thif function can be called from an Editor Script.
		if (!Application.isPlaying)
			return 0;

		if (_facingCol == null)
			distToCenterOfNextCol = Mathf.Infinity;
		else
			distToCenterOfNextCol = Mathf.Abs(_facingCol.center.transform.position.y - transform.position.y);
		return _TraveledDistance() - distToCenterOfNextCol * 5 + Blocks * 50;
	}

	private bool _DoIFlap()
	{
		return _genome.FeedForward(GetNormalizedInputs(this))[0] > 0.5f;
	}

	private void _Flap () {
		if (IsDead)
			return;

		_animator.SetTrigger ("Flap");
		_rb.velocity = Vector2.up * BirdPopulation.instance.flapForce;
		_flaps++;
	}

	/// <summary>
	/// Traveled distance is calculated by coputing the speed * deltaTime.
	/// </summary>
	private float _TraveledDistance ()
	{
		return (IsDead) ? -1 : GameControl.instance.scrollSpeed * (Time.time - _startTime);
	}

	#region InputFunctions
	private static float XDistToNextColumn (BirdGenome genome) {
		if (_facingCol == null)
			return 0;
		return (_facingCol.center.transform.position.x - genome.transform.position.x) / ColumnControl.instance.xColMaxDelta;
	}

	/// <summary>
	/// The y difference between this object and the center of the facingCenter can
	/// be negative.To normalize this value, a maxDelta is added.
	/// </summary>
	private static float YDistToNextColumn (BirdGenome genome) {
		float deltaY, maxDelta;

		if (_facingCol == null)
			return 0;

		maxDelta = ColumnControl.instance.yColMaxDelta;
		deltaY = _facingCol.center.transform.position.y - genome.transform.position.y;

		return (deltaY + maxDelta) / (2 * maxDelta);
	}

	/// <summary>
	/// Creates an array of inputs (float values) between 0 and 1.
	/// It actualy works even if the input values aren't normalized, but it
	/// converges faster when the inputs are normalized.
	/// </summary>
	private static float[] GetNormalizedInputs (BirdGenome genome)
	{
		return inputs.Select(f => f(genome)).ToArray();
	}
	#endregion
}
