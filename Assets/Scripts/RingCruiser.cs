using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCruiser : MonoBehaviour
{
    public float circle_radius = 1;
    public float movementSpeed = 1;

    Vector3 origin;
    Vector3 destination;
    RingManager parent;
    int selectedPart = -1;
    void Start()
    {
        parent = transform.parent.GetComponent(typeof(RingManager)) as RingManager;
        origin = destination = parent.transform.position;
    }
    void Update()
    {
        if ((int)parent.selectedElement != this.selectedPart)
        {
            selectedPart = (int)parent.selectedElement;
            Transform part = parent.GetRingElement(selectedPart);
            Vector3 center = part.position;
            center.z = 0;
            destination = origin + (center.normalized * circle_radius);
        }
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * movementSpeed);

    }
}
