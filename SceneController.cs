using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    private static SceneController instance;
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
                instance = new SceneController();

            return instance;
        }
    }

    public enum SceneIndex
    {
        TITLE = 0,
        STAGE_1 = 1,
        STAGE_2 = 2,
        LOADING = 3
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene((int)SceneIndex.TITLE, LoadSceneMode.Additive);
    }

    public void GameStart()
    {
        LoadingSceneManager.setActiveLoadingScene(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.TITLE);
        SceneManager.LoadScene((int)SceneIndex.STAGE_1, LoadSceneMode.Additive);
        LoadingSceneManager.setActiveLoadingScene(false);
    }

    public void EnterStage(int curIdx, int stageIdx)
    {
        LoadingSceneManager.setActiveLoadingScene(true);
        SceneManager.UnloadSceneAsync(curIdx);
        SceneManager.LoadScene(stageIdx, LoadSceneMode.Additive);
        LoadingSceneManager.setActiveLoadingScene(false);
    }

    public void UnloadScene(int stageIdx)
    {
        LoadingSceneManager.setActiveLoadingScene(true);
        SceneManager.UnloadSceneAsync(stageIdx + 1);
    }
}
