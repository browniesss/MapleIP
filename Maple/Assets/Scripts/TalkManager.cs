using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // 대화 매니저 스크립트 
    Dictionary<int, string[]> talkData; // 대화 내용 Dictionary
    Dictionary<int, Sprite> portraitData; // 초상화 Dictionary

    public Sprite[] portraitArr; // 초상화 배열

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData() // 대화 내용 생성
    {
        // Normal Talk
        talkData.Add(1000, new string[] {  "어서 와, 메이플 아일랜드는 처음이지?:0", "기초가 중요해 !:0",
            "어떤 강한 모험가도 처음은 기초부터 시작하는 거야:0" ,"숙련되면 눈 감고도 할 수 있게 될 거야:0" }); // 마이 

        talkData.Add(2000, new string[] {  "내 이름은 루카스, 암허스트의 촌장이라네.:0","수많은 모험가들이 메이플 아일랜드를 거쳐갔지.:0",
            "메이플 아일랜드에 온 것을 환영하네.:0","아직 모험을 떠날 자신이 없다면, 마이의 수련장에 들러보는 것도 괜찮다네.:0" }); // 루카스

        talkData.Add(3000, new string[] { "우아앙~ 내 아름다운 피부가~:0" }); // 히나 

        talkData.Add(4000, new string[] { "원래 저긴 내 자린데..:0", "언젠간..:0" }); // 장로 스탄 

        // Quest Talk
        talkData.Add(10 + 1000, new string[] { "어서 와 ! 환영의 의미로 사과를 하나 줄테니까 먹어 봐 !:0", "사과는 I 키를 눌러 인벤토리를 열어보면 있을거야 !:0" }); // 마이 
        talkData.Add(11 + 1000, new string[] { "내가 준 사과를 전부 먹으라니까 그러네~ 아이템 창을 열어봐.:0",
            "사과가 있을테니 더블클릭해서 먹으면 돼.:0" }); // 마이 
        talkData.Add(12 + 1000, new string[] { "잘 했어 ! 인벤토리에서 아이템 사용은 그렇게 하는거야 !:0",
            "오른쪽 포탈을 타고 다음 맵으로 이동하면 돼 !:0" }); // 마이 

        talkData.Add(20 + 1000, new string[] { "이번에는 몬스터를 사냥하는 방법을 배워볼까 ?:0", "좌측 키보드 Control키를 눌러서 공격을 할 수 있어 !:0",
        "자 그러면 오른쪽에 있는 주니어 스톤볼을 5마리만 잡아볼래 ?:0","다 잡고 다시 말 걸어줘 !:0"}); // 마이 
        talkData.Add(21 + 1000, new string[] { "아직 5마리를 못 잡은거 같은데 ?:0", "오른쪽의 주니어 스톤볼을 5마리 잡고 말 걸어줘:0" }); // 마이 
        talkData.Add(22 + 1000, new string[] { "좋아 이정도면 충분히 익힌거 같네 !:0", "이번에도 오른쪽 포탈을 타고 다음 맵으로 넘어가면 돼!:0" }); // 마이

        portraitData.Add(1000 + 0, portraitArr[0]); // 마이 초상화
        portraitData.Add(2000 + 0, portraitArr[1]); // 루카스 초상화
        portraitData.Add(3000 + 0, portraitArr[2]); // 히나 초상화
        portraitData.Add(4000 + 0, portraitArr[3]); // 히나 초상화
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id)) // 만약 talkData Dictionary 안에 해당하는 Key 값이 없다면 
        {   // 해당 퀘스트 진행 순서 대사가 없을 때. 
            if (!talkData.ContainsKey(id - id % 10))
            {
                return GetTalk(id - id % 100, talkIndex);
            }
            // 퀘스트 맨 처음 대사마저 없을 때 기본 대사를 출력.
            else
            {
                return GetTalk(id - id % 10, talkIndex);
            }
        }

        if (talkIndex == talkData[id].Length) // 대화의 문장 갯수가 인덱스와 같다면
            return null;
        else
            return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
