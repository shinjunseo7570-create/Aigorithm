using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Pot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector2 thisPos;

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Skill"))
        {
            gameObject.SetActive(false);
            //항아리 반응 (스탯 만든 후)
            Debug.Log("항아리 깨짐");
        }
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
