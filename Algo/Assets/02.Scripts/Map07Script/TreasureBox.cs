using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Skill"))
        {
            gameObject.SetActive(false);
            Debug.Log("상자가 공격받아 파괴당함");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();
            //보상 주는 스크립트
            Debug.Log("상자와 상호작용하여 보상 획득");
            gameObject.SetActive(false);
        }
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
