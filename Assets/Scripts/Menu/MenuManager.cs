using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public string MusicName { get; private set;}
    public string MusicDir { get; private set; }
    public float BpmValue { get; private set; }
    public float OffsetValue { get; private set; }

    public AudioSource MusicSource { get; private set;}
    public GameObject settings;
    public GameObject menu;

    private void Awake()
    {
        MusicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Find all music in user's level directory
        string musicPath = FindObjectOfType<GameSettings>().LevelDir;
        string[] musicFiles = Directory.GetFiles(musicPath, "*.ogg", SearchOption.AllDirectories);

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
        StartCoroutine(PlayMusic());

        // Load meta data
        LevelDataPasser levelDataPasser = GetComponent<LevelDataPasser>();
        List<(float, float)> metaDataList = levelDataPasser.LoadRecordingFromDat(MusicName);
        BpmValue = metaDataList[0].Item1;
        OffsetValue = metaDataList[0].Item2;

    }

    public IEnumerator PlayMusic()
    {
        yield return StartCoroutine(LoadMp3File(MusicName));
        MusicSource.Play();

        // Start logo beating
        float delay = 1 / (BpmValue / 60);
        MenuAnimator menuAnimator = FindObjectOfType<MenuAnimator>();
        yield return new WaitForSeconds(OffsetValue);
        StartCoroutine(menuAnimator.PlayBeatAnimation(delay));
    }

    // Create Audio Clip from ogg file
    public IEnumerator LoadMp3File(string fileName)
    {
        string uri = "file://" + FindObjectOfType<GameSettings>().LevelDir + "/" + fileName + "/" + fileName + ".ogg";

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
                Debug.Log(www.error);
            else
                MusicSource.clip = DownloadHandlerAudioClip.GetContent(www);
        }
    }

    public GameObject Menu { get { return menu; } }
    public GameObject Settings { get { return settings; } }
}
