using System.Collections;
using System.Collections.Generic;

public class QuestData // 데이터만 저장해 줄 스크립트이며 오브젝트 컴포넌트에 추가하지 않을것이므로 Monobehaviour X
{
    public string questName;
    public int[] npcId;

    public QuestData(string name,int[] npc)
    {
        questName = name;
        npcId = npc;
    }
}
