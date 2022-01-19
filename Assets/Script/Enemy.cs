using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float speedMoving = 100f;

    [SerializeField]
    private GameObject[] enemyPrefabs;

    private GameObject currentEnemy;

    public Rigidbody rb;

    public SphereCollider sphereCollider;
    public BoxCollider boxCollider;
    public Collider collider;

    [SerializeField]
    Transform gm;

    private int indexEnemy = 0;

    private GameObject collisionEffect;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        boxCollider = GetComponent<BoxCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.up, ForceMode.Impulse);
        LaunchEnemy();
        //Instantiate(gm, transform);
    }

    public void LaunchEnemy()
    {
        indexEnemy = Random.Range(0, enemyPrefabs.Length);
        GameObject enemy = enemyPrefabs[indexEnemy];
        if (enemy.tag == "ball")
        {
            collider = sphereCollider;
        }
        else
        {
            collider = boxCollider;
        }
        collider.enabled = true;
        currentEnemy = Instantiate(enemyPrefabs[indexEnemy], transform);
    }

    // Update is called once per frame
    public void SlicedEnemy()
    {
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Blade")
        {
            collider.enabled = false;
            //rb.useGravity = true;
            rb.useGravity = true;
            SlicedEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall")
            Destroy(gameObject, 0f);
    }
}
