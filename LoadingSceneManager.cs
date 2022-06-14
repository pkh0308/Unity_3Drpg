using System;
using UnityEngine;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScene;

    public static Action<bool> setActiveLoadingScene;

    void Awake()
    {
        setActiveLoadingScene = (a) => { SetActiveLoadingScene(a); };
        SceneController.Instance.LoadTitle();
    }

    public void SetActiveLoadingScene(bool action)
    {
        loadingScene.SetActive(action);
    }
}
