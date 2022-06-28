using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//플레이어 씬에 플레이어 및 메인카메라, GameManager, ObjectManager, UiManager 등 배치
//로딩 매니저 씬과 플레이어 씬은 계속 활성화 된 채로 진행, 스테이지 씬은 추가로 로드 및 언로드

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScene;

    public static Action gameStart;
    public static Action<int> enterStage;
    public static Action<int> setActiveScene;

    int curIdx;

    public enum SceneIndex
    {
        TITLE = 0,
        STAGE_1 = 1,
        STAGE_2 = 2,
        LOADING = 3,
        PLAYER_SCENE = 4
    }

    void Awake()
    {
        gameStart = () => { GameStart(); };
        enterStage = (a) => { EnterStage(a); };
        setActiveScene = (a) => { SetActiveScene(a); };

        curIdx = 1;
        LoadTitle();
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene((int)SceneIndex.TITLE, LoadSceneMode.Additive);
    }

    public void SetActiveScene(int sceneIdx)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIdx));
    }

    //로딩스크린 활성화 후 타이틀 씬 언로드, 플레이어씬 및 스테이지1 로드
    public void GameStart()
    {
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.TITLE);
        SceneManager.LoadScene((int)SceneIndex.PLAYER_SCENE, LoadSceneMode.Additive);
        StartCoroutine(Loading((int)SceneIndex.STAGE_1));
    }

    public void EnterStage(int stageIdx)
    {
        loadingScene.SetActive(true);
        SceneManager.UnloadSceneAsync(curIdx);
        StartCoroutine(Loading(stageIdx));
        curIdx = stageIdx;
    }

    //입장하는 스테이지를 LoadSceneAsync로 로딩하며 AsyncOperation 변수에 저장
    //로딩 작업이 완료될때까지 대기한 후 해당 스테이지를 액티브 씬으로 설정, 이후 오브젝트 생성
    IEnumerator Loading(int idx)
    {
        WaitForSeconds seconds = new WaitForSeconds(0.1f);
        AsyncOperation op = SceneManager.LoadSceneAsync(idx, LoadSceneMode.Additive);
        //로딩 완료될때까지 대기
        while (!op.isDone)
        {
            yield return seconds;
        }
        yield return new WaitForSeconds(0.5f); //로딩 화면 체크용 추가 시간, 나중에 삭제할 것
        loadingScene.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx));
        ObjectManager.loadObjects(idx);
    }
}
