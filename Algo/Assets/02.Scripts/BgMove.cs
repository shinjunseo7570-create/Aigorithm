using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BgMove : MonoBehaviour
{
    public RectTransform transform_bg_l;
    public RectTransform transform_bg_r;
    public Text text_mouse;
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
        transform_bg_l.pivot = Vector3.zero;
        transform_bg_l.position = (initPosition1);
        transform_bg_r.pivot = Vector3.zero;
        transform_bg_r.position = (initPosition2);

    }

    // Update is called once per frame
    void Update()
    {
        Update_MousePosition_Lobby();
    }

    //From https://codefinder.janndk.com/ 
    private void Update_MousePosition_Lobby()
    {
        Transform backgrounds = GameObject.Find("Canvas").transform.Find("Backgrounds Parent");

        if (backgrounds.gameObject.activeSelf == true)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            double a = mousePos.x * density;
            double b = mousePos.y * density;
            double c = mousePos.z * density;
            Vector3 bgPos = new Vector3(0 - (float)a, 0 - (float)b, 0 - (float)c);
            transform_bg_l.position = (bgPos);

            string message = mousePos.ToString();
            text_mouse.text = message;
            Debug.Log(message);
        }
    }

}
