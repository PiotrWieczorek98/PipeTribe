using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterAnimator : MonoBehaviour
{
    List<Animator> animators = new List<Animator>();
    public float delayBetweenLetters = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        float offset = 0;
        Transform letters = transform.GetChild(0);
        foreach (Transform letter in letters)
            animators.Add(letter.GetComponent<Animator>());

        for(int i = 0; i < animators.Count; i++)
        {
            if (i < 3)
                StartCoroutine(delayedAnimation("JumpingTop", offset, i));
            else
                StartCoroutine(delayedAnimation("JumpingBot", offset, i));
            offset += delayBetweenLetters;
        }
    }

    IEnumerator delayedAnimation(string name, float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        animators[index].Play(name);
    }
}
