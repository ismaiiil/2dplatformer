using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public float fadeSpeed = 0.02f;
    public bool fadeToBlack;
    public bool fadeToTrans;
    private Image image;

   
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (fadeToBlack)
        {
            var _alpha = image.color.a;
            _alpha += fadeSpeed;
            image.color = new Color(image.color.r, image.color.b, image.color.g, _alpha);
            if (_alpha >= 1f)
            {
                Debug.Log("Alpha 1" + _alpha);
                fadeToBlack = false;
                return;
            }
        }
        else if (fadeToTrans)
        {
            var _alpha = image.color.a;
            _alpha -= fadeSpeed;
            image.color = new Color(image.color.r, image.color.b, image.color.g, _alpha);
            if (_alpha <= 0f)
            {
                fadeToTrans = false;
                return;
            }
        }
    }


}
