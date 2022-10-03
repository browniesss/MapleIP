using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ���� �Ŵ��� ��ũ��Ʈ.

    public Image revive_Image = null; // ��Ȱ �̹���
    public Image fadePanel = null; // ȭ�� ���̵���, ���̵�ƿ� ���ִ� �̹���
    public TalkManager talkManager = null; 
    public QuestManager questManager = null;
    public Player player = null;
    public GameObject talkPanel; // ��ȭâ �̹���
    public Image portraitImg; // ��ȭâ �ʻ�ȭ �̹���
    public Text talkText; // ��ȭ ���� �ؽ�Ʈ
    public Text npcName;  // ��ȭ ��� NPC �̸� �ؽ�Ʈ
    public Text questName_text; // ����Ʈ �̸� �ؽ�Ʈ
    public Text level_text; // ���� �ؽ�Ʈ 
    public GameObject scanObject; // ���� �浹 , ��ĵ�� GameObject

    public Transform[] portalPos; // 0���� ���� ��Ż 1���� ������ ��Ż

    public int talkIndex;
    public bool isAction; // ��ȭ�� �ϰ��ִ��� üũ�� �� ����.

    bool fadeCheck = true; // fade - in, fade - out ���� �Ǻ��� bool ����

    void Awake()
    {

    }

    public void Revive_Player() // �÷��̾ �� ����ִ� �Լ�. - ��Ȱ ��ư�� ��������
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // ���� �ٽ� �ε���.
    }

    void LoadInfo() // ���� �ٲ������ �÷��̾��� ��ǥ�� �� �������ְ� ����Ʈ ������ �޾ƿ� �Լ�.
    {
        // ���� ��(��), ���� ��(��)���� �Դ��� �˻� �� ��ġ ����.
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

        fadePanel.color = new Color(0, 0, 0, 1); // ���̵� ���ִ� �̹��� ���İ��� ����.
    }

    

    public void Action(GameObject scanObj) // �÷��̾ ȣ���ϴ� �Լ�.
    {
        scanObject = scanObj; // ��ĵ ������Ʈ�� �־��� �� 
        npcName.text = scanObject.name; // npc �̸��� �����ִ� �ؽ�Ʈ�� �ٲ��ְ�

        if (scanObject.tag == "Npc") // NPC ���ٸ�.
        {
            ObjData objdata = scanObject.GetComponent<ObjData>(); // NPC�� �����͸� �����ͼ� 
            Talk(objdata.id); // Talk �Լ��� ȣ��.
        }

        talkPanel.SetActive(isAction); // ��ȭâ �̹��� Ȱ��ȭ.
    }

    void Talk(int id) // NPC�� ���̵� �޾ƿͼ�
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id); // ��ȭ �ε����� �޾ƿ�.

        string talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex); // ��ȭ ������ ������.

        if(talkData==null) // ��ȭ ������ ���ٸ� .
        {
            // �ʱ�ȭ.
            player.scanObject = null;
            isAction = false;
            talkIndex = 0;
            questManager.CheckQuest(id);
            return;
        } 

        questName_text.text = questManager.CheckQuest(); // ����Ʈ �̸��� �޾ƿͼ� �־���.
        player.cur_QuestName = questName_text.text; // �÷��̾� �������� ����.

        talkText.text = talkData.Split(':')[0]; // Split �Լ��� ������ : �� ����� �迭�� ����� 0 ��° �� , ���ڿ��� ������.

        portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
        // Split �Լ��� ������ : �� ����� �迭�� ����� 1��° ���ڿ��� int.Parse �� ����ȯ�� ���� �ε����� �޾ƿ�.
        portraitImg.color = new Color(1, 1, 1, 1); // �̹����� Alpha ���� 1�� ��.

        isAction = true;
        talkIndex++;
    }

    void Fade_Out() // ���̵� �ƿ� �Լ�.
    {
        if (!fadeCheck)
            return;

        Color color = fadePanel.color; // color �� �ǳ� �̹����� �޾ƿ�.

        for (int i = 0; i <= 255; i++) // for�� 255�� �ݺ� 0���� ���� �� ����
        {
            color.a -= Time.deltaTime * 0.01f; // �̹��� ���� ���� Ÿ�� ��Ÿ �� * 0.01

            fadePanel.color = color; // �ǳ� �̹��� �÷��� �ٲ� ���İ� ����

            if (fadePanel.color.a <= 0) // ���� �ǳ� �̹��� ���� ���� 0���� ������
            {
                fadeCheck = false; // checkbool �� 
                fadePanel.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        Fade_Out();
    }
}
