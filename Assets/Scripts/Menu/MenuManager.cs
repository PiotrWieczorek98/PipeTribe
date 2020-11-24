using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public string MusicName { get; private set;}
    public string MusicDir { get; private set; }

    private void Awake()
    {
        // Find all music in user's level directory
        string musicPath = FindObjectOfType<GameSettings>().LevelDir;
        string[] musicFiles = Directory.GetFiles(musicPath, "*.mp3", SearchOption.AllDirectories);
        // Random index
        int index = Random.Range(0, musicFiles.Length);
        MusicName = Path.GetFileName(musicFiles[index]);
        MusicDir = musicFiles[index];
        // Play randomly chosen music
        StartCoroutine(LoadMp3File("file://" + MusicDir));
    }

    // Create Audio Clip from mp3 file
    IEnumerator LoadMp3File(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield return null;
        }

        // Retrieve results as binary data
        byte[] results = www.downloadHandler.data;
        MemoryStream memStream = new MemoryStream(results);
        NLayer.MpegFile mpgFile = new NLayer.MpegFile(memStream);
        float[] samples = new float[mpgFile.Length];
        mpgFile.ReadSamples(samples, 0, (int)mpgFile.Length);

        AudioClip backgroundMusic = AudioClip.Create("foo", samples.Length, mpgFile.Channels, mpgFile.SampleRate, false);
        backgroundMusic.SetData(samples, 0);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }
}
