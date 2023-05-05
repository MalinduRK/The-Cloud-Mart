using UnityEngine;
using UnityEngine.UI;

public class CartItemLayout : MonoBehaviour
{
    public Text itemName;
    public Text seller;
    public Text price;
    public Image image;

    public void Populate(CartItem item)
    {
        itemName.text = item.name;
        seller.text = item.seller;
        price.text = item.price;
        image.sprite = item.image;
    }
}
