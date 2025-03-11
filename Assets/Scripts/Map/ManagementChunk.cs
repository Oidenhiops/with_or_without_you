using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagementChunk : MonoBehaviour
{
    public ChunkInfo chunkInfo;
    [NaughtyAttributes.Button]
    public void CheckObjectInsideChunk()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, chunkInfo.chunkSize * Vector3.one / 2.01f, Quaternion.identity, LayerMask.GetMask("Map"));
        if (hitColliders.Length > 0 )
        {
            foreach (Collider collider in hitColliders)
            {
                collider.gameObject.transform.SetParent(transform);
            }
            CombineMeshes();
        }
    }
    public void CombineMeshes()
    {
        Dictionary<Texture2D, List<GameObject>> materialToCombineInstances = new Dictionary<Texture2D, List<GameObject>>();
        List<GameObject> allBlocks = new List<GameObject>();
        foreach (Transform child in transform)
        {
            allBlocks.Add(child.gameObject);
        }

        for (int i = 0; i < allBlocks.Count; i++)
        {
            Texture2D material = (Texture2D)allBlocks[i].GetComponent<MeshRenderer>().material.GetTexture("_BaseMap");
            if (!materialToCombineInstances.ContainsKey(material))
            {
                materialToCombineInstances[material] = new List<GameObject>();
            }
            materialToCombineInstances[material].Add(allBlocks[i]);
        }
        foreach (var entry in materialToCombineInstances)
        {
            List<GameObject> bloks = entry.Value;
            CombineInstance[] combineInstances = new CombineInstance[bloks.Count];
            for (int i = 0; i < bloks.Count; i++)
            {
                MeshFilter meshFilter = bloks[i].GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    combineInstances[i].mesh = meshFilter.sharedMesh;
                    combineInstances[i].transform = bloks[i].transform.localToWorldMatrix;
                }
            }
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combineInstances, true, true);
            GameObject combinedObject = new GameObject();
            combinedObject.layer = LayerMask.NameToLayer("Map");
            combinedObject.transform.SetParent(transform);
            MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedMesh;
            MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
            combinedObject.AddComponent<MeshCollider>();
            combinedMeshRenderer.material = bloks[0].GetComponent<MeshRenderer>().material;
            combinedObject.name = $"CombinedMesh {bloks[0].GetComponent<MeshRenderer>().material.name}";
            foreach (GameObject obj in bloks)
            {
                Destroy(obj);
            }
        }
        DisableCombinedMesh();
    }
    [NaughtyAttributes.Button]
    public void DisableCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
            //transform.GetChild(i).GetComponent<Collider>().enabled = false;
        }
    }
    [NaughtyAttributes.Button]
    public void EnabledCombinedMesh()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
            //transform.GetChild(i).GetComponent<Collider>().enabled = true;
        }
    }
    [NaughtyAttributes.Button]
    public void GenerateMap()
    {
        StartCoroutine(RestartMap());
    }
    public IEnumerator RestartMap()
    {
        chunkInfo.walkerPositions = new List<Vector3>();
        chunkInfo.occupiedPositions = new HashSet<Vector3>();
        chunkInfo.allBlocks = new List<ManagementMapBlock>();
        chunkInfo.steps = chunkInfo.chunkSize * 2;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(GenerateBlocksInChunk());
    }
    public IEnumerator GenerateBlocksInChunk()
    {
        Vector3 startPosition = transform.position;
        for (int i = 0; i < chunkInfo.walkerCount; i++)
        {
            chunkInfo.walkerPositions.Add(startPosition);
            chunkInfo.occupiedPositions.Add(startPosition);
        }
        GameObject initialBlock = Instantiate(chunkInfo.cubePrefab, startPosition, Quaternion.identity, transform);
        chunkInfo.allBlocks.Add(initialBlock.GetComponent<ManagementMapBlock>());
        chunkInfo.walkerPositions[0] = startPosition;
        for (int i = 0; i < chunkInfo.steps; i++)
        {
            for (int j = 0; j < chunkInfo.walkerPositions.Count; j++)
            {
                Vector3 walkerPosition = chunkInfo.walkerPositions[j];
                bool moved = false;
                for (int attempt = 0; attempt < 4; attempt++)
                {
                    Vector3 direction = GetRandomDirection();
                    Vector3 newPosition = walkerPosition + direction;
                    newPosition.x = Mathf.Clamp(newPosition.x, -chunkInfo.chunkSize / 2 + transform.position.x, chunkInfo.chunkSize / 2 + transform.position.x);
                    newPosition.z = Mathf.Clamp(newPosition.z, -chunkInfo.chunkSize / 2 + transform.position.z, chunkInfo.chunkSize / 2 + transform.position.z);
                    if (!chunkInfo.occupiedPositions.Contains(newPosition))
                    {
                        InstantiateBlock(newPosition);
                        chunkInfo.walkerPositions[j] = newPosition;
                        moved = true;
                        break;
                    }
                }
                if (!moved)
                {
                    chunkInfo.walkerPositions[j] = GetRandomOccupiedPosition();
                }
            }
        }
        chunkInfo.blocksPositions = GetBlocksPositions();
        yield return new WaitForSeconds(1);
        FillHoles();
        yield return new WaitForSeconds(1);
        DestroyBridges();
    }
    public void DestroyBridges()
    {
        for (int m = 0; m < 10; m++)
        {
            for (int p = chunkInfo.blocksPositions.Count - 1; p >= 0; p--)
            {
                if (ValidateNeighbours(chunkInfo.blocksPositions[p]))
                {
                    foreach(var block in chunkInfo.allBlocks)
                    {
                        if (block.transform.position == chunkInfo.blocksPositions[p])
                        {
                            Destroy(block.gameObject);
                        }
                    }
                    chunkInfo.blocksPositions.RemoveAt(p);
                    chunkInfo.allBlocks.RemoveAt(p);
                }
            }
        }
    }
    public bool ValidateNeighbours(Vector3 pos)
    {
        int amountColl = 0;
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.forward))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.back))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.left))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.right))
        {
            amountColl++;
        }
        if (amountColl <= 1)
        {
            return true;
        }
        return false;
    }
    public void FillHoles()
    {
        for (int m = 0; m < 10; m++)
        {
            for (int p = 0; p < chunkInfo.blocksPositions.Count; p++)
            {
                if (!chunkInfo.blocksPositions.Contains(chunkInfo.blocksPositions[p] + Vector3.forward) && ValidateFillHole(chunkInfo.blocksPositions[p] + Vector3.forward))
                {
                    InstantiateBlock(chunkInfo.blocksPositions[p] + Vector3.forward);
                    chunkInfo.blocksPositions.Add(chunkInfo.blocksPositions[p] + Vector3.forward);
                }
                if (!chunkInfo.blocksPositions.Contains(chunkInfo.blocksPositions[p] + Vector3.back) && ValidateFillHole(chunkInfo.blocksPositions[p] + Vector3.back))
                {
                    InstantiateBlock(chunkInfo.blocksPositions[p] + Vector3.back);
                    chunkInfo.blocksPositions.Add(chunkInfo.blocksPositions[p] + Vector3.back);
                }
                if (!chunkInfo.blocksPositions.Contains(chunkInfo.blocksPositions[p] + Vector3.left) && ValidateFillHole(chunkInfo.blocksPositions[p] + Vector3.left))
                {
                    InstantiateBlock(chunkInfo.blocksPositions[p] + Vector3.left);
                    chunkInfo.blocksPositions.Add(chunkInfo.blocksPositions[p] + Vector3.left);
                }
                if (!chunkInfo.blocksPositions.Contains(chunkInfo.blocksPositions[p] + Vector3.right) && ValidateFillHole(chunkInfo.blocksPositions[p] + Vector3.right))
                {
                    InstantiateBlock(chunkInfo.blocksPositions[p] + Vector3.right);
                    chunkInfo.blocksPositions.Add(chunkInfo.blocksPositions[p] + Vector3.right);
                }
            }
        }
    }
    public bool ValidateFillHole(Vector3 pos)
    {
        int amountColl = 0;
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.forward))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.back))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.left))
        {
            amountColl++;
        }
        if (chunkInfo.blocksPositions.Contains(pos + Vector3.right))
        {
            amountColl++;
        }
        if (amountColl >= 3)
        {
            return true;
        }
        return false;
    }
    public void InstantiateBlock(Vector3 positionInstance)
    {
        if (!ExistsBlock(positionInstance))
        {
            GameObject block = Instantiate(chunkInfo.cubePrefab, positionInstance, Quaternion.identity, transform);
            chunkInfo.allBlocks.Add(block.GetComponent<ManagementMapBlock>());
            chunkInfo.occupiedPositions.Add(positionInstance);
        }
    }
    bool ExistsBlock(Vector3 pos)
    {
        return chunkInfo.allBlocks.Any(b => b.transform.position == pos);
    }
    public List<Vector3> GetBlocksPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var block in chunkInfo.allBlocks)
        {
            positions.Add(block.gameObject.transform.position);
        }
        return positions;
    }
    Vector3 GetRandomDirection()
    {
        // Elegir una direcci�n aleatoria en el plano XZ (arriba, abajo, izquierda, derecha)
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0: return Vector3.forward;   // Arriba (en Z)
            case 1: return Vector3.back;      // Abajo (en -Z)
            case 2: return Vector3.right;     // Derecha (en X)
            default: return Vector3.left;     // Izquierda (en -X)
        }
    }
    Vector3 GetRandomOccupiedPosition()
    {
        // Elegir una posici�n ocupada aleatoria
        int index = Random.Range(0, chunkInfo.occupiedPositions.Count);
        foreach (var position in chunkInfo.occupiedPositions)
        {
            if (index == 0) return position;
            index--;
        }
        return Vector3.zero; // Retorno de seguridad, aunque no deber�a ocurrir
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, chunkInfo.chunkSize * Vector3.one);
    }
    [System.Serializable]
    public class ChunkInfo
    {
        public int chunkSize;
        public int walkerCount = 5;   // N�mero de "walkers"
        public int steps = 0;
        public List<Vector3> walkerPositions; // Lista de posiciones de los walkers
        public HashSet<Vector3> occupiedPositions; // Posiciones ya ocupadas por cubos
        public List<Vector3> blocksPositions = new List<Vector3>();
        public GameObject cubePrefab;
        public List<ManagementMapBlock> allBlocks = new List<ManagementMapBlock>();
    }
}
