using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearance : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField]
    private float rotateSpeed = 80f;

    private Vector3 rotateDirection = Vector3.forward;

    [SerializeField]
    private bool needRotate = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (needRotate)
            transform.Rotate(rotateDirection * Time.deltaTime * rotateSpeed);
    }

  
}
