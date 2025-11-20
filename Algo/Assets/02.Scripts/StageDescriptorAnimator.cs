using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class StageDescriptorAnimator : MonoBehaviour
{

    public float speed = 15.0f;
    public float duration = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(MoveUp());
            yield return StartCoroutine(MoveDown());
        }
    }

    IEnumerator MoveUp()
    {
        float timer = 0;
        while (timer < duration)
        {
            transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator MoveDown()
    {
        float timer = 0;
        while (timer < duration)
        {
            transform.Translate(new Vector3(0, -1, 0) * speed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;

        }
    }
}
