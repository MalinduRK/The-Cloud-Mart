using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadMoreItems : MonoBehaviour
{
    public Text promptText;
    public Camera mainCamera;
    // Distance where the text prompt is triggered
    public float maxDistance = 2.5f;
    private MultiModelLoader multiModelLoader;

    // Start is called before the first frame update
    void Start()
    {
        multiModelLoader = gameObject.AddComponent<MultiModelLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.gameObject == gameObject)
        {
            promptText.text = "Load more items\n(LMB)";
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Mouse button pressed");
                multiModelLoader.LoadFiles();
            }
        }
        else
        {
            promptText.text = "";
        }
    }
}
