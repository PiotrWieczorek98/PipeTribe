using UnityEngine;
using UnityEngine.UI;

public class DifficultySlider : MonoBehaviour
{
	public GameSettings.DifficultyType difficultyType;
	public float value;
	Slider slider;

	private void Awake()
	{
		slider = GetComponent<Slider>();
	}

	private void Start()
	{
		// Set loaded value
		value = slider.value = FindObjectOfType<GameSettings>().GetDifficultyValue(difficultyType);
		slider.enabled = false;
	}

	// On slider value change
	public void UpdateValue(float newValue)
	{
		value = newValue;
		FindObjectOfType<GameSettings>().SetDifficultyValue(this);
	}
	void OnMouseEnter()
	{
		slider.enabled = true;
	}
	private void OnMouseExit()
	{
		slider.enabled = false;
	}
}
