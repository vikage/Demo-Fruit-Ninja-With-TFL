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
        collider.enabled = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Transform appearance = this.gameObject.transform.GetChild(0);
        int partCount = appearance.childCount;
        List<Transform> parts = new List<Transform>();
        while (appearance.childCount > 0)
        {
            Transform part = appearance.GetChild(0);
            part.SetParent(null);
            Rigidbody rbChild = part.GetComponent<Rigidbody>();
            rbChild.useGravity = true;
            rbChild.isKinematic = false;
            Vector2 dropVector = new Vector2(Random.Range(-4f, 4f), Random.Range(-4f, 4f));
            rbChild.AddForce(dropVector, ForceMode.Impulse);
            Destroy(part.gameObject, 2f);
        }
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Blade")
        {
            SlicedEnemy();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "wall")
            Destroy(gameObject, 0f);
    }
}
