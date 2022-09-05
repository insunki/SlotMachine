using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class Title : MonoBehaviour
{
    private SaveLoad save;
    public static Title instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public void ClickStart()
    {
        Loading.LoadScene("Main");
        gameObject.SetActive(false);
    }

    public void ClickLoad()
    {
        StartCoroutine(LoadGame());
    }
    IEnumerator LoadGame()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main");
        while (!operation.isDone)
        {
            yield return null;
        }
        save = FindObjectOfType<SaveLoad>();
        save.LoadData();
        gameObject.SetActive(false);
    }

    public void ClickExit()
    {
        Application.Quit();
    }
}
