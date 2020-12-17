using UnityEngine;
using UnityEngine.UI;

public class ComboEffects : MonoBehaviour
{
	public Text combo;
	public Image display;
	public Sprite effect50;
	public Sprite effect100;
	public Sprite effect500;
	public AnimationClip comboEffect;

	string lastValue = "0x";
	Animator animator;

	private void Awake()
	{
		animator = display.transform.parent.GetComponent<Animator>();
	}
	private void Start()
	{
		if (!FindObjectOfType<GameSettings>().ComboEffects)
			GetComponent<ComboEffects>().enabled = false;
	}

	private void Update()
	{
		if (combo.text != lastValue)
		{
			lastValue = combo.text;
			int value = int.Parse(lastValue.Remove(lastValue.Length - 1));
			switch (value)
			{
				case 50:
					display.sprite = effect50;
					animator.Play(comboEffect.name);
					break;
				case 100:
					display.sprite = effect100;
					animator.Play(comboEffect.name);
					break;
				case 500:
					display.sprite = effect500;
					animator.Play(comboEffect.name);
					break;
				default:
					// Show 500 animation every 500 combo
					if (value >= 1000 && value % 500 == 0)
					{
						display.sprite = effect500;
						animator.Play(comboEffect.name);
					}
					break;
			}

		}
	}
}
