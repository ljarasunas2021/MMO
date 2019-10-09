using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleController : MonoBehaviour
{
    public GameObject bulletHole;
    public int maxBullets;
    public int secondsUntilDestroy;
    public List<GameObject> bulletHoles;

    private int inactiveHoleIndex = 0;

    private void Start()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            GameObject hole = Instantiate(bulletHole, transform);
            bulletHoles.Add(hole);
            hole.SetActive(false);
        }
    }

    public void CreateBulletHole(RaycastHit hit)
    {
        Debug.Log(hit.normal);
        GameObject hole = bulletHoles[inactiveHoleIndex];
        hole.transform.position = hit.point + hit.normal * 0.2f;
        hole.transform.rotation = Quaternion.LookRotation(hit.normal);
        hole.SetActive(true);
        inactiveHoleIndex++;
        StartCoroutine(DestroyBulletHole(hole));
    }

    private IEnumerator DestroyBulletHole(GameObject hole)
    {
        yield return new WaitForSeconds(secondsUntilDestroy);
        hole.SetActive(false);
        bulletHoles.Remove(hole);
        bulletHoles.Add(hole);
        inactiveHoleIndex--;
    }
}
