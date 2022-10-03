using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    // ������ ��ũ��Ʈ
    Player player;
    SkillManager skillManager;
    [SerializeField]
    public SkillSlot[] Skill_slots;  // ���Ե� �迭

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        skillManager = FindObjectOfType<SkillManager>();
    }

    void Start()
    {
        Skill_slots = GetComponentsInChildren<SkillSlot>(); // ���� ������Ʈ���� �־���.

        LoadQuickSlot();
    }

    public void Update_QuickSlot() // ������ ������Ʈ �Լ�
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

            if (Skill_slots[i].skill != null) // ��ų�� �����ߴٸ�
                player.have_QuickSlot_Skill[i] = Skill_slots[i].skill.skill_Code; // �ش� ��ų�ڵ带 ��� ���Կ� �����ߴ��� ������ ����
        }
    }

    void LoadQuickSlot() // �� �̵� �� ������ �ε� �Լ�.
    {
        for (int i = 0; i < Skill_slots.Length; i++)
        {
            if (player.have_QuickSlot_Skill[i] != 0) // ������ �ִ� �������� �ִٸ�.
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
