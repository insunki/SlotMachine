using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    static string nextScene;

    [SerializeField] Image LoadingBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; // false인 이유 씬 속도조절

        float timer = 0f;
        while(!op.isDone) // 씬 로딩이 끝나지않은상태
        {
            yield return null; 

            if(op.progress < 0.1f) // 10%가 안되면
            {
                LoadingBar.fillAmount = op.progress; // 진행되게
            }
            else // 10% 넘으면 fakeLoading
            {
                timer += Time.unscaledDeltaTime;
                LoadingBar.fillAmount = Mathf.Lerp(0.1f, 1f, timer); // 10%에서 1초에 걸쳐서 채우게만듬
                if(LoadingBar.fillAmount >= 1f) // 다 채우면
                {
                    op.allowSceneActivation = true;
                    yield return null;
                }
            }
        }
    }
}
