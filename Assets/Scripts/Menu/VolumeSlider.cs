using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
	public GameSettings.VolumeType volumeType;
	public float volume;
	Slider slider;

	private void Awake()
	{
		slider = GetComponent<Slider>();
	}

	private void Start()
	{
		// Set loaded value
		volume = slider.value = FindObjectOfType<GameSettings>().GetVolume(volumeType);
		slider.enabled = false;
	}

	// On slider value change
	public void UpdateVolume(float newVolume)
	{
		volume = newVolume;
		FindObjectOfType<GameSettings>().SetVolume(this);
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
