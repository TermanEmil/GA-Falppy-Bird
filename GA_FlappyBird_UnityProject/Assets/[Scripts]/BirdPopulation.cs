using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class BirdPopulation : MonoBehaviour
{
	public static BirdPopulation instance;

	private BirdGenome[] _birdGenomes;
	private Population _population;
	public Genome[] Genomes { get { return _population.Genomes; } }

	public int GenerationCount { get { return (_population == null) ? 0 : _population.GenerationCount; } }

	[SerializeField] private GameObject _genomePrefab = null;
	[SerializeField] private float _mutationChance = 0.05f;
	[SerializeField] private float _mutationSize = 0.02f;
	[SerializeField] private float _randomGenomeWeightsVals = 6;
	public float RandomGenomeWeightsVals { get { return _randomGenomeWeightsVals; } }

	// The mutation value will be maxVal * _mutationSize
	private float _mutationVal { get { return RandomGenomeWeightsVals * _mutationSize; } }

	[SerializeField] private int _genomeCount = 50;

	[SerializeField] private int _hiddenNeurons = 6;
	public int hiddenNeurons { get { return _hiddenNeurons; } }

	[SerializeField] private float _flapForce = 5;
	public float flapForce { get { return _flapForce; } }

	// If the maximum fitness from the population is less than
	// this value, the population is reset.
	[SerializeField] private float _minPopulFitness = 10f;

	// Holds the reference of the column that the population is currently facing.
	// Each bird faces the same column.
	private Column _facingColumn = null;
	public Column FacingColumn
	{
		get
		{
			if (_facingColumn == null)
				return (_facingColumn = ColumnControl.instance.GetColumnFacingObj(transform));
			else
				return _facingColumn;
		}
	}

	[SerializeField] private UnityEvent _onGenomeDeath = null;
	[SerializeField] private UnityEvent _onPopulationDeath = null;

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		_birdGenomes = new BirdGenome[_genomeCount];
		_InitPopulation();
	}

	void FixedUpdate()
	{
		_facingColumn = ColumnControl.instance.GetColumnFacingObj(transform);
	}

	public bool EveryoneIsDead ()
	{
		return _birdGenomes.FirstOrDefault(x => !x.IsDead) == null;
	}

	public int AlliveCount ()
	{
		return _birdGenomes.Sum(x => (x.IsDead) ? 0 : 1);
	}

	public void OnGenomeDeath ()
	{
		_onGenomeDeath.Invoke();
		if (EveryoneIsDead ())
			OnPopulationDeath ();
	}

	public void OnPopulationDeath ()
	{
		_Evolve();
		GameControl.instance.ResetAll();
		_onPopulationDeath.Invoke();

		foreach (var bird in _birdGenomes)
			bird.Init();
	}

	public void ReinitPopl(bool keepOldGenerationCount = false)
	{
		_population.Reinit(RandomGenomeWeightsVals, keepOldGenerationCount);

		foreach (var genome in _birdGenomes)
			genome.Init();
	}

	public void KillAll()
	{
		foreach (var genome in _birdGenomes)
			genome.Die();
	}

	private void _InitPopulation()
	{
		_population = new Population(new Genome[_genomeCount], _mutationChance, _mutationVal);

		for (int i = 0; i < _genomeCount; i++)
		{
			BirdGenome newOrganizm = Instantiate(_genomePrefab, transform).GetComponent<BirdGenome>();
			newOrganizm.transform.position = transform.position;
			newOrganizm.Id = i;
			_birdGenomes[i] = newOrganizm;

			_population.Genomes[i] = new Genome(
				inputsNeuronsC: BirdGenome.inputs.Count,
				hiddenNeuronsC: hiddenNeurons,
				outputsC: 1,
				weightRandomValue: RandomGenomeWeightsVals
			);
		}
	}

	private void _Evolve()
	{
		// If biggest fitness from the population is less than required, reinit population.
		if (_birdGenomes.Max(x => x.Fitness) < _minPopulFitness)
		{
			ReinitPopl(false);
			Debug.Log("Reinitializing population");
		}
		else
			_population.Evolve();
	}
}
