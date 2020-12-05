using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//	TODO: load all music at once and switch on select
public class LevelBrowser : MonoBehaviour
{
	public GameObject levelBox;
	List<RectTransform> boxes;
	int selectedBox = 0;

	float movementSpeed = 3;
	Vector3 gloabalDestination;

	private void Awake()
	{
		boxes = new List<RectTransform>();
		gloabalDestination = new Vector3();

		// Find all music in user's level directory
		string[] levels = Directory.GetDirectories(CrossSceneData.LevelDir);

		// Return if no music was found
		if (levels.Length == 0)
			return;

		List<string> levelsSorted = new List<string>(levels);
		levelsSorted.Sort();

		int colorIndex = 0;
		float posY = 0;
		foreach (string level in levelsSorted)
		{
			// Create object
			string levelName = new DirectoryInfo(level).Name;
			GameObject box = Instantiate(levelBox, transform);

			// Set color
			LevelBox.BackgroundColor boxColor = (LevelBox.BackgroundColor)colorIndex;

			// Initialize object
			levelBox.GetComponent<LevelBox>().InitializeBox(levelName, boxColor, posY);

			if (colorIndex < 5)
				colorIndex++;
			else
				colorIndex = 0;
			posY -= 100;

			// Add to list
			boxes.Add(box.GetComponent<RectTransform>());
		}
	}

	private void Update()
	{
		if (Input.mouseScrollDelta.y > 0 && selectedBox > 0)
		{
			selectedBox--;
			gloabalDestination = new Vector3(0, gloabalDestination.y - 100f, 1);
		}
		else if (Input.mouseScrollDelta.y < 0 && selectedBox < boxes.Count - 1)
		{
			selectedBox++;
			gloabalDestination = new Vector3(0, gloabalDestination.y + 100f, 1);
		}

		for (int i = 0; i < boxes.Count; i++)
		{
			Vector3 localDestination = new Vector3(0, i * -100f + gloabalDestination.y, 1);
			float scale = CrossSceneData.Remap(Mathf.Abs(i - selectedBox), 0, boxes.Count, 4, 2f);
			Vector3 localScale = new Vector3(scale, scale, 1);
			boxes[i].anchoredPosition = Vector3.Lerp(boxes[i].anchoredPosition, localDestination, Time.deltaTime * movementSpeed);
			boxes[i].localScale = Vector3.Lerp(boxes[i].localScale, localScale, Time.deltaTime * movementSpeed);
		}

	}
}
