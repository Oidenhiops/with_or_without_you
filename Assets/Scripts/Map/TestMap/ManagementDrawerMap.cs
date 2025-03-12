using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class ManagementDrawerMap : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    List<ManagementMapBlock> mapBlocks = new List<ManagementMapBlock>();
    List<ManagementMapSetTexture> decorationsBlocks = new List<ManagementMapSetTexture>();

    public bool autoInit;
    void Start()
    {
        if (autoInit) DrawMap();
    }
    [NaughtyAttributes.Button]
    void DrawMap()
    {
        StartCoroutine(DrawRoom());
    }
    public IEnumerator DrawRoom()
    {
        GetAllBlocks();
        DrawBlocks();
        yield return new WaitForSeconds(0.1f);
        BuildNavMesh();
    }
    void GetAllBlocks()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.TryGetComponent<ManagementMapBlock>(out ManagementMapBlock managementMapBlock))
            {
                mapBlocks.Add(managementMapBlock);
            }
            else if (transform.GetChild(i).gameObject.TryGetComponent<ManagementMapSetTexture>(out ManagementMapSetTexture managementMapSetTexture))
            {
                decorationsBlocks.Add(managementMapSetTexture);
            }
        }
    }
    void DrawBlocks()
    {
        for (int i = 0; i < mapBlocks.Count; i++)
        {
            mapBlocks[i].DrawBlock();
        }
        for (int i = 0; i < decorationsBlocks.Count; i++)
        {
            decorationsBlocks[i].DrawBlock();
        }
        mapBlocks.Clear();
        decorationsBlocks.Clear();
    }
    void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    void UpdateNavMesh()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
