using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : MonoBehaviour
{
	public enum VolumeType { MasterVolume, MusicVolume, EffectsVolume }
	public enum KeyType { Action1, Action2, Start, Stop, Tap };
	public AudioMixer audioMixer;

	public Dictionary<KeyType, KeyCode> KeyBindings { get; private set; }
	public Dictionary<VolumeType, float> VolumeLevels { get; private set; }
	public string LevelDir { get; private set; }
	public float MusicNotesOffset { get; private set; } = 0;

	private void Awake()
	{
		QualitySettings.vSyncCount = 0;  // VSync must be disabled
		Application.targetFrameRate = 60;

		LevelDir = Application.streamingAssetsPath + "/levels";
		LoadSettings();
		SaveSettings();
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
					masterVolume = float.Parse(node.Element("Master").Value);
					effectsVolume = float.Parse(node.Element("Effects").Value);
					musicVolume = float.Parse(node.Element("Music").Value);
				}
				else if (node.Name == "KeyBinds")
				{
					action1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Action1").Value);
					action2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Action2").Value);
					start = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Start").Value);
					stop = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Stop").Value);
					tap = (KeyCode)System.Enum.Parse(typeof(KeyCode), node.Element("Tap").Value);
				}
			}
		}
		catch
		{
			Debug.LogError("Xml load error");
		}

		// Set those settings in game
		KeyBindings = new Dictionary<KeyType, KeyCode>()
		{
			{KeyType.Action1,    action1},
			{KeyType.Action2,    action2 },
			{KeyType.Tap,        tap },
			{KeyType.Start,      start },
			{KeyType.Stop,       stop }
		};

		VolumeLevels = new Dictionary<VolumeType, float>()
		{
			{VolumeType.MasterVolume,   masterVolume},
			{VolumeType.MusicVolume,    musicVolume},
			{VolumeType.EffectsVolume,  effectsVolume},
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
				new XElement("Tap", KeyBindings[KeyType.Tap]))
		);

		xmlTree.Save(fileLoc);
	}
}
