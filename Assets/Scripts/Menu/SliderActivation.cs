using UnityEngine;
using UnityEngine.UI;

public class SliderActivation : MonoBehaviour
{
    void OnMouseEnter()
    {
        GetComponent<Slider>().enabled = true;
    }
    private void OnMouseExit()
    {
        GetComponent<Slider>().enabled = false;
    }
}
