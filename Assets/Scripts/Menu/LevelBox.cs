using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBox : MonoBehaviour
{
	public enum BackgroundColor { Red, Green, Orange, Yellow, Purple, Blue };
	public Text title;
	public Text combo;
	public Text score;

	public void InitializeBox(string songName, BackgroundColor boxColor)
	{
		title.text = songName;

		// Get song data
		string fileLocation = CrossSceneData.ScoresDir + "/" + songName;
		List<(float, float)> scores = FindObjectOfType<LevelDataPasser>().LoadLevelDataFromDat(fileLocation);

		if (scores == null)
		{
			score.text = "Not Played";
			combo.text = "Not Played";
		}
		else
		{
			// Sort to get highest total score
			scores.Sort((x, y) => y.Item1.CompareTo(x.Item2));
			float maxCombo = scores[scores.Count - 1].Item1;
			float maxScore = scores[scores.Count - 1].Item2;

			score.text = maxScore.ToString();
			combo.text = maxCombo.ToString();
		}

		// Change Color
		Image boxBackground = GetComponent<Image>();
		switch (boxColor)
		{
			case BackgroundColor.Red:
				boxBackground.color = new Color(0.760f, 0.145f, 0.145f);
				break;
			case BackgroundColor.Green:
				boxBackground.color = new Color(0.415f, 0.745f, 0.184f);
				break;
			case BackgroundColor.Orange:
				boxBackground.color = new Color(0.874f, 0.443f, 0.145f);
				break;
			case BackgroundColor.Purple:
				boxBackground.color = new Color(0.462f, 0.254f, 0.541f);
				break;
			case BackgroundColor.Yellow:
				boxBackground.color = new Color(0.984f, 0.949f, 0.207f);
				break;
			case BackgroundColor.Blue:
				boxBackground.color = new Color(0.388f, 0.607f, 0.996f);
				break;
			default:
				boxBackground.color = UnityEngine.Color.white;
				break;
		}
	}
}