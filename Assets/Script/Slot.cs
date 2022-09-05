using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool slotsStop;

    private int randomValue;
    private float timeInterval;

    private GameControl gamecontrol;

    void Awake()
    {
        gamecontrol = FindObjectOfType<GameControl>();
    }
    void Start()
    {
        slotsStop = true;
        GameControl.SpinStart += Spin; // 액션에 스핀 추가
    }

    public void Spin()
    {
        StartCoroutine(Spinslot());
    }
    private IEnumerator Spinslot()
    {
        slotsStop = false;
        timeInterval = 0.025f;
        for (int i = 0; i < 1; i++)
        {
            gamecontrol.IconRandom();
            gamecontrol.Setting();
            if (transform.position.y <= -4.2f)
            {
                transform.position = new Vector2(transform.position.x, 0);

            }
            yield return new WaitForSeconds(timeInterval);
        }
        randomValue = Random.Range(20, 60);
        switch (randomValue % 3)
        {
            case 1:
                randomValue += 2;
                break;
            case 2:
                randomValue += 1;
                break;
        }
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

        slotsStop = true;
    }
}
