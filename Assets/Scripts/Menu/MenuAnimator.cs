using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimator : MonoBehaviour
{
    public float delayBetweenLetters;

    public AnimationClip jumpingTopLetters;
    public AnimationClip jumpingBotLetters;

    public AnimationClip spinAnimation;
    public AnimationClip zoomInLogoAnimation;
    public AnimationClip loadingAnimation;
    public AnimationClip beatAnimation;

    public Transform letters;
    public Transform ring;
    public Transform logo;
    float currentLogoBrightness = 0.3f;
    float currentButtonsBrightness = 0f;
    float brightnessIncrease = 0.2f;

    Animator generalAnimator;

    // Start is called before the first frame update
    void Start()
    {
        float delay = 1.5f;
        for (int i = 0; i < letters.childCount; i++)
        {
            if (i < 3)
                StartCoroutine(delayedLetterAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpingTopLetters.name, delay));
            else
                StartCoroutine(delayedLetterAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpingBotLetters.name, delay));
            delay += delayBetweenLetters;
        }

        ring.GetComponent<Animator>().Play(spinAnimation.name);
        logo.GetComponent<Animator>().Play(zoomInLogoAnimation.name);

        generalAnimator = transform.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!logo)
            return;

        if(currentButtonsBrightness < 1)
        {
            currentLogoBrightness += brightnessIncrease * Time.deltaTime;
            currentButtonsBrightness += brightnessIncrease * Time.deltaTime;

            currentLogoBrightness = Mathf.Clamp(currentLogoBrightness, 0, 1);
            currentButtonsBrightness = Mathf.Clamp(currentButtonsBrightness, 0, 1);
        }

        foreach (Transform letter in letters)
            letter.GetComponent<Image>().color = new Color(currentLogoBrightness, currentLogoBrightness, currentLogoBrightness);
        foreach (Transform segment in ring)
            segment.GetComponent<Image>().color = new Color(currentLogoBrightness, currentLogoBrightness, currentLogoBrightness);

    }

    public void PlayLoadingAnimation()
    {
        generalAnimator.Play(loadingAnimation.name);
    }

    public void PlayBeatAnimation()
    {
        generalAnimator.Play(beatAnimation.name);
    }

    IEnumerator delayedLetterAnimation(Animator animator, string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(name);
    }

}
