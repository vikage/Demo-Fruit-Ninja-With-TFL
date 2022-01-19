using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField]
    public GameObject bladeTrailPrefab;

    bool isCutting = false;

    private Rigidbody rb;

    private Camera cam;

    private GameObject currentBladeTrail;

    private SphereCollider collider;

    [SerializeField]
    private GameObject effectPrefab;

    private GameObject currentBladeEffect;

    private Coroutine routineEffect;

    [SerializeField]
    private bool swipeCutting = false;

    private Vector3 cuttingPosition = new Vector3();

    private void Start()
    {
        currentBladeEffect = Instantiate(effectPrefab, transform.position, transform.rotation);
        currentBladeEffect.SetActive(false);
        collider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }
    void LateUpdate()
    {
        if (swipeCutting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCutting();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                StopCutting();
            }
        }

        if (isCutting)
        {
            UpdateCutting();
        }
    }

    public void SetCuttingPosition(Vector2 position)
    {
        StartCutting();
        cuttingPosition = new Vector3(position.x,position.y, transform.position.z);
    }
    public void SetCuttingPosition(Vector3 position)
    {
        StartCutting();
        cuttingPosition = position;
    }

    void UpdateCutting()
    {
        if (swipeCutting)
        {
            Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            cuttingPosition = new Vector3(worldPoint.x, worldPoint.y, rb.position.z);
        }
        rb.position = cuttingPosition;
    }

    void StartCutting()
    {
        isCutting = true;
        currentBladeTrail = Instantiate(bladeTrailPrefab, transform);
        collider.enabled = true;
    }

    public void StopCutting()
    {
        isCutting = false;
        currentBladeTrail.transform.SetParent(null);
        Destroy(currentBladeTrail, 1f);
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            currentBladeEffect.transform.position = transform.position;
            currentBladeEffect.SetActive(true);
            if (routineEffect!=null)
            StopCoroutine(routineEffect);
            routineEffect = StartCoroutine(ShowEffect());
        }
    }

    private IEnumerator ShowEffect()
    {
        yield return new WaitForSeconds(0.4f);
        currentBladeEffect.SetActive(false);
    }
}
