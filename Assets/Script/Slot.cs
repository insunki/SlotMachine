using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isSlotsStop;

    private int randomValue;
    private float timeInterval;

    private GameControl gamecontrol;

    void Awake()
    {
        gamecontrol = FindObjectOfType<GameControl>();
    }
    void Start()
    {
        isSlotsStop = true;
        GameControl.SpinStart += Spin;
    }

    public void Spin()
    {
        StartCoroutine(Spinslot());
    }
    private IEnumerator Spinslot()
    {
        isSlotsStop = false;
        timeInterval = 0.025f;
        gamecontrol.IconRandom();
        gamecontrol.Setting();
        randomValue = Random.Range(20, 60);
        for (int i = 0; i < randomValue; i++)
        {
            if (transform.position.y <= -4.2f)
                transform.position = new Vector2(transform.position.x, 0);
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.7f);
            if (i > Mathf.RoundToInt(randomValue * 0.7f))
                timeInterval = 0.05f;
            yield return new WaitForSeconds(timeInterval);
        }
        transform.position = new Vector2(transform.position.x, 0);
        isSlotsStop = true;
    }
}
