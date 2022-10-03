using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    // ���� �Ŵ��� ��ũ��Ʈ 
    public static MonsterManager Instance; // �̱���

    public Transform[] spawnPoint; // ���� ���� ����Ʈ
    private int[] spawn_Save;
    private int spawnDivide;
    [SerializeField] private GameObject monsterPrefab; // ���� ������
    [SerializeField] private int monsterCount; // ������ ���� �� 

    [SerializeField] Queue<Monster> monsterQueue = new Queue<Monster>(); // ������Ʈ Ǯ ( ť )

    private void Awake()
    {
        Instance = this;

        Initialize(monsterCount); // ������ ���� ���� �� ��ŭ ��������.

        spawn_Save = new int[spawnPoint.Length]; 

        spawnDivide = monsterCount / spawnPoint.Length;
    }

    void Initialize(int initCount) // �����Լ�.
    {
        for (int i = 0; i < initCount; i++)
        {
            monsterQueue.Enqueue(CreateNewObject()); // ������Ʈ ���� �� ť�� �־���.
        }
    }

    Monster CreateNewObject() // ���͸� ���� ���� �� ��������.
    {
        var newObj = Instantiate(monsterPrefab).GetComponent<Monster>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }
    public Monster GetObject() // ���͸� ������Ʈ Ǯ(ť)���� ������ ��������.
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

    public static void ReturnObject(Monster obj) // ������Ʈ Ǯ(ť)�� �ٽ� �־��ֱ� ���� �Լ�.
    {

        obj.gameObject.SetActive(false);
        Instance.monsterQueue.Enqueue(obj);
    }

    IEnumerator Spawn_Time() // ��ȯ�ϱ� ���� �ڷ�ƾ.
    {
        yield return new WaitForSeconds(3f);

        spawn_possible = true; // bool ������ true�� �ٲ��� 
    }

    public bool spawn_possible = true;
    void Update_Spawn()
    {
        if (monsterQueue.Count > 0 && spawn_possible) // ��Ȱ��ȭ �� ���Ͱ� �ִٸ�, ��ȯ�� �� �ִٸ� 
        {
            Monster monster = GetObject(); // ���͸� �޾ƿ�.

            if (monster != null)
            {
                if (monster.isDie) // �׾��ִ� ���� ���ٸ� �ٽ� �ʱ�ȭ.
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
