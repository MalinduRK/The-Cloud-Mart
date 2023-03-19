using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityTrigger : MonoBehaviour
{
    public GameObject objectToHide;

    private void Start()
    {
        objectToHide.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToHide.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToHide.SetActive(false);
        }
    }

}
