using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resulte : MonoBehaviour
{
    public Text[] SlotResulteMoney;
    private GameControl GC;
    public float ResulteBet;
    private float ResulteMoney;
    public int number;
    // Start is called before the first frame update
    void Awake()
    {
        GC = FindObjectOfType<GameControl>();
        
    }
    private void OnEnable()
    {
        SlotResulte();
    }

    public void SlotResulte()
    {
        ResulteMoney = 0.1f * GC.Betting * ResulteBet;
        SlotResulteMoney[2].text = GC.CommaText(ResulteMoney).ToString();
        ResulteMoney = 0.3f * GC.Betting * ResulteBet;
        SlotResulteMoney[1].text = GC.CommaText(ResulteMoney).ToString();
        ResulteMoney = 0.7f * GC.Betting * ResulteBet;
        SlotResulteMoney[0].text = GC.CommaText(ResulteMoney).ToString();
    }
}
