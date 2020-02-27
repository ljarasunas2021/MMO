using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : MonoBehaviour
{
    public float speed, radius;

    // Start is called before the first frame update
    void Start()
    {
        float theta = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, radius);
        transform.position = new Vector3(Mathf.Cos(theta) * distance, transform.position.y, Mathf.Sin(theta) * distance);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x - speed, transform.localScale.y - speed, transform.localScale.z);
    }
}
