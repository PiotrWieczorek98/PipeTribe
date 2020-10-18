using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float approachTime = 1;
    public float fadeSpeed = 0.00f;
    
    float lifeTime = 0;
    bool fadeOutFlag = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime >= approachTime && !fadeOutFlag)
        {
            fadeOutFlag = true;
            StartCoroutine(FadeOutObject());
        }
    }

    private IEnumerator FadeOutObject()
    {
        while (this.GetComponent<Renderer>().material.color.a > 0)
        {
            Color color = this.GetComponent<Renderer>().material.color;
            float fadeAmount = color.a - (fadeSpeed * Time.deltaTime);

            Color fadedColor = new Color(color.r, color.g, color.b, color.a);
            this.GetComponent<Renderer>().material.color = fadedColor;
        }

        yield return null;
    }
}
