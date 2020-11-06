using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformDrawer : MonoBehaviour
{
    public Color waveformColor = Color.green;
    public Color backgroundColor = Color.blue;

    public Image timeline;
    public AudioClip audioClip, monoAudioClip;

    RectTransform timelineTransform;

    public Texture2D CreateWaveformSpectrumTexture(int leftBorder, int rightBorder)
    {
        int width = (int)timelineTransform.rect.width;
        int height = (int)timelineTransform.rect.height;
        Texture2D timelineTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        float[] samples = new float[monoAudioClip.samples];
        float[] waveform = new float[width];
        monoAudioClip.GetData(samples, 0);
        
        List<float> samplesList = new List<float>(samples);
        leftBorder = (int)Remap(leftBorder, 0, Screen.width, 0, samplesList.Count);
        rightBorder = (int)Remap(rightBorder, 0, Screen.width, 0, samplesList.Count);
        samplesList.RemoveRange(rightBorder, samplesList.Count - rightBorder);
        samplesList.RemoveRange(0, leftBorder);


        int packSize = (samplesList.Count / width) + 1;
        int s = 0;
        for (int i = 0; i < samplesList.Count; i += packSize)
        {
            waveform[s] = Mathf.Abs(samplesList[i]);
            s++;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                timelineTexture.SetPixel(x, y, backgroundColor);
            }
        }

        for (int x = 0; x < waveform.Length; x++)
        {
            for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++)
            {
                timelineTexture.SetPixel(x, (height / 2) + y, waveformColor);
                timelineTexture.SetPixel(x, (height / 2) - y, waveformColor);
            }
        }
        timelineTexture.Apply();

        return timelineTexture;
    }

    public AudioClip CloneAudioClipToMono(AudioClip sourceAudioClip)
    {
        AudioClip monoAudioClip = AudioClip.Create(sourceAudioClip.name + "Mono", sourceAudioClip.samples, 1, sourceAudioClip.frequency, false);
        float[] stereoVersion = new float[sourceAudioClip.samples * sourceAudioClip.channels];
        float[] monoVersion = new float[sourceAudioClip.samples];
        sourceAudioClip.GetData(stereoVersion, 0);

        for (int i = 0, j = 0; i < stereoVersion.Length; i += 2, j++)
        {
            monoVersion[j] = stereoVersion[i];
        }
        monoAudioClip.SetData(monoVersion, 0);

        return monoAudioClip;
    }


    void Awake()
    {
        timelineTransform = timeline.GetComponent(typeof(RectTransform)) as RectTransform;

        monoAudioClip = CloneAudioClipToMono(audioClip);
        Texture2D texture = CreateWaveformSpectrumTexture(0, Screen.width);
        timeline.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void OverrideSprite(Texture2D texture)
    {
        timeline.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
