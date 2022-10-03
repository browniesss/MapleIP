using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalData : MonoBehaviour
{
    // 포탈 스크립트
    Player player;
    GameManager gameManager;
    public int map_index; // 맵 인덱스 
    public string destinationMap_Name = null; // 목적지 맵 이름 

    bool checkstart = false;
    private bool checkbool = false;

    void Start()
    {
        player = FindObjectOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Map_Move() // 맵 이동 함수.
    {
        if (player.cur_MapIndex > map_index) // 다음 맵에서 포탈을 탈 경우
        {
            player.cur_MapMoving = 2;
            player.cur_MapIndex = map_index;
            player.sceneSave();
        }
        else // 이전 맵에서 포탈을 탈 경우
        {
            player.cur_MapMoving = 1;
            player.cur_MapIndex = map_index;
            player.sceneSave();
        }

        checkstart = true;
    }

    void Fade() // 맵 이동 시 페이드 함수.
    {

        gameManager.fadePanel.gameObject.SetActive(true);
        Color color = gameManager.fadePanel.color; //color 에 판넬 이미지 참조

        for (int i = 0; i <= 255; i++) //for문 100번 반복 0보다 작을 때 까지
        {
            color.a += Time.deltaTime * 0.01f; //이미지 알파 값을 타임 델타 값 * 0.01

            gameManager.fadePanel.color = color; //판넬 이미지 컬러에 바뀐 알파값 참조

            if (gameManager.fadePanel.color.a >= 1) //만약 판넬 이미지 알파 값이 1보다 커지면
            {
                checkbool = true; //checkbool 참으로 변경 해줘서 맵을 이동해줌. 
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
