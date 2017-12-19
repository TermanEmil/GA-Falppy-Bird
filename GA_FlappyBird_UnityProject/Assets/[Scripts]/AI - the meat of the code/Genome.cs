using UnityEngine;
using System.Linq;

[System.Serializable]
public class Genome
{
	// All layers, except the output layer, has a bias neuron, which always has the value of 1.

	private Neuron[] _inputLayer;
	public Neuron[] InputLayer { get { return _inputLayer; } }

	private Neuron[] _hiddenLayer;
	public Neuron[] HiddenLayer { get { return _hiddenLayer; } }

	public float Fitness { get; set; } = 0;

	// Number of outputs
	private int _outputCount;

	/// <summary>
	/// Creates the input and hidden layer. Each layer has an additional Bias neuron.
	/// The weightRandomValue is used to randomly initialize the network.
	/// </summary>
	public Genome (int inputsNeuronsC, int hiddenNeuronsC, int outputsC, float weightRandomValue = 0)
	{
		if (inputsNeuronsC <= 0 || hiddenNeuronsC <= 0 || outputsC <= 0)
			throw new System.Exception("Invalid parameters");

		_inputLayer = Enumerable.Range(0, inputsNeuronsC + 1).Select(x => new Neuron(hiddenNeuronsC + 1)).ToArray();
		_inputLayer[_inputLayer.Length - 1].IsBias = true;

		_outputCount = outputsC;
		_hiddenLayer = Enumerable.Range(0, hiddenNeuronsC + 1).Select(x => new Neuron(_outputCount)).ToArray();
		_hiddenLayer[_hiddenLayer.Length - 1].IsBias = true;

		if (weightRandomValue != 0)
			GenerateRandomGenes(weightRandomValue);
	}

	public Genome(Neuron[] inLayer, Neuron[] hidLayer)
	{
		_inputLayer = inLayer;
		_hiddenLayer = hidLayer;
		_outputCount = _hiddenLayer[0].Weights.Length;
	}

	/// <summary>
	/// Copy contructor. Output count is equal to the number of weights of any neuron
	/// from the hiddenlayer.
	/// </summary>
	public Genome (Genome other)
	{
		_inputLayer = other.InputLayer.Select(x => new Neuron(x)).ToArray();
		_hiddenLayer = other.HiddenLayer.Select(x => new Neuron(x)).ToArray();
		_outputCount = _hiddenLayer[0].Weights.Length;
	}

	/// <summary>
	/// The format is: in {v1 v2 v3... wn} hidden: {v1 v2... vn}
	/// Representing each node's values
	/// </summary>
	public override string ToString()
	{
		return
			string.Join("in {", _inputLayer.Select(x => x.Val.ToString()).ToArray()) + "} " +
			string.Join("hidden: {", _hiddenLayer.Select(x => x.Val.ToString()).ToArray()) + "}";
	}

	/// <summary>
	/// Activates the brain, giving an array of _outputCount length
	/// First, this function copies the inputValues in the input layer.
	/// Then it calculates the values of each node for the hidden layer.
	/// Finally, it computes the final output.
	/// </summary>
	/// <returns>An array with the network's outputs.</returns>
	public float[] FeedForward (float[] inputValues)
	{
		int i;

		for (i = 0; i < inputValues.Length; i++)
			_inputLayer[i].Val = inputValues[i];

		i = 0;
		foreach (var node in _hiddenLayer)
			node.Val = _FeedNeuronalData(_inputLayer, i++);

		return Enumerable.Range(0, _outputCount)
						 .Select(x => _FeedNeuronalData(_hiddenLayer, x))
						 .ToArray();
	}

	/// <summary>
	/// Randomly initializes each weight in the network with
	/// values between -maxWeight and maxWeight
	/// </summary>
	public void GenerateRandomGenes (float maxWeight)
	{
		foreach (var neuron in _inputLayer.Concat(_hiddenLayer))
			neuron.RandomWeights(maxWeight);
	}

	/// <summary>
	/// Calculate the value of a neuron given the previous layer and the targeted layer id.
	/// The activation function used is Sigmoid.
	/// </summary>
	private float _FeedNeuronalData(Neuron[] inputs, int neuronId)
	{
		return Sigmoid(inputs.Sum(neuron => neuron.Val * neuron.Weights[neuronId]));
	}

	/// <summary>
	/// Activation function used to squeeze the result between 0 and 1
	/// for any given value.
	/// </summary>
	private static float Sigmoid(float x)
	{
		return 1.0f / (1 + Mathf.Exp(-x));
	}
}
