using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalData : MonoBehaviour
{
    // ��Ż ��ũ��Ʈ
    Player player;
    GameManager gameManager;
    public int map_index; // �� �ε��� 
    public string destinationMap_Name = null; // ������ �� �̸� 

    bool checkstart = false;
    private bool checkbool = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Map_Move() // �� �̵� �Լ�.
    {
        if (player.cur_MapIndex > map_index) // ���� �ʿ��� ��Ż�� Ż ���
        {
            player.cur_MapMoving = 2;
            player.cur_MapIndex = map_index;
            player.sceneSave();
        }
        else // ���� �ʿ��� ��Ż�� Ż ���
        {
            player.cur_MapMoving = 1;
            player.cur_MapIndex = map_index;
            player.sceneSave();
        }

        checkstart = true;
    }

    void Fade() // �� �̵� �� ���̵� �Լ�.
    {

        gameManager.fadePanel.gameObject.SetActive(true);
        Color color = gameManager.fadePanel.color; //color �� �ǳ� �̹��� ����

        for (int i = 0; i <= 255; i++) //for�� 100�� �ݺ� 0���� ���� �� ����
        {
            color.a += Time.deltaTime * 0.01f; //�̹��� ���� ���� Ÿ�� ��Ÿ �� * 0.01

            gameManager.fadePanel.color = color; //�ǳ� �̹��� �÷��� �ٲ� ���İ� ����

            if (gameManager.fadePanel.color.a >= 1) //���� �ǳ� �̹��� ���� ���� 1���� Ŀ����
            {
                checkbool = true; //checkbool ������ ���� ���༭ ���� �̵�����. 
            }
        }
    }

    private void Update()
    {
        if (checkstart)
            Fade();

        if(checkbool)
            SceneManager.LoadScene(destinationMap_Name);
    }
}
