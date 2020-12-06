using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MusicLoader : MonoBehaviour
{
	AudioSource musicSource;

	public IEnumerator PlayMusic(AudioSource audioSource, string fileLoc, bool playOnLoad = true)
	{
		musicSource = audioSource;
		yield return StartCoroutine(LoadMp3File(fileLoc));
		if (playOnLoad)
			audioSource.Play();
	}

	// Create Audio Clip from ogg file
	IEnumerator LoadMp3File(string fileLoc)
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
}
