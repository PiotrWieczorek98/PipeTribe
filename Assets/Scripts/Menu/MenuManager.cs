using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    private void Awake()
    {
        string[] musicFiles = Directory.GetFiles(Application.streamingAssetsPath + "/music", "*.mp3");
        // Random index
        int index = Random.Range(0, musicFiles.Length);
        string musicDir = "file://" + musicFiles[index];

        StartCoroutine(LoadMp3Files(musicDir));
    }

    IEnumerator LoadMp3Files(string uri)
    {
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Or retrieve results as binary data
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
}
