using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CartItemLayout : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI seller;
    public TextMeshProUGUI price;
    public Image image;
    public TextMeshProUGUI quantity;

    public void Populate(CartItem item)
    {
        itemName.text = item.name;
        seller.text = item.seller;
        price.text = "Rs." + item.price;
        image.sprite = item.image;
        quantity.text = "X" + item.quantity.ToString();
    }
}
