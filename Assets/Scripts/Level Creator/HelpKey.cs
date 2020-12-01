using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpKey : MonoBehaviour
{
    public GameSettings.KeyType helpKey;
    void Start()
    {
        GetComponent<Text>().text = FindObjectOfType<GameSettings>().GetBindedKey(helpKey).ToString();
    }
}
