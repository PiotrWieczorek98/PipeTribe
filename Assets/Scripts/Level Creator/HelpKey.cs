using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpKey : MonoBehaviour
{
    public GameSettings.KeyMap helpKey;
    void Start()
    {
        GetComponent<Text>().text = FindObjectOfType<GameSettings>().GetKeyBind(helpKey).ToString();
    }
}
