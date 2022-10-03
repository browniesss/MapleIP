using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // 퀘스트 매니저 스크립트
    public int questId; // 퀘스트 id 
    public int questActionIndex; // 퀘스트 대화 index

    Dictionary<int, QuestData> questList; // 퀘스트 리스트 Dictionary

    Player player;

    public Item[] questItem; // 퀘스트 진행 아이템들.

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

    void UpdateSaved() // 씬 이동 시 현재 진행중인 퀘스트를 받아옴.
    {
        questId = player.cur_QuestID;
        questActionIndex = player.cur_QuestIndex;
    }

    void GenerateData() // 데이터 생성
    {
        questList.Add(10, new QuestData("인벤토리를 열어 사과 먹기",
                                        new int[] { 1000, 1000, 1000 })); // 배열의 Length = 3

        questList.Add(20, new QuestData("주니어 스톤볼 5마리 사냥하기",
                                        new int[] { 1000, 1000, 1000 })); // 배열의 Length = 3

        questList.Add(30, new QuestData("자유롭게 여행하기",
                                       new int[] { 10000, 10000, 10000 })); // 배열의 Length = 3
    }

    public int GetQuestTalkIndex(int id) // 퀘스트 인덱스를 얻어오는 함수
    {
        if (questActionIndex == questList[questId].npcId.Length - 2) // 퀘스트 인덱스가 끝나기 2개 전, 즉 퀘스트를 수락 후 완료인지
                                                                     // 아직 완료를 못했는지 판별.
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

        return questId + questActionIndex; // 현재 퀘스트 ID + 퀘스트 INDEX 를 반환해줌.
    }

    public string CheckQuest(int id) // npcId를 받아와서 
    {
        ControlObject();  // 퀘스트 아이템 컨트롤

        if (!questList.ContainsKey(questId)) // 해당 아이디를 포함하지 않는다면 return 
            return null;

        // 퀘스트 id 와 id가 일치하고 최대 Index보다 적을때만
        if (id == questList[questId].npcId[questActionIndex]
            && questActionIndex < questList[questId].npcId.Length - 2) // questList[퀘스트ID].npcId[퀘스트진행인덱스]
        {
            questActionIndex++;
            player.cur_QuestIndex = questActionIndex;
        }

        if (questActionIndex == questList[questId].npcId.Length - 1) // 인덱스가 현재 아이디의 길이 - 1 과 일치했다면
        { 
            NextQuest(); // 다음 퀘스트 진행
        }

        if (questList.ContainsKey(questId))
            return questList[questId].questName;
        else
            return null;
    }

    public string CheckQuest() // 함수 오버로딩
    {
        if (!questList.ContainsKey(questId))
            return null;

        return questList[questId].questName;
    }

    public bool CheckNextQuest() // 다음 퀘스트로 넘어가도 되는지 체크해줄 함수
    {
        switch (questId)
        {
            case 10: // 마이 사과먹기 퀘스트
                for (int i = 0; i < player.haveItem.Length; i++)
                {
                    if (player.haveItem[i] == questItem[0].itemCode) // 사과를 가지고 있다면
                    {
                        return false;
                    }
                }
                return true; // 위의 조건에서 안걸렸다면 true를 반환해서 다음 퀘스트로 넘겨줌.
            case 20: // 마이 주니어 스톤볼 잡기 퀘스트
                if (player.catchCount <= 4) // 4마리 까지 밖에 못 잡았다면 
                {
                    return false;
                }
                return true;
        }
        return false;
    }

    void NextQuest() // 다음퀘스트로 넘어갈 함수
    {
        questId += 10;
        questActionIndex = 0;

        // 플레이어에게도 해당 정보를 저장 해 줌
        player.cur_QuestID = questId;
        player.cur_QuestIndex = questActionIndex;
    }

    void ControlObject() // 퀘스트 아이템을 컨트롤 해줄 함수
    {
        switch (questId + questActionIndex)
        {
            case 10:
                player.inventory.AcquireItem(questItem[0]);
                break;
        }
    }
}
