using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class is only used to obtain easy access to UI elements
public class UIComponents : MonoBehaviour
{
	public Text levelNameText;
	public Transform timer;
	public Text comboCounter;
	public Text scorePercentage;
	public RingManager ringManager;
	public RectTransform healthPointsBar;
	public RectTransform timerBar;

	public Text LevelNameText { get { return levelNameText; } }
	public Transform Timer { get { return timer; } }
	public Text ComboCounter { get { return comboCounter; } }
	public Text ScorePercentage { get { return scorePercentage; } }
	public RingManager RingManager { get { return ringManager; } }
	public RectTransform HealthPointsBar { get { return healthPointsBar; } }
	public RectTransform TimerBar { get { return timerBar; } }
}
