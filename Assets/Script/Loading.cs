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
        op.allowSceneActivation = false; // false�� ���� �� �ӵ�����

        float timer = 0f;
        while(!op.isDone) // �� �ε��� ��������������
        {
            yield return null; 

            if(op.progress < 0.1f) // 10%�� �ȵǸ�
            {
                LoadingBar.fillAmount = op.progress; // ����ǰ�
            }
            else // 10% ������ fakeLoading
            {
                timer += Time.unscaledDeltaTime;
                LoadingBar.fillAmount = Mathf.Lerp(0.1f, 1f, timer); // 10%���� 1�ʿ� ���ļ� ä��Ը���
                if(LoadingBar.fillAmount >= 1f) // �� ä���
                {
                    op.allowSceneActivation = true;
                    yield return null;
                }
            }
        }
    }
}
