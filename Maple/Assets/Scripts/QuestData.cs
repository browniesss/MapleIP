using System.Collections;
using System.Collections.Generic;

public class QuestData // �����͸� ������ �� ��ũ��Ʈ�̸� ������Ʈ ������Ʈ�� �߰����� �������̹Ƿ� Monobehaviour X
{
    public string questName;
    public int[] npcId;

    public QuestData(string name,int[] npc)
    {
        questName = name;
        npcId = npc;
    }
}
