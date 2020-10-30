using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponents : MonoBehaviour
{
    Text timer, combo;
    RingManager ringManager;
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            switch (child.name)
            {
                case "Timer":
                    timer = child.GetComponent(typeof(Text)) as Text;
                    break;
                case "Combo":
                    combo = child.GetComponent(typeof(Text)) as Text;
                    break;
                case "Color Ring":
                    ringManager = child.GetComponent(typeof(RingManager)) as RingManager;
                    break;
            }
            
        }
    }

    public Text GetCombo { get { return combo; } }
    public Text GetTimer { get { return timer; } }
    public RingManager GetRingManager { get { return ringManager; } }

}
