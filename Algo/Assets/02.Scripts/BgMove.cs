using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BgMove : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform transform_bg_l;
    public float density;

    // Start is called before the first frame update
    void Start()
    {
        Init_Cursor();
    }

    private void Init_Cursor()
    {
        Vector3 initPosition1 = new Vector3(0, 0, 0);
        Vector3 initPosition2 = new Vector3(0, 0, 0);
        transform_bg_l.pivot = Vector2.zero;
        transform_bg_l.position = (initPosition1);

    }

    // Update is called once per frame
    void Update()
    {
        Update_MousePosition_Lobby();
    }

    //CodeFinder 코드파인더
    //From https://codefinder.janndk.com/ 
    private void Update_MousePosition_Lobby()
    {
        Transform backgrounds = GameObject.Find("Canvas").transform.Find("Backgrounds Parent");

        if (backgrounds.gameObject.activeSelf == true)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            double a = mousePos.x * density;
            double b = mousePos.y * density;
            Vector2 bgPos = new Vector2(0 - (float)a, 0 - (float)b);
            transform_bg_l.position = (bgPos);
        }
    }

}
