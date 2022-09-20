using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bonus : MonoBehaviour
{
    public GameObject BonusWindow;
    public GameObject BonusCountUI;
    public GameObject BonusResulteMoney;
    public GameObject Logo;
    public GameObject BonusLogo;
    public Text BonusCount;
    private GameControl game;
    public int num;
    public int Count;
    public float multiply;
    void Awake()
    {
        game = FindObjectOfType<GameControl>();
    }

    void Bonuscontents()
    {
        BonusWindow.SetActive(false);
        BonusCountUI.SetActive(true);
        BonusCount.text = num.ToString();
        BonusResulteMoney.SetActive(true);
        Logo.SetActive(false);
        BonusLogo.SetActive(true);
        game.AutoBtn();
    }

    public void Bonus20Game()
    {
        num = 20;
        Count = 20;
        multiply = 1f;
        Bonuscontents();
    }
    public void Bonus15Game()
    {
        num = 15;
        Count = 15;
        multiply = 1.5f;
        Bonuscontents();
    }
    public void Bonus10Game()
    {
        num = 10;
        Count = 10;
        multiply = 2f;
        Bonuscontents();
    }
    public void Bonus8Game()
    {
        num = 8;
        Count = 8;
        multiply = 2.5f;
        Bonuscontents();
    }
    public void Bonus5Game()
    {
        num = 5;
        Count = 5;
        multiply = 3f;
        Bonuscontents();
    }

}
