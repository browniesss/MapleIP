using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    // 몬스터 매니저 스크립트 
    public static MonsterManager Instance; // 싱글톤

    public Transform[] spawnPoint; // 몬스터 스폰 포인트
    private int[] spawn_Save;
    private int spawnDivide;
    [SerializeField] private GameObject monsterPrefab; // 몬스터 프리팹
    [SerializeField] private int monsterCount; // 생성할 몬스터 수 

    [SerializeField] Queue<Monster> monsterQueue = new Queue<Monster>(); // 오브젝트 풀 ( 큐 )

    private void Awake()
    {
        Instance = this;

        Initialize(monsterCount); // 저장해 놓은 몬스터 수 만큼 생성해줌.

        spawn_Save = new int[spawnPoint.Length]; 

        spawnDivide = monsterCount / spawnPoint.Length;
    }

    void Initialize(int initCount) // 생성함수.
    {
        for (int i = 0; i < initCount; i++)
        {
            monsterQueue.Enqueue(CreateNewObject()); // 오브젝트 생성 후 큐에 넣어줌.
        }
    }

    Monster CreateNewObject() // 몬스터를 새로 생성 후 리턴해줌.
    {
        var newObj = Instantiate(monsterPrefab).GetComponent<Monster>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }
    public Monster GetObject() // 몬스터를 오브젝트 풀(큐)에서 꺼내와 리턴해줌.
    {
        if (monsterQueue.Count > 0)
        {
            var obj = monsterQueue.Dequeue();
            int rand_Spawn_Point = Random.Range(0, spawnPoint.Length); // 0 ~ 2

            obj.transform.position = spawnPoint[rand_Spawn_Point].position;

            obj.gameObject.SetActive(true);

            return obj;
        }
        else
        {
            return null;
        }
    }

    public static void ReturnObject(Monster obj) // 오브젝트 풀(큐)에 다시 넣어주기 위한 함수.
    {

        obj.gameObject.SetActive(false);
        Instance.monsterQueue.Enqueue(obj);
    }

    IEnumerator Spawn_Time() // 소환하기 위한 코루틴.
    {
        yield return new WaitForSeconds(3f);

        spawn_possible = true; // bool 변수를 true로 바꿔줌 
    }

    public bool spawn_possible = true;
    void Update_Spawn()
    {
        if (monsterQueue.Count > 0 && spawn_possible) // 비활성화 된 몬스터가 있다면, 소환할 수 있다면 
        {
            Monster monster = GetObject(); // 몬스터를 받아옴.

            if (monster != null)
            {
                if (monster.isDie) // 죽어있던 몬스터 였다면 다시 초기화.
                    monster.ReInit();
            }

            spawn_possible = false;

            StartCoroutine(Spawn_Time());
        }
    }

    void Start()
    {

    }

    void Update()
    {
        Update_Spawn();
    }
}
