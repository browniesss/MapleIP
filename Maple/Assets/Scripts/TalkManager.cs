using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    // ��ȭ �Ŵ��� ��ũ��Ʈ 
    Dictionary<int, string[]> talkData; // ��ȭ ���� Dictionary
    Dictionary<int, Sprite> portraitData; // �ʻ�ȭ Dictionary

    public Sprite[] portraitArr; // �ʻ�ȭ �迭

    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData() // ��ȭ ���� ����
    {
        // Normal Talk
        talkData.Add(1000, new string[] {  "� ��, ������ ���Ϸ���� ó������?:0", "���ʰ� �߿��� !:0",
            "� ���� ���谡�� ó���� ���ʺ��� �����ϴ� �ž�:0" ,"���õǸ� �� ���� �� �� �ְ� �� �ž�:0" }); // ���� 

        talkData.Add(2000, new string[] {  "�� �̸��� ��ī��, ���㽺Ʈ�� �����̶��.:0","������ ���谡���� ������ ���Ϸ��带 ���İ���.:0",
            "������ ���Ϸ��忡 �� ���� ȯ���ϳ�.:0","���� ������ ���� �ڽ��� ���ٸ�, ������ �����忡 �鷯���� �͵� �����ٳ�.:0" }); // ��ī��

        talkData.Add(3000, new string[] { "��ƾ�~ �� �Ƹ��ٿ� �Ǻΰ�~:0" }); // ���� 

        talkData.Add(4000, new string[] { "���� ���� �� �ڸ���..:0", "������..:0" }); // ��� ��ź 

        // Quest Talk
        talkData.Add(10 + 1000, new string[] { "� �� ! ȯ���� �ǹ̷� ����� �ϳ� ���״ϱ� �Ծ� �� !:0", "����� I Ű�� ���� �κ��丮�� ����� �����ž� !:0" }); // ���� 
        talkData.Add(11 + 1000, new string[] { "���� �� ����� ���� ������ϱ� �׷���~ ������ â�� �����.:0",
            "����� �����״� ����Ŭ���ؼ� ������ ��.:0" }); // ���� 
        talkData.Add(12 + 1000, new string[] { "�� �߾� ! �κ��丮���� ������ ����� �׷��� �ϴ°ž� !:0",
            "������ ��Ż�� Ÿ�� ���� ������ �̵��ϸ� �� !:0" }); // ���� 

        talkData.Add(20 + 1000, new string[] { "�̹����� ���͸� ����ϴ� ����� ������� ?:0", "���� Ű���� ControlŰ�� ������ ������ �� �� �־� !:0",
        "�� �׷��� �����ʿ� �ִ� �ִϾ� ���溼�� 5������ ��ƺ��� ?:0","�� ��� �ٽ� �� �ɾ��� !:0"}); // ���� 
        talkData.Add(21 + 1000, new string[] { "���� 5������ �� ������ ������ ?:0", "�������� �ִϾ� ���溼�� 5���� ��� �� �ɾ���:0" }); // ���� 
        talkData.Add(22 + 1000, new string[] { "���� �������� ����� ������ ���� !:0", "�̹����� ������ ��Ż�� Ÿ�� ���� ������ �Ѿ�� ��!:0" }); // ����

        portraitData.Add(1000 + 0, portraitArr[0]); // ���� �ʻ�ȭ
        portraitData.Add(2000 + 0, portraitArr[1]); // ��ī�� �ʻ�ȭ
        portraitData.Add(3000 + 0, portraitArr[2]); // ���� �ʻ�ȭ
        portraitData.Add(4000 + 0, portraitArr[3]); // ���� �ʻ�ȭ
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id)) // ���� talkData Dictionary �ȿ� �ش��ϴ� Key ���� ���ٸ� 
        {   // �ش� ����Ʈ ���� ���� ��簡 ���� ��. 
            if (!talkData.ContainsKey(id - id % 10))
            {
                return GetTalk(id - id % 100, talkIndex);
            }
            // ����Ʈ �� ó�� ��縶�� ���� �� �⺻ ��縦 ���.
            else
            {
                return GetTalk(id - id % 10, talkIndex);
            }
        }

        if (talkIndex == talkData[id].Length) // ��ȭ�� ���� ������ �ε����� ���ٸ�
            return null;
        else
            return talkData[id][talkIndex];
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
