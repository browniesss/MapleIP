using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 사운드 매니저 스크립트 
    public AudioClip[] audioList; // 사운드 목록을 저장.

    AudioSource audioSource;

    int randBgm;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        randBgm = Random.Range(0, 3); // 랜덤값으로 0 ~ 2 값을 받아온 후 

        audioSource.clip = audioList[randBgm]; // 해당 스크립트를 가지고 있는 오브젝트의 AudiSource 클립을 audioList의 bgm 중 랜덤으로 넣어
        audioSource.Play(); // 재생
    }

    void Update()
    {
        
    }
}
