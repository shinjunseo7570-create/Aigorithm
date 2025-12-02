using UnityEngine;

public class potal : MonoBehaviour
{
    bool ready = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ready = true;
            Debug.Log("준비");
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ready = false;
            Debug.Log("준비 해제");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && ready)
        {
            LoadingSceneManager.LoadScene("Main");
            Debug.Log("씬 아웃");
            }
    }
}
