using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//플레이어 씬에 플레이어 및 메인카메라, GameManager, ObjectManager, UiManager 등 배치
//로딩 매니저 씬과 플레이어 씬은 계속 활성화 된 채로 진행, 스테이지 씬은 추가로 로드 및 언로드
public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager Inst { get; private set; }

    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject loadingCamera;
    WaitForSeconds interval;

    int curIdx;

    public enum SceneIndex
    {
        LOADING = 0,
        STAGE_1 = 1,
        STAGE_2 = 2,
        TITLE = 3,
        PLAYER_SCENE = 4
    }

    void Awake()
    {
        Inst = this;

        Application.targetFrameRate = 60; //프레임 제한

        curIdx = 1;
        interval = new WaitForSeconds(0.1f);

        loadingScreen.SetActive(true);
    }

    public void LoadTitle()
    {
        StartCoroutine(Loading((int)SceneIndex.TITLE));
    }

    public void SetLoadingScreen(bool act)
    {
        loadingScreen.SetActive(act);
    }

    public void SetActiveScene(int sceneIdx)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIdx));
    }

    //로딩스크린 활성화 후 타이틀 씬 언로드, 플레이어씬 및 스테이지1 로드
    public void GameStart()
    {
        loadingScreen.SetActive(true);
        SceneManager.UnloadSceneAsync((int)SceneIndex.TITLE);
        SceneManager.LoadScene((int)SceneIndex.PLAYER_SCENE, LoadSceneMode.Additive);
        StartCoroutine(Loading((int)SceneIndex.STAGE_1));
    }

    //스테이지를 넘어갈 경우 curIdx 갱신 및 해당 stageIdx의 씬 로드, 현재 씬 언로드
    public void EnterStage(int stageIdx)
    {
        if (stageIdx == curIdx) return;

        loadingScreen.SetActive(true);
        SceneManager.UnloadSceneAsync(curIdx);
        StartCoroutine(Loading(stageIdx));
        curIdx = stageIdx;
    }

    //입장하는 스테이지를 LoadSceneAsync로 로딩하며 AsyncOperation 변수에 저장
    //로딩 작업이 완료될때까지 대기한 후 해당 스테이지를 액티브 씬으로 설정, 이후 오브젝트 생성
    IEnumerator Loading(int idx)
    {
        loadingScreen.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(idx, LoadSceneMode.Additive);
        //로딩 완료될때까지 대기
        while (!op.isDone)
        {
            yield return interval;
        }
        yield return new WaitForSeconds(0.5f); //로딩 화면 체크용 추가 시간, 나중에 삭제할 것
        loadingScreen.SetActive(false);

        if (idx == 3) loadingCamera.SetActive(false);
        if (idx >= 3) yield break;
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(idx));
        ObjectManager.loadObjects(idx);
    }
}