using System.Linq;
using UnityEngine;

[System.Serializable]
public class Neuron
{
	// A bias neuron always has the value of 1.
	private float _value;
	public float Val {
		get { return (IsBias) ? 1 : _value; }
		set { _value = value; }
	}

	private float[] _weights;
	public float[] Weights { get { return _weights; } }

	public bool IsBias { get; set; }

	public Neuron(int weightsCount)
	{
		_weights = Enumerable.Repeat(0f, weightsCount).ToArray();
	}

	public Neuron(Neuron other)
	{
		Val = other.Val;
		IsBias = other.IsBias;
		_weights = other.Weights.Select(x => x).ToArray();
	}

	public override string ToString()
	{
		return Val + string.Join("; w {", Weights.Select(x => x.ToString()).ToArray()) + "}";
	}

	/// <summary>
	/// Initialize the weights with a random value between -maxWeight and maxWeights
	/// </summary>
	public void RandomWeights (float maxWeight)
	{
		for (int i = 0; i < _weights.Length; i++)
			_weights[i] = Random.Range(-maxWeight, maxWeight);
	}

	/// <summary>
	/// Mutates each weight with the given mutation value.
	/// </summary>
	public void MutateWeights (float mutationValue)
	{
		for (int i = 0; i < Weights.Length; i++)
			MutateWeight(mutationValue, i);
	}

	/// <summary>
	/// Mutates the weight[i] by adding a random value between +/-mutationValue
	/// </summary>
	public void MutateWeight (float mutationValue, int i)
	{
		Weights[i] += Random.Range(-mutationValue, mutationValue);
	}
}
