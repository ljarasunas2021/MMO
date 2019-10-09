using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleController : MonoBehaviour
{
    public int secondsUntilDestroy;
    public Dictionary<GameObject, BulletHolesAndIndex> bulletHoles = new Dictionary<GameObject, BulletHolesAndIndex>();

    private int inactiveHoleIndex = 0;

    public void CreateBulletHole(RaycastHit hit, GameObject bulletHoleType)
    {
        GameObject hole;
        BulletHolesAndIndex bulletHolesAndIndex;
        if (bulletHoles.ContainsKey(bulletHoleType))
        {
            bulletHolesAndIndex = bulletHoles[bulletHoleType];

            if (bulletHolesAndIndex.bulletHoles.Count < bulletHolesAndIndex.inactiveHoleIndex)
            {
                hole = bulletHolesAndIndex.bulletHoles[bulletHolesAndIndex.inactiveHoleIndex];
            }
            else
            {
                hole = SpawnBulletHole(bulletHoleType);
                bulletHolesAndIndex.bulletHoles.Add(hole);
            }
        }
        else
        {
            bulletHoles[bulletHoleType] = new BulletHolesAndIndex();
            bulletHolesAndIndex = bulletHoles[bulletHoleType];
            hole = SpawnBulletHole(bulletHoleType);
            bulletHolesAndIndex.bulletHoles.Add(hole);
        }
        hole.transform.position = hit.point + hit.normal * 0.01f;
        hole.transform.rotation = Quaternion.LookRotation(hit.normal);
        hole.SetActive(true);
        Debug.Log(bulletHolesAndIndex.inactiveHoleIndex);
        bulletHolesAndIndex.inactiveHoleIndex += 1;
        StartCoroutine(DestroyBulletHole(hole, bulletHoleType));
    }

    private IEnumerator DestroyBulletHole(GameObject hole, GameObject bulletHoleType)
    {
        yield return new WaitForSeconds(secondsUntilDestroy);
        hole.SetActive(false);
        BulletHolesAndIndex bulletHolesAndIndex = bulletHoles[bulletHoleType];
        bulletHolesAndIndex.bulletHoles.Remove(hole);
        bulletHolesAndIndex.bulletHoles.Add(hole);
        bulletHolesAndIndex.inactiveHoleIndex--;
    }

    private GameObject SpawnBulletHole(GameObject bulletHoleType)
    {
        GameObject hole = Instantiate(bulletHoleType, transform);
        hole.SetActive(false);
        return hole;
    }
}

[System.Serializable]
public class BulletHolesAndIndex
{
    public List<GameObject> bulletHoles;
    public int inactiveHoleIndex;

    public BulletHolesAndIndex()
    {
        bulletHoles = new List<GameObject>();
        inactiveHoleIndex = 0;
    }
}
