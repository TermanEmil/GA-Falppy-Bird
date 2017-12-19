using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIControl : MonoBehaviour {
	[SerializeField] private Text _generationField = null;
	[SerializeField] private Text _aliveField = null;

	[SerializeField] private Text _timeScaleTxt = null;

	private void Start()
	{
		UpdateGenerationText();
		_timeScaleTxt.text = Time.timeScale.ToString("F2");
	}

	public void UpdateGenerationText()
	{
		_generationField.text = "Gen: " + BirdPopulation.instance.GenerationCount;
	}

	public void UpdateAliveCount(){
		_aliveField.text = "Alive: <color=red>" + BirdPopulation.instance.AlliveCount() + "</color>";
	}

	public void KillAllBtn ()
	{
		BirdPopulation.instance.KillAll();
	}

	public void ReinitBtn ()
	{
		GameControl.instance.ResetAll();
		BirdPopulation.instance.ReinitPopl();
	}

	public void TimeScaleBtn (float delta)
	{
		Time.timeScale = Mathf.Max(0, Time.timeScale + delta);
		_timeScaleTxt.text = Time.timeScale.ToString("F2");
	}

	public void ToggleActive (GameObject obj)
	{
		obj.SetActive(!obj.activeSelf);
	}
}
