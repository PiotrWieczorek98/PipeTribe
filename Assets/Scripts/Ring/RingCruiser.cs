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
    int selectedElement = -1;

    private void Awake()
    {
        ringManager = transform.parent.GetComponentInChildren(typeof(RingManager)) as RingManager;
        origin = destination = ringManager.gameObject.transform.position;
    }
    void Update()
    {
        if ((int)ringManager.selectedElement != this.selectedElement)
        {
            selectedElement = (int)ringManager.selectedElement;
            Transform part = ringManager.GetRingElement(selectedElement);
            Vector3 center = part.position;
            center.z = 0;
            destination = origin + (center.normalized * circle_radius);
        }
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * movementSpeed);

    }
}
