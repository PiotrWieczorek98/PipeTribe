using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelCreatorManager : MonoBehaviour
{
	public AudioSource MusicSource { get; private set; }
	public bool MusicLoaded { get; private set; } = false;
	public string MusicName { get; set; }
	public float OffsetValue { get; set; } = 0;
	public float BPMValue { get; set; } = 60;
	public int BeatsTotal { get; set; } = 0;

	private void Awake()
	{
		MusicSource = GetComponent<AudioSource>();
	}

	// Create Audio Clip from ogg file
	public IEnumerator LoadMp3File(string fileName)
	{
		MusicLoaded = false;

		string uri = "file://" + CrossSceneData.LevelDir + "/" + fileName + "/" + fileName + ".ogg";

		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS))
		{
			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log(www.error);
			}
			else
			{
				MusicLoaded = true;
				MusicSource.clip = DownloadHandlerAudioClip.GetContent(www);
			}
		}
	}

	public void SetBmpOffset(float bpmValue, float offsetValue)
	{
		BPMValue = bpmValue;
		OffsetValue = offsetValue;
		FindObjectOfType<InputValues>().UpdateFieldValues();
	}

}
