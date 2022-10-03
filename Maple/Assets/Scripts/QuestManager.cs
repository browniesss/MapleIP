using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // ����Ʈ �Ŵ��� ��ũ��Ʈ
    public int questId; // ����Ʈ id 
    public int questActionIndex; // ����Ʈ ��ȭ index

    Dictionary<int, QuestData> questList; // ����Ʈ ����Ʈ Dictionary

    Player player;

    public Item[] questItem; // ����Ʈ ���� �����۵�.

    void Awake()
    {
        player = FindObjectOfType<Player>();

        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    private void Start()
    {
        UpdateSaved();
    }

    void UpdateSaved() // �� �̵� �� ���� �������� ����Ʈ�� �޾ƿ�.
    {
        questId = player.cur_QuestID;
        questActionIndex = player.cur_QuestIndex;
    }

    void GenerateData() // ������ ����
    {
        questList.Add(10, new QuestData("�κ��丮�� ���� ��� �Ա�",
                                        new int[] { 1000, 1000, 1000 })); // �迭�� Length = 3

        questList.Add(20, new QuestData("�ִϾ� ���溼 5���� ����ϱ�",
                                        new int[] { 1000, 1000, 1000 })); // �迭�� Length = 3

        questList.Add(30, new QuestData("�����Ӱ� �����ϱ�",
                                       new int[] { 10000, 10000, 10000 })); // �迭�� Length = 3
    }

    public int GetQuestTalkIndex(int id) // ����Ʈ �ε����� ������ �Լ�
    {
        if (questActionIndex == questList[questId].npcId.Length - 2) // ����Ʈ �ε����� ������ 2�� ��, �� ����Ʈ�� ���� �� �Ϸ�����
                                                                     // ���� �ϷḦ ���ߴ��� �Ǻ�.
        {
            if (CheckNextQuest())
            {
                questActionIndex = questList[questId].npcId.Length - 1;
                return questId + questActionIndex;
            }
            else
            {
                questActionIndex = questList[questId].npcId.Length - 2;
                return questId + questActionIndex;
            }
        }

        return questId + questActionIndex; // ���� ����Ʈ ID + ����Ʈ INDEX �� ��ȯ����.
    }

    public string CheckQuest(int id) // npcId�� �޾ƿͼ� 
    {
        ControlObject();  // ����Ʈ ������ ��Ʈ��

        if (!questList.ContainsKey(questId)) // �ش� ���̵� �������� �ʴ´ٸ� return 
            return null;

        // ����Ʈ id �� id�� ��ġ�ϰ� �ִ� Index���� ��������
        if (id == questList[questId].npcId[questActionIndex]
            && questActionIndex < questList[questId].npcId.Length - 2) // questList[����ƮID].npcId[����Ʈ�����ε���]
        {
            questActionIndex++;
            player.cur_QuestIndex = questActionIndex;
        }

        if (questActionIndex == questList[questId].npcId.Length - 1) // �ε����� ���� ���̵��� ���� - 1 �� ��ġ�ߴٸ�
        { 
            NextQuest(); // ���� ����Ʈ ����
        }

        if (questList.ContainsKey(questId))
            return questList[questId].questName;
        else
            return null;
    }

    public string CheckQuest() // �Լ� �����ε�
    {
        if (!questList.ContainsKey(questId))
            return null;

        return questList[questId].questName;
    }

    public bool CheckNextQuest() // ���� ����Ʈ�� �Ѿ�� �Ǵ��� üũ���� �Լ�
    {
        switch (questId)
        {
            case 10: // ���� ����Ա� ����Ʈ
                for (int i = 0; i < player.haveItem.Length; i++)
                {
                    if (player.haveItem[i] == questItem[0].itemCode) // ����� ������ �ִٸ�
                    {
                        return false;
                    }
                }
                return true; // ���� ���ǿ��� �Ȱɷȴٸ� true�� ��ȯ�ؼ� ���� ����Ʈ�� �Ѱ���.
            case 20: // ���� �ִϾ� ���溼 ��� ����Ʈ
                if (player.catchCount <= 4) // 4���� ���� �ۿ� �� ��Ҵٸ� 
                {
                    return false;
                }
                return true;
        }
        return false;
    }

    void NextQuest() // ��������Ʈ�� �Ѿ �Լ�
    {
        questId += 10;
        questActionIndex = 0;

        // �÷��̾�Ե� �ش� ������ ���� �� ��
        player.cur_QuestID = questId;
        player.cur_QuestIndex = questActionIndex;
    }

    void ControlObject() // ����Ʈ �������� ��Ʈ�� ���� �Լ�
    {
        switch (questId + questActionIndex)
        {
            case 10:
                player.inventory.AcquireItem(questItem[0]);
                break;
        }
    }
}
