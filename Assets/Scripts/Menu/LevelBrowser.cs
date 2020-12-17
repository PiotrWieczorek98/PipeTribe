using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//	TODO: load all music at once and switch on select
public class LevelBrowser : MonoBehaviour
{
	public GameObject levelBox;
	List<RectTransform> boxes;
	List<AudioClip> musicList;
	int selectedBox = 0;

	float movementSpeed = 3;
	Vector3 globalDestination;
	MenuManager menuManager;

	public void Awake()
	{
		menuManager = FindObjectOfType<MenuManager>();

		boxes = new List<RectTransform>();
		globalDestination = new Vector3();
		musicList = new List<AudioClip>();


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
			box.GetComponent<LevelBox>().InitializeBox(levelName, boxColor);
			box.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);

			// Add to list
			boxes.Add(box.GetComponent<RectTransform>());
			
			// Increment
			posY -= 100;
			if (colorIndex < 5)
				colorIndex++;
			else
				colorIndex = 0;
		}

		CrossSceneData.SelectedLevelName = new DirectoryInfo(levelsSorted[0]).Name;
		// Load ogg files to AudioClip list
		StartCoroutine(LoadMusicList(levelsSorted));
	}

	IEnumerator LoadMusicList(List<string> levelsSorted)
	{
		MusicLoader musicLoader = FindObjectOfType<MusicLoader>();
		yield return StartCoroutine(musicLoader.LoadMusicFiles(musicList, levelsSorted));

		// Disable level browser because user is currently in main menu
		transform.parent.gameObject.SetActive(false);
	}

	public void SetFirstMusic()
	{
		menuManager.MusicSource.clip = musicList[0];
		menuManager.MusicSource.Play();
	}

	private void Update()
	{
		bool moved = false;
		if (Input.mouseScrollDelta.y > 0)
		{
			MoveUp();
			moved = true;
		}
		else if (Input.mouseScrollDelta.y < 0)
		{
			MoveDown();
			moved = true;

		}

		// If list was moved
		if (moved)
		{
			CrossSceneData.SelectedLevelName = boxes[selectedBox].GetComponent<LevelBox>().title.text;
			menuManager.MusicSource.clip = musicList[selectedBox];
			menuManager.MusicSource.Play();
		}

		// Move animation requires interpolation
		for (int i = 0; i < boxes.Count; i++)
		{
			Vector3 localDestination = new Vector3(0, i * -100f + globalDestination.y, 1);
			float distanceFromCenter = Mathf.Abs(i - selectedBox);
			if (distanceFromCenter > 3)
			{
				distanceFromCenter = 3;
			}
			distanceFromCenter = CrossSceneData.Remap(distanceFromCenter, 0, boxes.Count, 4, 2f);
			Vector3 localScale = new Vector3(distanceFromCenter, distanceFromCenter, 1);
			boxes[i].anchoredPosition = Vector3.Lerp(boxes[i].anchoredPosition, localDestination, Time.deltaTime * movementSpeed);
			boxes[i].localScale = Vector3.Lerp(boxes[i].localScale, localScale, Time.deltaTime * movementSpeed);
		}
	}

	public void MoveDown()
	{
		if (selectedBox < boxes.Count - 1)
		{
			selectedBox++;
			globalDestination = new Vector3(0, globalDestination.y + 100f, 1);
		}
		else
		{
			selectedBox = 0;
			globalDestination = new Vector3(0, 0, 1);
		}
	}

	public void MoveUp()
	{
		if (selectedBox > 0)
		{
			selectedBox--;
			globalDestination = new Vector3(0, globalDestination.y - 100f, 1);
		}
		else
		{
			selectedBox = boxes.Count - 1;
			globalDestination = new Vector3(0, (boxes.Count - 1) * 100f, 1);
		}
	}
}
