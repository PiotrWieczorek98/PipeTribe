using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MusicLoader : MonoBehaviour
{
	AudioSource musicSource;

	public IEnumerator PlayMusic(AudioSource audioSource, string fileLoc, bool playOnLoad = true)
	{
		musicSource = audioSource;
		yield return StartCoroutine(SetMusicFile(fileLoc));
		if (playOnLoad)
			audioSource.Play();
	}

	// Create Audio Clip from ogg file
	IEnumerator SetMusicFile(string fileLoc)
	{
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileLoc, AudioType.OGGVORBIS))
		{
			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log(www.error);
			}
			else
			{
				musicSource.clip = DownloadHandlerAudioClip.GetContent(www);
			}
		}
	}

	//TODO FINISH LOADING ALL MUSIC TO LEVEL BROWSER

	public IEnumerator LoadMusicFiles(List<AudioClip> musicList, List<string> filesLoc)
	{
		foreach(string file in filesLoc)
		{
			string levelName = new System.IO.DirectoryInfo(file).Name;
			levelName = file + "/" + levelName + ".ogg";

			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(levelName, AudioType.OGGVORBIS))
			{
				yield return www.SendWebRequest();

				if (www.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log(www.error);
				}
				else
				{
					musicList.Add(DownloadHandlerAudioClip.GetContent(www));
				}
			}
		}
	}
}
