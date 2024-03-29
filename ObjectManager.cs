﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static Func<string, GameObject> makeObj;
    public static Action<int> loadObjects;

    [SerializeField] UiManager uiManager;

    // Objects
    [Header("오브젝트")]
    [SerializeField] GameObject mushroomPrefab;
    [SerializeField] GameObject pineTreePrefab;

    //npcs
    [Header("NPC")]
    [SerializeField] GameObject misakiPrefab;
    [SerializeField] GameObject yukoPrefab;

    //enemies
    [Header("몬스터")]
    [SerializeField] GameObject enemyPeacefulPrefab;
    [SerializeField] GameObject enemyNormalPrefab;
    [SerializeField] GameObject enemyAggressivePrefab;

    //UI
    [Header("UI")]
    [SerializeField] GameObject hpBarPrefab;
    [SerializeField] Canvas minorCanvas;

    GameObject[] mushroom;
    GameObject[] pineTree;
    GameObject[] misaki;
    GameObject[] yuko;
    GameObject[] enemyPeaceful;
    GameObject[] enemyNormal;
    GameObject[] enemyAggressive;
    GameObject[] hpBar;
    Npc[] npcDatas;

    GameObject[] targetPool;
    Dictionary<int, string[]> npcDataDic;

    void Awake()
    {
        makeObj = (a) => { return MakeObj(a); };
        loadObjects = (a) => {  Generate(); LoadObjects(a); };

        mushroom = new GameObject[30];
        pineTree = new GameObject[30];

        misaki = new GameObject[5];
        yuko = new GameObject[5];
        npcDatas = new Npc[misaki.Length + yuko.Length];

        enemyPeaceful = new GameObject[30];
        enemyNormal = new GameObject[30];
        enemyAggressive = new GameObject[30];

        hpBar = new GameObject[30];

        npcDataDic = new Dictionary<int, string[]>();
        LoadNpcData();
    }

    void LoadNpcData()
    {
        // npc 데이터 세팅
        TextAsset npcData = Resources.Load("npcData") as TextAsset;
        StringReader npcDataReader = new StringReader(npcData.text);

        while (npcDataReader != null)
        {
            string line = npcDataReader.ReadLine();
            if (line == null) break;

            line = npcDataReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                // 0 : id, 1 : name, 2 : hasShop(상점 보유 여부)
                npcDataDic.Add(int.Parse(datas[0]), new string[] { datas[1], datas[2] });

                line = npcDataReader.ReadLine();
                if (line == null) break;
            }
        }
        npcDataReader.Close();
    }

    // 모든 프리팹들을 Instantiate 한 후 배열에 할당
    // 최초 비활성화, 이 후 MakeObj 함수로 활성화
    void Generate()
    {
        //npcs
        int npcIdx = 0;
        for (int idx = 0; idx < misaki.Length; idx++)
        {
            misaki[idx] = Instantiate(misakiPrefab);
            misaki[idx].SetActive(false);
            npcDatas[idx] = misaki[idx].GetComponent<Npc>();
        }
        npcIdx += misaki.Length;
        for (int idx = 0; idx < yuko.Length; idx++)
        {
            yuko[idx] = Instantiate(yukoPrefab);
            yuko[idx].SetActive(false);
            npcDatas[npcIdx] = yuko[idx].GetComponent<Npc>();
            npcIdx++;
        }
        //UI
        for (int idx = 0; idx < hpBar.Length; idx++)
        {
            hpBar[idx] = Instantiate(hpBarPrefab, minorCanvas.transform);
            hpBar[idx].SetActive(false);
        }
        
        //마스터 클라이언트가 아니라면 이하 오브젝트는 생성x
        if(!NetworkManager.Inst.IsMaster()) return;

        //objects
        for (int idx = 0; idx < mushroom.Length; idx++)
        {
            mushroom[idx] = NetworkManager.Inst.Instantiate(ObjectNames.mushroom);
            mushroom[idx].SetActive(false);
        }

        //enemies
        int peacefulId = enemyPeacefulPrefab.GetComponent<Enemy>().EnemyId;
        for (int idx = 0; idx < enemyPeaceful.Length; idx++)
        {
            enemyPeaceful[idx] = Instantiate(enemyPeacefulPrefab);
            //initialize
            enemyPeaceful[idx].GetComponent<Enemy>().Initialize(uiManager.GetEnemyData(peacefulId));
            enemyPeaceful[idx].SetActive(false);
        }
        int normalId = enemyNormalPrefab.GetComponent<Enemy>().EnemyId;
        for (int idx = 0; idx < enemyNormal.Length; idx++)
        {
            enemyNormal[idx] = Instantiate(enemyNormalPrefab);
            //initialize
            enemyNormal[idx].GetComponent<Enemy>().Initialize(uiManager.GetEnemyData(normalId));
            enemyNormal[idx].SetActive(false);
        }
        int aggressiveId = enemyAggressivePrefab.GetComponent<Enemy>().EnemyId;
        for (int idx = 0; idx < enemyAggressive.Length; idx++)
        {
            enemyAggressive[idx] = Instantiate(enemyAggressivePrefab);
            //initialize
            enemyAggressive[idx].GetComponent<Enemy>().Initialize(uiManager.GetEnemyData(aggressiveId));
            enemyAggressive[idx].SetActive(false);
        }
    }

    // stageTable.csv 파일을 읽어들여서 해당 스테이지의 오브젝트 배치
    void LoadObjects(int stageIdx)
    {
        if(!NetworkManager.Inst.IsMaster()) return;

        TextAsset stageTable = Resources.Load("stageTable") as TextAsset;
        StringReader reader = new StringReader(stageTable.text);

        string line = reader.ReadLine();
        while (line != stageIdx.ToString())
        {
            line = reader.ReadLine();
            if (line == null) break;
        }

        line = reader.ReadLine();
        while (line.Length > 1)
        {
            string[] datas = line.Split(',');
            // 0 : 오브젝트 이름, 1, 2, 3: x좌표, y좌표, z좌표
            GameObject obj = MakeObj(datas[0]);
            obj.transform.position = new Vector3(float.Parse(datas[1]), float.Parse(datas[2]), float.Parse(datas[3]));

            line = reader.ReadLine();
            if (line == null) break;
        }
        reader.Close();
    }

    // 오브젝트 이름을 string 형태로 받아 활성화 후 반환
    // 매개변수는 ObjectNames 클래스를 이용
    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case ObjectNames.mushroom:
                targetPool = mushroom;
                break;
            case ObjectNames.enemyPeaceful:
                targetPool = enemyPeaceful;
                break;
            case ObjectNames.enemyNormal:
                targetPool = enemyNormal;
                break;
            case ObjectNames.enemyAggressive:
                targetPool = enemyAggressive;
                break;
            case ObjectNames.hpBar:
                targetPool = hpBar;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (!targetPool[idx].activeSelf)
            {
                targetPool[idx].SetActive(true);
                return targetPool[idx];
            }
        }
        return null;
    }

    // 오브젝트 이름을 string 형태로 받아 해당하는 오브젝트 전체 풀을 반환
    // 매개변수는 ObjectNames 클래스를 이용
    public GameObject[] GetPool(string type)
    {
        switch (type)
        {
            case ObjectNames.mushroom:
                targetPool = mushroom;
                break;
            case ObjectNames.pineTree:
                targetPool = pineTree;
                break;
            case ObjectNames.misaki:
                targetPool = misaki;
                break;
            case ObjectNames.yuko:
                targetPool = yuko;
                break;
            case ObjectNames.enemyPeaceful:
                targetPool = enemyPeaceful;
                break;
            case ObjectNames.enemyNormal:
                targetPool = enemyNormal;
                break;
            case ObjectNames.enemyAggressive:
                targetPool = enemyAggressive;
                break;
            case ObjectNames.hpBar:
                targetPool = hpBar;
                break;
        }
        return targetPool;
    }

    // 오브젝트 이름을 string 형태로 받아 해당하는 오브젝트 중 하나를 반환(0 인덱스)
    // 매개변수는 ObjectNames 클래스를 이용
    public GameObject GetOne(string type)
    {
        switch (type)
        {
            case ObjectNames.mushroom:
                targetPool = mushroom;
                break;
            case ObjectNames.pineTree:
                targetPool = pineTree;
                break;
            case ObjectNames.misaki:
                targetPool = misaki;
                break;
            case ObjectNames.yuko:
                targetPool = yuko;
                break;
            case ObjectNames.enemyPeaceful:
                targetPool = enemyPeaceful;
                break;
            case ObjectNames.enemyNormal:
                targetPool = enemyNormal;
                break;
            case ObjectNames.enemyAggressive:
                targetPool = enemyAggressive;
                break;
            case ObjectNames.hpBar:
                targetPool = hpBar;
                break;
        }
        return targetPool[0];
    }

    // 오브젝트 이름을 string 형태로 받아 해당하는 오브젝트 중 현재 활성화된 개수를 반환
    // 매개변수는 ObjectNames 클래스를 이용
    public int GetActiveCount(string type)
    {
        int count = 0;

        switch (type)
        {
            case ObjectNames.mushroom:
                targetPool = mushroom;
                break;
            case ObjectNames.pineTree:
                targetPool = pineTree;
                break;
            case ObjectNames.misaki:
                targetPool = misaki;
                break;
            case ObjectNames.yuko:
                targetPool = yuko;
                break;
            case ObjectNames.enemyPeaceful:
                targetPool = enemyPeaceful;
                break;
            case ObjectNames.enemyNormal:
                targetPool = enemyNormal;
                break;
            case ObjectNames.enemyAggressive:
                targetPool = enemyAggressive;
                break;
            case ObjectNames.hpBar:
                targetPool = hpBar;
                break;
        }

        for (int idx = 0; idx < targetPool.Length; idx++)
        {
            if (targetPool[idx].activeSelf)
            {
                count++;
            }
        }
        return count;
    }
}