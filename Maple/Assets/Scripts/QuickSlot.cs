using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    // 퀵슬롯 스크립트
    Player player;
    SkillManager skillManager;
    [SerializeField]
    public SkillSlot[] Skill_slots;  // 슬롯들 배열

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        skillManager = FindObjectOfType<SkillManager>();
    }

    void Start()
    {
        Skill_slots = GetComponentsInChildren<SkillSlot>(); // 하위 오브젝트들을 넣어줌.

        LoadQuickSlot();
    }

    public void Update_QuickSlot() // 퀵슬롯 업데이트 함수
    {
        for (int i = 0; i < Skill_slots.Length; i++)
        {
            for (int k = 0; k < Skill_slots.Length; k++)
            {
                if (Skill_slots[i].skill != null)
                {
                    if (player.have_QuickSlot_Skill[k] == Skill_slots[i].skill.skill_Code)
                        player.have_QuickSlot_Skill[k] = 0;
                }
            }

            if (Skill_slots[i].skill != null) // 스킬을 보유했다면
                player.have_QuickSlot_Skill[i] = Skill_slots[i].skill.skill_Code; // 해당 스킬코드를 몇번 슬롯에 장착했는지 정보를 저장
        }
    }

    void LoadQuickSlot() // 씬 이동 시 퀵슬롯 로드 함수.
    {
        for (int i = 0; i < Skill_slots.Length; i++)
        {
            if (player.have_QuickSlot_Skill[i] != 0) // 가지고 있는 아이템이 있다면.
            {
                skillManager.skillList[player.have_QuickSlot_Skill[i] - 1].usable_Skill = true;
                Skill_slots[i].AddSkill(skillManager.skillList[player.have_QuickSlot_Skill[i] - 1]);
            }
        }
    }

    void Update()
    {

    }
}
