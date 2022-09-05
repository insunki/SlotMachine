using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Information : MonoBehaviour
{
    public GameObject InformationWindow;
    public GameObject ResulteWindow;
    public bool active;
    public GameObject Menu;

    public GameObject[] SlotResulteIcon;

    public SaveLoad saveload;
    public void Exit()
    {
        active = false;
        InformationWindow.SetActive(false);
        ResulteWindow.SetActive(false);
    }

    public void Informatiton()
    {
        active = !active;
        if (active)
        {
            InformationWindow.SetActive(true);
        }
    }
    
    public void NextPage()
    {
        if (active)
        {
            InformationWindow.SetActive(false);
            ResulteWindow.SetActive(true);
        }
    }

    public void Before()
    {
        if (active)
        {
            ResulteWindow.SetActive(false);
            InformationWindow.SetActive(true);
        }
    }

    public void MenuWindow()
    {
        active = !active;
        if(active)
        {
            Menu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void MenuContinue()
    {
        active = false;
        Menu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MenuExit()
    {
        Application.Quit();
    }
    public void MenuSave()
    {
        saveload.SaveData();
    }
}
