using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public enum KeyMap { Action1, Action2, Start, Stop, Tap};
    public AudioMixer audioMixer;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;

        // Key bindings
        KeyBindings = new Dictionary<KeyMap, KeyCode>()
        { 
            {KeyMap.Action1,    KeyCode.Space},
            {KeyMap.Action2,    KeyCode.C },
            {KeyMap.Tap,        KeyCode.B },
            {KeyMap.Start,      KeyCode.P },
            {KeyMap.Stop,       KeyCode.S }
        };

        LevelDir = Application.streamingAssetsPath + "/levels";
    }

    public KeyCode GetBindedKey(KeyMap keyType)
    {
        return KeyBindings[keyType];
    }

    public void ChangeKey(KeyMap keyType, KeyCode newKey)
    {
        KeyBindings.Remove(keyType);
        KeyBindings.Add(keyType, newKey);
    }

    public void ChangeMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value);
    }
    public void ChangeMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value);
    }
    public void ChangeEffectsVolume(float value)
    {
        audioMixer.SetFloat("EffectsVolume", value);
    }
    public Dictionary<KeyMap, KeyCode> KeyBindings { get; private set; }
    public string LevelDir { get; private set; }
    public float MusicNotesOffset { get; private set; } = 0;
}
