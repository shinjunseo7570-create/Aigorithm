using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Pot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector2 thisPos;
    GameObject player;
    PlayerStats playerStats;
    void Awake()
    { 
        player = GameObject.FindWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Skill"))
        {
            gameObject.SetActive(false);
            BreakPot();
            Debug.Log("항아리 깨짐");
        }
    }
    public void TakeDamage(float amount)
    {
        gameObject.SetActive(false);
        BreakPot();
        //항아리 반응 (스탯 만든 후)
        Debug.Log("항아리 깨짐");
    }

    public void BreakPot()
    {
        int per = UnityEngine.Random.Range(0, 3);
        if(per < 4)
        {
            Debug.Log("버프");
            buf();
        }
        else if(per < 8)
        {
            Debug.Log("디버프");
            debuf();
        }
        else
        {
            Debug.Log("빈 항아리");
            return;
        }
    }
    void buf()
    {
        int bufnum = UnityEngine.Random.Range(0, 1);
        if (bufnum == 0) 
        {
            ItemSelectManager.Instance.ShowItemSelection();
            Debug.Log("아이템 선택 스크린");
        }
        else if(bufnum == 1 || bufnum == 2)
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
            if(playerStats.currentStamina > playerStats.maxStamina)
            {
                playerStats.currentStamina = playerStats.maxStamina;
            }
            Debug.Log("스테미너 회복" + playerStats.currentStamina);
        }
        else { return; }
    }
    void debuf()
    {
        int debufnum = UnityEngine.Random.Range(0, 17);
        if (debufnum == 0)
        {
            Debug.Log("아이템 소실");
            //loseItem();
        }
        else if (1 <= debufnum && debufnum <= 4)
        {
            playerStats.attackPower -= 1;
            Debug.Log("공격력 감소" + playerStats.attackPower);
        }
        else if (5<= debufnum && debufnum <= 8)
        {
            playerStats.maxHealth -= 10;
            playerStats.health -= 10;
            Debug.Log("체력 감소" + playerStats.health);
        }
        else if (9<= debufnum && debufnum <= 12)
        {
            playerStats.currentStamina -= 10;
            Debug.Log("스테미너 감소" + playerStats.currentStamina);
        }
        else if(13<= debufnum && debufnum <= 16)
        {
            //포인트 감소 코드
            Debug.Log("포인트 감소");
        }
        else { return; }
    }

    void Start()
    {
        thisPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = thisPos;
    }
}
