using UnityEngine;

public class CartPanelController : MonoBehaviour
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
