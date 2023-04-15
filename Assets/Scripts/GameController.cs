using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private Realm _realm;
    private GameModel _gameModel;

    public Text scoreText;

    void OnEnable()
    {
        _realm = Realm.GetInstance();
        _gameModel = _realm.Find<GameModel>("poketrainernic");
        if (_gameModel == null)
        {
            _realm.Write(() => {
                _gameModel = _realm.Add(new GameModel("poketrainernic", 0, 0, 0));
            });
        }
    }

    void OnDisable()
    {
        _realm.Dispose();
    }

    public void SetButtonScore(string color, int inc)
    {
        switch (color)
        {
            case "RedSquare":
                _realm.Write(() => {
                    _gameModel.redScore++;
                });
                break;
            case "GreenSquare":
                _realm.Write(() => {
                    _gameModel.greenScore++;
                });
                break;
            case "WhiteSquare":
                _realm.Write(() => {
                    _gameModel.whiteScore++;
                });
                break;
            default:
                Debug.Log("Color Not Found");
                break;
        }
    }

    void Update()
    {
        scoreText.text = "Red: " + _gameModel.redScore + "\n" + "Green: " + _gameModel.greenScore + "\n" + "White: " + _gameModel.whiteScore;
    }

}