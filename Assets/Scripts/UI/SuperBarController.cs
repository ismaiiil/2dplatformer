using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBarController : MonoBehaviour
{
    #pragma warning disable CS0649
    [SerializeField] private float healthBarWidth;
    #pragma warning disable CS0649
    [SerializeField] private PlayerController PlayerController;
    [SerializeField] private float healthBarWidthEase;
    private float healthBarWidthSmooth;
    // Start is called before the first frame update
    void Start()
    {
        healthBarWidth = 1;
        healthBarWidthSmooth = healthBarWidth;

    }

    // Update is called once per frame
    void Update()
    {
        healthBarWidth = (float)PlayerController.SuperAmount / (float)PlayerController.MaxSuperAMount;
        healthBarWidthSmooth += (healthBarWidth - healthBarWidthSmooth) * Time.deltaTime * healthBarWidthEase;
        transform.localScale = new Vector2(healthBarWidthSmooth, transform.localScale.y);
    }
}
