using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public enum Windows { MainMenu, Settings, LevelBrowser }
	public Windows CurrentWindow { get; set; }
	public string MusicName { get; private set; }
	public string MusicDir { get; private set; }
	public float BpmValue { get; private set; }
	public float OffsetValue { get; private set; }
	public AudioSource MusicSource { get; private set; }

	public GameObject settings;
	public GameObject mainMenu;
	public GameObject levelBrowser;

	private void Awake()
	{
		MusicSource = GetComponent<AudioSource>();
		CurrentWindow = Windows.MainMenu;
	}

	private void Start()
	{
		// Find all music in user's level directory
		string[] musicFiles = Directory.GetFiles(CrossSceneData.LevelDir, "*.ogg", SearchOption.AllDirectories);

		// Return if no music was found
		if (musicFiles.Length == 0)
			return;

		// Random index
		//int index = Random.Range(0, musicFiles.Length - 1);
		int index = 1;
		MusicName = Path.GetFileName(musicFiles[index]);
		MusicName = MusicName.Replace(".ogg", "");
		MusicDir = musicFiles[index];

		// Play randomly chosen music
		string uri = "file://" + CrossSceneData.LevelDir + "/" + MusicName + "/" + MusicName + ".ogg";
		StartCoroutine(GetComponent<MusicLoader>().PlayMusic(MusicSource, uri));

		// Load meta data
		LevelDataPasser levelDataPasser = GetComponent<LevelDataPasser>();
		string fileLocation = CrossSceneData.LevelDir + "/" + MusicName + "/" + MusicName;
		List<(float, float)> metaDataList = levelDataPasser.LoadLevelDataFromDat(fileLocation);
		BpmValue = metaDataList[0].Item1;
		OffsetValue = metaDataList[0].Item2;

		// Make logo beat to music
		StartCoroutine(OffsetBeatAnimation());
	}

	public IEnumerator OffsetBeatAnimation()
	{
		float delay = 1 / (BpmValue / 60);
		MenuAnimator menuAnimator = FindObjectOfType<MenuAnimator>();
		yield return new WaitForSeconds(OffsetValue);
		StartCoroutine(menuAnimator.PlayBeatAnimation(delay));
	}

	public GameObject Menu { get { return mainMenu; } }
	public GameObject Settings { get { return settings; } }
}
