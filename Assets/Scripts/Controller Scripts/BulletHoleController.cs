using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

///<summary> Control bullet holes </summary>
public class BulletHoleController : NetworkBehaviour
{
    #region Variables
    // seconds until bullet hole is destroyed
    public int secondsUntilDestroy;
    // list of materials and corresponding bullet holes
    public List<MaterialAndBulletHole> materialsAndBulletHoles = new List<MaterialAndBulletHole>();

    // dictionary of corresponding material and bullet holes
    private Dictionary<Material, GameObject> materialAndBulletHolesDict = new Dictionary<Material, GameObject>();
    // dictionary of bullet hole prefab and list of bullet holes
    private Dictionary<GameObject, BulletHolesAndIndex> bulletHoles = new Dictionary<GameObject, BulletHolesAndIndex>();
    //array of material prefabs
    private Material[] materialPrefabs;
    #endregion

    #region  Initialize

    private void Awake()
    {
        materialPrefabs = new Material[materialsAndBulletHoles.Count];
        for (int i = 0; i < materialsAndBulletHoles.Count; i++)
        {
            materialPrefabs[i] = materialsAndBulletHoles[i].material;
        }
    }

    /// <summary> Create dictionary materialAndBulletHolesDict </summary>
    private void Start()
    {
        foreach (MaterialAndBulletHole materialAndBulletHole in materialsAndBulletHoles) materialAndBulletHolesDict[materialAndBulletHole.material] = materialAndBulletHole.bulletHole;
    }
    #endregion

    #region BulletHoleVoids
    [ClientRpc]
    ///<summary> Create a bullet hole using object pooling (disabling/enabling gameObjects rather than instantiating destroying them<summary>
    ///<param name = "hit"> hit of raycast which will determine where it is placed </param>
    ///<param name = "material"> material that raycast hit </param>
    public void RpcCreateBulletHole(int materialIndex, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (materialIndex == -1) return;

        Material material = materialPrefabs[materialIndex];
        GameObject bulletHoleType = materialAndBulletHolesDict[material];
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
        hole.transform.position = hitPoint + hitNormal * 0.01f;
        hole.transform.rotation = Quaternion.LookRotation(hitNormal);
        hole.SetActive(true);
        bulletHolesAndIndex.inactiveHoleIndex += 1;
        StartCoroutine(DestroyBulletHole(hole, bulletHoleType));
    }

    ///<summary> Destroy a bullet hole </summary>
    ///<param name = "hole"> bullet hole to destroy </param>
    ///<param name = "bulletHoleType"> bullet hole prefab that corresponds to type of bullet hole </param>
    private IEnumerator DestroyBulletHole(GameObject hole, GameObject bulletHoleType)
    {
        yield return new WaitForSeconds(secondsUntilDestroy);
        hole.SetActive(false);
        BulletHolesAndIndex bulletHolesAndIndex = bulletHoles[bulletHoleType];
        bulletHolesAndIndex.bulletHoles.Remove(hole);
        bulletHolesAndIndex.bulletHoles.Add(hole);
        bulletHolesAndIndex.inactiveHoleIndex--;
    }

    ///<summary> Spawn a new bullet hole </summary>
    ///<param name = "bulletHoleType"> type of bullet hole that will be spawned </param>
    private GameObject SpawnBulletHole(GameObject bulletHoleType)
    {
        GameObject hole = Instantiate(bulletHoleType, transform);
        hole.SetActive(false);
        return hole;
    }
    #endregion

    ///<summary> Return the material prefabs </summary>
    public Material[] ReturnMaterialPrefabs()
    {
        return materialPrefabs;
    }
}

#region BulletHolesAndIndex
[System.Serializable]
/// <summary> Contains a list of bullet holes and the inactive hole index </summary>
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
#endregion

#region MaterialAndBulletHole
[System.Serializable]
///<summary> Contains a material and the corresponding bullet hole prefab<summary>
public class MaterialAndBulletHole
{
    public Material material;
    public GameObject bulletHole;
}
#endregion
