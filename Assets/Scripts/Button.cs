using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{

    public GameController game;

    void OnMouseDown()
    {
        game.SetButtonScore(gameObject.name, 1);
    }

}