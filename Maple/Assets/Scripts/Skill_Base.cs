using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    // ��ų BASE UI  ��ũ��Ʈ
    Player player;
    public GameObject skill_Window_base = null;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update_Skill_Window_Pos()
    {
        transform.position = new Vector3(skill_Window_base.transform.position.x,
            skill_Window_base.transform.position.y - 187.3f, 0);
    }

    void Update()
    {
        Update_Skill_Window_Pos();
    }
}
