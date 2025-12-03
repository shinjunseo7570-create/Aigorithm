using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    Vector2 origin;
    GameObject player;
    PlayerStats playerStats;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        origin = gameObject.transform.position;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Skill"))
        {
            gameObject.SetActive(false);
            Debug.Log("보물상자 파괴됨");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();
            buf();
            Debug.Log("보물 획득");
            gameObject.SetActive(false);
        }
    }
    void buf()
    {
        int bufnum = UnityEngine.Random.Range(0, 9);
        if (bufnum == 0)
        {
            ItemSelectManager.Instance.ShowItemSelection();
            Debug.Log("아이템 선택 스크린");
        }
        else if (bufnum == 1 || bufnum == 2)
        {
            playerStats.attackPower += 1;
            Debug.Log("공격력 증가" + playerStats.attackPower);
        }
        else if (bufnum == 3 || bufnum == 4)
        {
            playerStats.maxHealth += 10;
            playerStats.health += 10;
            Debug.Log("체력 증가" + playerStats.health);
        }
        else if (bufnum == 5 || bufnum == 6)
        {
            playerStats.currentStamina += 10;
            if (playerStats.currentStamina > playerStats.maxStamina)
            {
                playerStats.currentStamina = playerStats.maxStamina;
            }
            Debug.Log("스테미너 회복" + playerStats.currentStamina);
        }
        else if (bufnum == 7 || bufnum == 8)
        {
            playerStats.point += 10;
            Debug.Log("포인트 증가" + playerStats.point);
        }
        else { return; }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = origin;
    }
}
