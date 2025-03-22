using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIEndGame;
    [SerializeField] private GameObject UIInGame;
    public void ShowUIEndGame()
    {
        UIEndGame.SetActive(true);
    }
    public void HideUIEndGame()
    {
        UIEndGame.SetActive(false);
    }

    public void ShowUIInGame()
    {
        UIInGame.SetActive(true);
    }
    public void HideUIInGame()
    {
        UIInGame.SetActive(false);
    }
}
