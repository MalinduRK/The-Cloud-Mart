using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartMenuController : MonoBehaviour
{
    public GameObject cartMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the menus on start
        cartMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
