using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public int moveDelay = 10;
    public int radius = 3;
    public bool isRandom = false;

    bool waiting = false;
    Vector3 destination = Vector3.zero;

    // Use this for initialization
    void Start(){}

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!waiting && isRandom)
        {
            StartCoroutine(LerpCube());
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, -destination, Time.deltaTime * movementSpeed);
        }
    }

    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
    }


    IEnumerator LerpCube()
    {
        destination = new Vector3(Random.Range(radius, -radius), Random.Range(radius, -radius));
        waiting = true;
        yield return new WaitForSeconds(moveDelay);
        waiting = false;
    }

}
