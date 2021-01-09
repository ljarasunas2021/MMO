using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPuzzle : MonoBehaviour
{
    public GameObject puzzleSphere = null;
    public GameObject puzzlePipe = null;
    public GameObject puzzlePipeWrong = null;
    bool solved = false;
    // bool needReset = false;
    private Vector3 puzzleSphereCoords;
    void Start()
    {
        puzzleSphere.GetComponent<Renderer>().material.color = new Color(0, 0, 255f);
        puzzleSphereCoords = puzzleSphere.transform.position;
        puzzlePipe.GetComponent<Renderer>().material.color = new Color(0, 0, 255f);
        puzzlePipeWrong.GetComponent<Renderer>().material.color = new Color(255f, 0, 255f);
    }
    void Update()
    {
        if (!solved)
        {
            if (puzzleSphere.transform.position.x < 1068f && puzzleSphere.transform.position.x > 1064f)
            {
                if (puzzleSphere.transform.position.z < 1000f && puzzleSphere.transform.position.z > 996f && puzzleSphere.transform.position.y < 19f)
                {
                    solved = true;
                    puzzleSphere.GetComponent<Renderer>().material.color = new Color(0, 255f, 0);
                    puzzlePipe.GetComponent<Renderer>().material.color = new Color(0, 255f, 0);
                }
            }
            if (puzzleSphere.transform.position.x < 1090f && puzzleSphere.transform.position.x > 1086f)
            {
                if (puzzleSphere.transform.position.z < 1000f && puzzleSphere.transform.position.z > 996f && puzzleSphere.transform.position.y < 19f)
                {
                    Reset();
                    puzzleSphere.GetComponent<Renderer>().material.color = new Color(225f, 0, 0);
                    puzzlePipeWrong.GetComponent<Renderer>().material.color = new Color(225f, 0, 0);
                }
            }
        }
    }
    public void Reset()
    {
        Debug.Log("Reset");
        puzzleSphere.GetComponent<Renderer>().material.color = new Color(0, 0, 255f);
        puzzlePipe.GetComponent<Renderer>().material.color = new Color(0, 0, 255f);
        puzzlePipeWrong.GetComponent<Renderer>().material.color = new Color(255f, 0, 255f);
        puzzleSphere.transform.position = puzzleSphereCoords;
        puzzleSphere.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
