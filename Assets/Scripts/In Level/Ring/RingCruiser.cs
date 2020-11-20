using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCruiser : MonoBehaviour
{
    public float circle_radius = 1;
    public float movementSpeed = 1;

    Vector3 origin;
    Vector3 destination;
    RingManager ringManager;
    RingElementSelector selectedElement;

    private void Awake()
    {
        ringManager = transform.parent.GetComponentInChildren(typeof(RingManager)) as RingManager;
        origin = destination = ringManager.gameObject.transform.position;
        selectedElement = ringManager.selectedElement;
    }
    void Update()
    {
        if (ringManager.selectedElement != selectedElement)
        {
            selectedElement = ringManager.selectedElement;

            Vector3 center = ringManager.selectedElement.transform.position;
            center.z = 0;
            destination = origin + (center.normalized * circle_radius);
        }
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * movementSpeed);

    }
}
