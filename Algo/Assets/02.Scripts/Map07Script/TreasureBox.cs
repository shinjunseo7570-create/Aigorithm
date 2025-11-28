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
            Debug.Log("보물상자 파괴됨");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();
            //���� �ִ� ��ũ��Ʈ
            Debug.Log("보물 획득");
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
