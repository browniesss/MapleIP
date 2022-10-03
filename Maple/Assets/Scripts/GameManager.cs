using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 게임 매니저 스크립트.

    public Image revive_Image = null; // 부활 이미지
    public Image fadePanel = null; // 화면 페이드인, 페이드아웃 해주는 이미지
    public TalkManager talkManager = null; 
    public QuestManager questManager = null;
    public Player player = null;
    public GameObject talkPanel; // 대화창 이미지
    public Image portraitImg; // 대화창 초상화 이미지
    public Text talkText; // 대화 내용 텍스트
    public Text npcName;  // 대화 상대 NPC 이름 텍스트
    public Text questName_text; // 퀘스트 이름 텍스트
    public Text level_text; // 레벨 텍스트 
    public GameObject scanObject; // 현재 충돌 , 스캔된 GameObject

    public Transform[] portalPos; // 0번은 왼쪽 포탈 1번은 오른쪽 포탈

    public int talkIndex;
    public bool isAction; // 대화를 하고있는지 체크해 줄 변수.

    bool fadeCheck = true; // fade - in, fade - out 할지 판별할 bool 변수

    void Awake()
    {

    }

    public void Revive_Player() // 플레이어를 되 살려주는 함수. - 부활 버튼을 눌렀을때
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 씬을 다시 로드함.
    }

    void LoadInfo() // 씬이 바뀌었을때 플레이어의 좌표를 재 설정해주고 퀘스트 정보를 받아올 함수.
    {
        // 이전 맵(씬), 다음 맵(씬)에서 왔는지 검사 후 위치 변경.
        if (player.cur_MapMoving == 1)
        {
            player.transform.position = new Vector3(portalPos[0].position.x + 1, portalPos[0].position.y + 1, portalPos[0].position.z);
        }
        else if (player.cur_MapMoving == 2)
        {
            player.transform.position = new Vector3(portalPos[1].position.x + 1, portalPos[1].position.y + 1, portalPos[1].position.z);
        }

        questName_text.text = player.cur_QuestName;
    }


    void Start()
    {
        questManager.CheckQuest();

        LoadInfo();

        fadePanel.color = new Color(0, 0, 0, 1); // 페이드 해주는 이미지 알파값을 변경.
    }

    

    public void Action(GameObject scanObj) // 플레이어가 호출하는 함수.
    {
        scanObject = scanObj; // 스캔 오브젝트를 넣어준 후 
        npcName.text = scanObject.name; // npc 이름을 보여주는 텍스트를 바꿔주고

        if (scanObject.tag == "Npc") // NPC 였다면.
        {
            ObjData objdata = scanObject.GetComponent<ObjData>(); // NPC의 데이터를 가져와서 
            Talk(objdata.id); // Talk 함수를 호출.
        }

        talkPanel.SetActive(isAction); // 대화창 이미지 활성화.
    }

    void Talk(int id) // NPC의 아이디를 받아와서
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id); // 대화 인덱스를 받아옴.

        string talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex); // 대화 내용을 가져옴.

        if(talkData==null) // 대화 내용이 없다면 .
        {
            // 초기화.
            player.scanObject = null;
            isAction = false;
            talkIndex = 0;
            questManager.CheckQuest(id);
            return;
        } 

        questName_text.text = questManager.CheckQuest(); // 퀘스트 이름을 받아와서 넣어줌.
        player.cur_QuestName = questName_text.text; // 플레이어 정보에도 저장.

        talkText.text = talkData.Split(':')[0]; // Split 함수로 구분자 : 를 사용해 배열로 만든뒤 0 번째 즉 , 문자열을 가져옴.

        portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
        // Split 함수로 구분자 : 를 사용해 배열로 만들어 1번째 문자열을 int.Parse 로 형변환을 통해 인덱스를 받아옴.
        portraitImg.color = new Color(1, 1, 1, 1); // 이미지의 Alpha 값을 1로 함.

        isAction = true;
        talkIndex++;
    }

    void Fade_Out() // 페이드 아웃 함수.
    {
        if (!fadeCheck)
            return;

        Color color = fadePanel.color; // color 에 판넬 이미지를 받아옴.

        for (int i = 0; i <= 255; i++) // for문 255번 반복 0보다 작을 때 까지
        {
            color.a -= Time.deltaTime * 0.01f; // 이미지 알파 값을 타임 델타 값 * 0.01

            fadePanel.color = color; // 판넬 이미지 컬러에 바뀐 알파값 참조

            if (fadePanel.color.a <= 0) // 만약 판넬 이미지 알파 값이 0보다 작으면
            {
                fadeCheck = false; // checkbool 참 
                fadePanel.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        Fade_Out();
    }
}
