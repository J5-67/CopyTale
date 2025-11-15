using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : MonoBehaviour
{
    [SerializeField] private GameObject Heart;
    private Movement2D movement2D;

    private void Awake()
    {
        TryGetComponent(out movement2D);
    }
    private void Update()
    {
        //¿Œ«≤Ω√Ω∫≈€
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        movement2D.MoveTo(new Vector3(inputX, inputY, 0f));
    }

}
