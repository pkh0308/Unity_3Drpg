using System.IO;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    // Objects
    [SerializeField] GameObject mushroomPrefab;
    [SerializeField] GameObject pineTreePrefab;

    GameObject[] mushroom;
    GameObject[] pineTree;
    GameObject[] targetPool;

    void Awake()
    {
        mushroom = new GameObject[30];
        pineTree = new GameObject[30];

        Generate();
        SetPosition();
    }

    // 모든 프리팹들을 Instantiate 한 후 배열에 할당
    // 최초 비활성화, 이 후 MakeObj 함수로 활성화
    void Generate()
    {
        for (int idx = 0; idx < mushroom.Length; idx++)
        {
            mushroom[idx] = Instantiate(mushroomPrefab);
            mushroom[idx].SetActive(false);
        }

        for (int idx = 0; idx < mushroom.Length; idx++)
        {
            pineTree[idx] = Instantiate(pineTreePrefab);
            pineTree[idx].SetActive(false);
        }
    }

    // stageTable.csv 파일을 읽어들여서 해당 스테이지의 오브젝트 배치
    void SetPosition()
    {
        TextAsset stageTable = Resources.Load("stageTable") as TextAsset;
        StringReader reader = new StringReader(stageTable.text);

        string line = reader.ReadLine();
        while (line != gameManager.StageIdx.ToString())
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
            case ObjectNames.pineTree:
                targetPool = pineTree;
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
