using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TestRooms : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    public List<ManagementMapBlock> blocks = new List<ManagementMapBlock>();
    public List<GameObject> specialBlocks = new List<GameObject>();
    public bool autoInit;
    void Start()
    {
        if(autoInit) DrawMap();    
    }
    [NaughtyAttributes.Button]
    void DrawMap(){
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
        blocks.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            blocks.Add(transform.GetChild(i).GetComponent<ManagementMapBlock>());
        }
        for (int i = 0; i < specialBlocks.Count; i++)
        {
            blocks.Add(specialBlocks[i].GetComponent<ManagementMapBlock>());
        }
    }
    void DrawBlocks()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].transform.childCount > 0) ClearBlockChilds(blocks[i].transform);
        }
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].DrawBlock();
        }
    }
    [NaughtyAttributes.Button]
    void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
    void ClearBlockChilds(Transform block)
    {
        foreach (Transform child in block)
        {
            Destroy(child.gameObject);
        }
    }
    [NaughtyAttributes.Button]
    void UpdateNavMesh()
    {
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
    }
}
