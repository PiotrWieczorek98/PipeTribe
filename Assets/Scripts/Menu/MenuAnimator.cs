using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuAnimator : MonoBehaviour
{
    public float delayBetweenLetters;

    public AnimationClip jumpTop;
    public AnimationClip jumpBot;
    public AnimationClip spinRing;
    public AnimationClip zoomIn;
    public AnimationClip loading;
    public AnimationClip beat;
    public AnimationClip zoomOut;

    public Transform letters;
    public Transform logo;
    public Transform settings;
    float currentLogoBrightness = 0.3f;
    float currentButtonsBrightness = 0f;
    float brightnessIncrease = 0.2f;

    AudioSource musicSource;
    float currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        float delay = 1.5f;
        for (int i = 0; i < letters.childCount; i++)
        {
            if (i < 3)
                StartCoroutine(PlayDelayedAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpTop.name, delay));
            else
                StartCoroutine(PlayDelayedAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpBot.name, delay));
            delay += delayBetweenLetters;
        }

        logo.GetComponent<Animator>().Play(zoomIn.name);

        musicSource = FindObjectOfType<MenuManager>().MusicSource;
    }

    public void PlayAnimation(Animator animator,AnimationClip clip)
    {
        animator.Play(clip.name);
    }

    public IEnumerator PlayBeatAnimation(float delay)
    {
        while(currentTime < musicSource.clip.length && letters.parent.gameObject.activeSelf)
        {
            letters.GetComponent<Animator>().Play(beat.name);
            currentTime = musicSource.time;
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator PlayDelayedAnimation(Animator animator, string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(name);
    }

}
