using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartItemLayout : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI seller;
    public TextMeshProUGUI price;
    public Image image;

    public void Populate(CartItem item)
    {
        itemName.text = item.name;
        seller.text = item.seller;
        price.text = item.price;
        image.sprite = item.image;
    }
}
