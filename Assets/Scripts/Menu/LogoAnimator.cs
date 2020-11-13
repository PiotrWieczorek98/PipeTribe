using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimator : MonoBehaviour
{
    public float delayBetweenLetters = 0.2f;
    float endOfLetterAnimations = 0;
    public AnimationClip jumpingTopLetters;
    public AnimationClip jumpingBotLetters;

    public AnimationClip spinAnimation;
    public AnimationClip travelUpAnimation;

    Transform letters, ring;
    private void Awake()
    {
        letters = transform.GetChild(0);
        ring = transform.GetChild(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        float delay = 0;
        for (int i = 0; i < letters.childCount; i++)
        {
            if (i < 3)
                StartCoroutine(delayedLetterAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpingTopLetters.name, delay, i));
            else
                StartCoroutine(delayedLetterAnimation(letters.GetChild(i).GetComponent<Animator>(), jumpingBotLetters.name, delay, i));
            delay += delayBetweenLetters;
        }
        endOfLetterAnimations = jumpingBotLetters.length + delay;

        //StartCoroutine(delayRingAnimation(ring.GetComponent<Animator>()));
        ring.GetComponent<Animator>().Play(spinAnimation.name);
    }

    float speed = 0.6f;
    private void Update()
    {

        float delta = 1 + speed * Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x * delta, transform.localScale.y * delta, 1);
        if (transform.localScale.x > 60)
            Destroy(transform.gameObject);

    }

    IEnumerator delayedLetterAnimation(Animator animator, string name, float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        animator.Play(name);
    }
    //IEnumerator delayRingAnimation(Animator animator)
    //{
    //    animator.Play(spinAnimation.name);
    //    yield return new WaitForSeconds(endOfLetterAnimations);
    //}
}
