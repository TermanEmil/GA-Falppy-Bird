using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class Population
{
	private Genome[] _genomes = null;
	public Genome[] Genomes { get { return _genomes; } }

	public float MutationChance { get; set; } = 0;
	public float MutationValue { get; set; } = 0;

	private int _generationCount;
	public int GenerationCount { get { return _generationCount; } }

	public Population(Genome[] startingPopulation, float mutChance, float mutValue)
	{
		if (startingPopulation.Length < 4)
			throw new Exception("Can't start from a population with less than 4 genomes");

		_genomes = startingPopulation;
		_generationCount = 0;

		MutationChance = mutChance;
		MutationValue = mutValue;
	}

	/// <summary>
	/// Creates the next generation by mating the best half genomes between them
	/// and mutating the offsprings. The new generation consits of the best
	/// half and the new mutated offsprings. Only the offsprings are mutated.
	/// The bigger the fitness - the bigger is the chance to mate. Even though,
	/// even the best genome has the chance to not mutate.
	/// </summary>
	public void Evolve()
	{
		Genome[] offsprings;
		Genome[] sortedBestHalf;

		sortedBestHalf = _genomes.OrderByDescending(x => x.Fitness)
								 .Take(_genomes.Length / 2)
								 .Reverse()
								 .ToArray();

		offsprings = _Crossover(sortedBestHalf, _GetRandomGenomeByFitness);

		foreach (var offspring in offsprings)
			_Mutate(offspring, true);

		_genomes = sortedBestHalf.Concat(offsprings)
								 .ToArray();

		_generationCount++;
	}

	/// <summary>
	/// Makes the weights random, meaning that the genomes forget what
	/// they've learned so far.
	/// </summary>
	public void Reinit(float maxWeight, bool keepOldGenerationCount = false)
	{
		if (!keepOldGenerationCount)
			_generationCount = 0;

		for (int i = 0; i < Genomes.Length; i++)
			_genomes[i].GenerateRandomGenes(maxWeight);
	}

	/// <summary>
	/// Each genome from the given genomes are mated with someone else,
	/// randomly picked by selectF function, avoiding the mating of the same
	/// genome with itself. An array of offsprings (children) is formed.
	/// </summary>
	private Genome[] _Crossover(Genome[] genomes, Func<Genome[], Genome[], Genome> selectF)
	{
		int genomesLen = genomes.Length;
		Genome[] offsprings = new Genome[genomesLen];
		Genome parent1, parent2;

		for (int i = 0; i < genomesLen; i++)
		{
			parent1 = selectF(genomes, null);
			parent2 = selectF(genomes, new Genome[1] { parent1 });
			offsprings[i] = _SinglePointCrossover(parent1, parent2);
		}

		return offsprings;
	}

	/// <summary>
	/// This function can mutate the given genome either by weights or by neurons.
	/// It means that the mutationChance is applied either to each neuron or to each weight.
	/// Basically, it iterates throuth each neuron/weight and adds to the weights a value
	/// between -mutationValue and mutationValue.
	/// </summary>
	private void _Mutate(Genome genome, bool byWeights)
	{
		foreach (var neuron in genome.InputLayer.Concat(genome.HiddenLayer).ToArray())
			if (byWeights)
			{
				for (int i = 0; i < neuron.Weights.Length; i++)
					if (UnityEngine.Random.Range(0f, 1f) < MutationChance)
						neuron.MutateWeight(MutationValue, i);
			}
			else
			{
				if (UnityEngine.Random.Range(0f, 1f) < MutationChance)
					neuron.MutateWeights(MutationValue);
			}
	}

	/// <summary>
	/// Helper function for _Crossover.
	/// A weighted random select. The bigger the fitness, the more likely to be choosen.
	/// The absMinFitness is added everytime, because there may be negative fitnesses,
	/// and it would be easier to keep everything positive :)
	/// </summary>
	private static Genome _GetRandomGenomeByFitness(Genome[] genomesIn, Genome[] exceptionGenomes = null)
	{
		Genome[] container;
		float absMinFitness;
		float totalFitness;
		float randomVal;

		if (exceptionGenomes == null)
			container = genomesIn;
		else
			container = genomesIn.Where(x => !exceptionGenomes.Contains(x))
								 .ToArray();

		if (container == null)
			throw new Exception("Can't have an empty container");

		absMinFitness = Mathf.Abs(container.Min(x => x.Fitness));
		totalFitness = container.Sum(x => x.Fitness + absMinFitness);

		randomVal = UnityEngine.Random.Range(0f, totalFitness);

		foreach (var x in container)
		{
			if (randomVal <= x.Fitness + absMinFitness)
				return x;
			randomVal -= x.Fitness + absMinFitness;
		}

		throw new Exception("Failed to pick a genome.");
	}

	/// <summary>
	/// Helper function for _Crossover
	/// Mates two parents, creating a new child.
	/// Crossovers the input and hidden layer from both parents.
	/// </summary>
	private Genome _SinglePointCrossover(Genome parent1, Genome parent2)
	{
		int crossPointInputL, crossPointHiddenL;
		Neuron[] _inputLayer, _hidLayer;

		crossPointInputL = UnityEngine.Random.Range(0, parent1.InputLayer.Length);
		crossPointHiddenL = UnityEngine.Random.Range(0, parent1.HiddenLayer.Length);

		_inputLayer = _CrossoverNeurons(parent1.InputLayer, parent2.InputLayer, crossPointInputL);
		_hidLayer = _CrossoverNeurons(parent1.HiddenLayer, parent2.HiddenLayer, crossPointHiddenL);
		return new Genome(_inputLayer, _hidLayer);
	}

	/// <summary>
	/// Helper for _SinglePointCrossover
	/// Takes as an input some neurons and a cross point and combines them
	/// taking the first crossPoint + 1 neurons from the parent1 and the other neurons
	/// from parent2 for result1 and the same thing, but in reverse, for result2.
	/// One of these results is randomly picked as a final result.
	/// </summary>
	private Neuron[] _CrossoverNeurons(Neuron[] parent1, Neuron[] parent2, int crossPoint)
	{
		Neuron[] crossResult1 = new Neuron[parent1.Length];
		Neuron[] crossResult2 = new Neuron[parent1.Length];

		for (int i = 0; i < parent1.Length; i++)
			if (i <= crossPoint)
			{
				crossResult1[i] = new Neuron(parent1[i]);
				crossResult2[i] = new Neuron(parent2[i]);
			}
			else
			{
				crossResult1[i] = new Neuron(parent2[i]);
				crossResult2[i] = new Neuron(parent1[i]);
			}
		return (UnityEngine.Random.Range(0, 2) == 0) ? crossResult1 : crossResult2;
	}
}
