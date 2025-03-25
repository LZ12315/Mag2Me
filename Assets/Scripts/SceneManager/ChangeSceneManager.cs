using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    [SerializeField] string sceneName;

    GameObject changeSceneObj;

    Image changeSceneImage;


    private void Start()
    {
        changeSceneObj = GameObject.FindGameObjectWithTag("ChangeScene");
        if(changeSceneObj == null )
        {
            Debug.LogWarning("无法切换场景");
        }

        changeSceneImage = changeSceneObj.GetComponent<Image>();

        EventCenter.Instance.AddEventListener("切换场景" + sceneName, () => StartLoad());
    }



    private void StartLoad()
    {
        EventCenter.Instance.Clear();

        changeSceneImage.DOFade(1f, 0.5f);

        StartCoroutine(ReallyLoadSceneAsyn(sceneName));
    }



    private IEnumerator ReallyLoadSceneAsyn(string scene)
    {

        AsyncOperation asy = SceneManager.LoadSceneAsync(scene);
        asy.allowSceneActivation = false;
        yield return new WaitForSeconds(1.5f);
        if (!asy.isDone)
        {
            if (asy.progress == 0.9f)
            {
                asy.allowSceneActivation = true;
                changeSceneImage.DOFade(0f, 0.5f);
            }
            yield return null;
        }


    }






}
