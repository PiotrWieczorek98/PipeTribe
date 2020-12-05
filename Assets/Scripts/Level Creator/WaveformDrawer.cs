using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformDrawer : MonoBehaviour
{
	public Color waveformColor = Color.green;
	public Color backgroundColor = Color.blue;

	public Image timeline;
	public AudioClip monoAudioClip;

	RectTransform timelineTransform;
	TimelineIndicator timelineIndicator;

	public Texture2D CreateWaveformSpectrumTexture(int leftBorder, int rightBorder)
	{
		int width = (int)timelineTransform.rect.width;
		int height = (int)timelineTransform.rect.height;
		Texture2D timelineTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

		float[] samples = new float[monoAudioClip.samples];
		float[] waveform = new float[width];
		monoAudioClip.GetData(samples, 0);

		List<float> samplesList = new List<float>(samples);
		leftBorder = (int)CrossSceneData.Remap(leftBorder, 0, Screen.width, 0, samplesList.Count);
		rightBorder = (int)CrossSceneData.Remap(rightBorder, 0, Screen.width, 0, samplesList.Count);
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

	public AudioClip CloneAudioClipToMono()
	{
		AudioClip source = FindObjectOfType<LevelCreatorManager>().MusicSource.clip;
		AudioClip monoAudioClip = AudioClip.Create(source.name + "Mono", source.samples, 1, source.frequency, false);
		float[] stereoVersion = new float[source.samples * source.channels];
		float[] monoVersion = new float[source.samples];
		source.GetData(stereoVersion, 0);

		for (int i = 0, j = 0; i < stereoVersion.Length; i += 2, j++)
		{
			monoVersion[j] = stereoVersion[i];
		}
		monoAudioClip.SetData(monoVersion, 0);

		return monoAudioClip;
	}


	public void InitializeTimelineSettings()
	{
		timelineTransform = timeline.GetComponent<RectTransform>();
		timelineIndicator = timeline.GetComponent<TimelineIndicator>();

		timelineIndicator.enabled = true;
		timelineIndicator.InitializeTimeline();
	}

	public void DrawTimeline()
	{
		monoAudioClip = CloneAudioClipToMono();
		Texture2D texture = CreateWaveformSpectrumTexture(0, Screen.width);
		timeline.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}

	public void OverrideSprite(Texture2D texture)
	{
		timeline.overrideSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}
}
