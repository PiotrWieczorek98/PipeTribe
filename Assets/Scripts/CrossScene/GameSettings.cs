﻿using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Globalization;

/// <summary>
/// Class required to pass informations between scenes
/// </summary>
public static class CrossSceneData
{
	public static string SelectedLevelName { get; set; }
	public static string LevelDir { get; private set; } = Application.streamingAssetsPath + "/levels";
	public static string ScoresDir { get; private set; } = Application.streamingAssetsPath + "/scores";
	public static float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}
}

public class GameSettings : MonoBehaviour
{
	public enum VolumeType { MasterVolume, MusicVolume, EffectsVolume }
	public enum DifficultyType { Speed, NoteOffset, Randomness, HealthRegainOnHit, HealthLossOnFail }
	public enum KeyType { Action1, Action2, Start, Stop, Tap };
	public AudioMixer audioMixer;

	public Dictionary<KeyType, KeyCode> KeyBindings { get; private set; }
	public Dictionary<VolumeType, float> VolumeLevels { get; private set; }
	public Dictionary<DifficultyType, float> DifficultyValues { get; private set; }
	public float Speed { get; private set; }
	public float NotesOffset { get; private set; }
	public float Randomness { get; private set; }
	public float HealthRegainOnHit { get; private set; }
	public float HealthLossOnFail { get; private set; }
	//TODO create settings entry
	public bool ComboEffects { get; private set; } = true;

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 60;

		LoadSettings();
		SaveSettings();
	}

	private void Start()
	{
		// Set volume settings in game
		audioMixer.SetFloat(VolumeType.MasterVolume.ToString(), GetVolume(VolumeType.MasterVolume));
		audioMixer.SetFloat(VolumeType.MusicVolume.ToString(), GetVolume(VolumeType.MusicVolume));
		audioMixer.SetFloat(VolumeType.EffectsVolume.ToString(), GetVolume(VolumeType.EffectsVolume));
	}

	public KeyCode GetBindedKey(KeyType keyType)
	{
		return KeyBindings[keyType];
	}

	public void SetKey(KeyType keyType, KeyCode newKey)
	{
		KeyBindings.Remove(keyType);
		KeyBindings.Add(keyType, newKey);
	}

	public float GetVolume(VolumeType type)
	{
		return VolumeLevels[type];
	}

	public void SetVolume(VolumeSlider volumeSlider)
	{
		audioMixer.SetFloat(volumeSlider.volumeType.ToString(), volumeSlider.volume);

		VolumeLevels.Remove(volumeSlider.volumeType);
		VolumeLevels.Add(volumeSlider.volumeType, volumeSlider.volume);

		// Save changes
		SaveSettings();
	}

	public float GetDifficultyValue(DifficultyType type)
	{
		return DifficultyValues[type];
	}

	public void SetDifficultyValue(DifficultySlider difficultySlider)
	{
		DifficultyValues.Remove(difficultySlider.difficultyType);
		DifficultyValues.Add(difficultySlider.difficultyType, difficultySlider.value);

		// Save changes
		SaveSettings();
	}

	public void LoadSettings()
	{
		// Defaults in case xml is broken / missing
		float masterVolume, musicVolume, effectsVolume;
		KeyCode action1, action2, start, stop, tap;

		action1 = KeyCode.Space;
		action2 = KeyCode.C;
		start = KeyCode.S;
		stop = KeyCode.P;
		tap = KeyCode.T;
		masterVolume = musicVolume = effectsVolume = 0;

		// Read settings from xml
		string fileLoc = Application.streamingAssetsPath + "/settings.xml";
		try
		{
			XDocument xml = XDocument.Load(fileLoc);

			foreach (XElement node in xml.Root.Nodes())
			{
				if (node.Name == "Volume")
				{
					masterVolume = float.Parse(node.Element("Master").Value, CultureInfo.InvariantCulture);
					effectsVolume = float.Parse(node.Element("Effects").Value, CultureInfo.InvariantCulture);
					musicVolume = float.Parse(node.Element("Music").Value, CultureInfo.InvariantCulture);
				}
				else if (node.Name == "KeyBinds")
				{
					action1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Action1").Value);
					action2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Action2").Value);
					start = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Start").Value);
					stop = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Stop").Value);
					tap = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Tap").Value);
				}
				else if (node.Name == "Difficulty")
				{
					Speed = float.Parse(node.Element("Speed").Value, CultureInfo.InvariantCulture);
					NotesOffset = float.Parse(node.Element("NoteOffset").Value, CultureInfo.InvariantCulture);
					Randomness = float.Parse(node.Element("Randomness").Value, CultureInfo.InvariantCulture);
					HealthLossOnFail = float.Parse(node.Element("HealthLossOnFail").Value, CultureInfo.InvariantCulture);
					HealthRegainOnHit = float.Parse(node.Element("HealthRegainOnHit").Value, CultureInfo.InvariantCulture);
				}
			}
		}
		catch
		{
			Debug.LogError("Xml load error");
		}

		// Create dictionaries to use across the game
		KeyBindings = new Dictionary<KeyType, KeyCode>()
		{
			{KeyType.Action1, action1},
			{KeyType.Action2, action2},
			{KeyType.Tap, tap},
			{KeyType.Start, start},
			{KeyType.Stop, stop}
		};

		VolumeLevels = new Dictionary<VolumeType, float>()
		{
			{VolumeType.MasterVolume, masterVolume},
			{VolumeType.MusicVolume, musicVolume},
			{VolumeType.EffectsVolume, effectsVolume},
		};

		DifficultyValues = new Dictionary<DifficultyType, float>()
		{
			{DifficultyType.Speed, Speed},
			{DifficultyType.NoteOffset, NotesOffset},
			{DifficultyType.Randomness, Randomness},
			{DifficultyType.HealthLossOnFail, HealthLossOnFail},
			{DifficultyType.HealthRegainOnHit, HealthRegainOnHit},
		};

	}

	public void SaveSettings()
	{
		string fileLoc = Application.streamingAssetsPath + "/settings.xml";

		XElement xmlTree = new XElement("GameSettings",
			new XElement("Volume",
				new XElement("Master", VolumeLevels[VolumeType.MasterVolume]),
				new XElement("Music", VolumeLevels[VolumeType.MusicVolume]),
				new XElement("Effects", VolumeLevels[VolumeType.EffectsVolume])),
			new XElement("KeyBinds",
				new XElement("Action1", KeyBindings[KeyType.Action1]),
				new XElement("Action2", KeyBindings[KeyType.Action2]),
				new XElement("Start", KeyBindings[KeyType.Start]),
				new XElement("Stop", KeyBindings[KeyType.Stop]),
				new XElement("Tap", KeyBindings[KeyType.Tap])),
			new XElement("Difficulty",
				new XElement("Speed", Speed),
				new XElement("NoteOffset", NotesOffset),
				new XElement("Randomness", Randomness),
				new XElement("HealthLossOnFail", HealthLossOnFail),
				new XElement("HealthRegainOnHit", HealthRegainOnHit))
		);

		xmlTree.Save(fileLoc);
	}

	/// colors: 
	/// red		0.760 0.145 0.145	0.760f, 0.145f, 0.145f
	/// green	0.415 0.745 0.184	0.415f, 0.745f, 0.184f
	/// orange	0.874 0.443 0.145	0.874f, 0.443f, 0.145f
	/// purple	0.462 0.254 0.541	0.462f, 0.254f, 0.541f
	/// yellow	0.984 0.949 0.207	0.984f, 0.949f, 0.207f
	/// blue	0.388 0.607 0.996	0.388f, 0.607f, 0.996f
}
