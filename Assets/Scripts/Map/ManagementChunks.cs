using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagementChunks : MonoBehaviour
{
    public AStarPathFinding aStarPathFinding;
    public ManagementSpawn managementSpawn;
    public ManagementOpenCloseScene managementOpenCloseScene;
    public int chunkSize = 17;
    public int chunksX = 5;
    public int chunksZ = 5;
    public List<PositionChunk> positionsChunks = new List<PositionChunk>();
    public List<ManagementMapBlock> allBlocksGenerated = new List<ManagementMapBlock>();
    public List<Vector3> positionsForMakePath = new List<Vector3>();
    public ManagementMapBlock spawnPosition;
    public GameObject initialChunk;
    public GameObject[] characters;
    public DirectionChunk directionChunk;
    private void Start()
    {
        StartCoroutine(GenerateChunks());
    }
    public IEnumerator GenerateChunks()
    {
        positionsChunks = GenerateMap(chunksX, chunksZ);
        GameObject chunkPrefab = Resources.Load<GameObject>("Prefabs/Map/Chunk");
        float offsetX = chunksX * chunkSize / 2f - (chunkSize / 2f);
        float offsetZ = chunksZ * chunkSize / 2f - (chunkSize / 2f);

        for (int x = 0; x < chunksX; x++)
        {
            for (int z = 0; z < chunksZ; z++)
            {
                for (int c = 0; c < positionsChunks.Count; c++)
                {
                    if (positionsChunks[c].positionChunk == new Vector3Int(x, 0, z))
                    {
                        Vector3 chunkPosition = new Vector3(x * chunkSize - offsetX, 0, z * chunkSize - offsetZ);
                        GameObject chunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
                        positionsChunks[c].managementChunk = chunk.GetComponent<ManagementChunk>();
                        chunk.GetComponent<ManagementChunk>().chunkInfo.chunkSize = chunkSize;
                        chunk.GetComponent<BoxCollider>().size = chunk.GetComponent<ManagementChunk>().chunkInfo.chunkSize * Vector3.one;
                        chunk.name = $"Chunk_{x}_{z}";
                        chunk.transform.parent = this.transform;
                    }
                }
            }
        }
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            positionsChunks[i].managementChunk.GenerateMap();
        }
        yield return new WaitForSeconds(1);
        managementOpenCloseScene.AdjustLoading(10);
        yield return new WaitForSeconds(1);
        managementOpenCloseScene.AdjustLoading(10);
        MakeBridges();
        yield return new WaitForSeconds(1);
        managementOpenCloseScene.AdjustLoading(20);
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            for (int b = 0; b < positionsChunks[i].managementChunk.chunkInfo.allBlocks.Count; b++)
            {
                allBlocksGenerated.Add(positionsChunks[i].managementChunk.chunkInfo.allBlocks[b]);
            }
        }
        yield return new WaitForSeconds(1);
        for (int b = allBlocksGenerated.Count - 1; b >= 0; b--)
        {
            if (allBlocksGenerated[b] == null)
            {
                allBlocksGenerated.RemoveAt(b);
            }
        }
        managementOpenCloseScene.AdjustLoading(10);
        yield return new WaitForSeconds(1);
        foreach (ManagementMapBlock mapBlock in allBlocksGenerated)
        {
            mapBlock.DetectInSpecifiedDirections();
        }
        managementOpenCloseScene.AdjustLoading(20);
        yield return new WaitForSeconds(1);
        managementOpenCloseScene.AdjustLoading(10);
        spawnPosition = allBlocksGenerated[Random.Range(0, allBlocksGenerated.Count - 1)];
        GetInitialChunk();
        characters = GameObject.FindGameObjectsWithTag("Player");
        foreach (var character in characters)
        {
            character.transform.position = new Vector3(spawnPosition.transform.position.x, 1, spawnPosition.transform.position.z);
        }
        positionsForMakePath = GetPositionsToMakePath();
        managementSpawn.SetSpawnPositions();
        yield return new WaitForSeconds(1);
        allBlocksGenerated.Clear();
        aStarPathFinding.occupiedPositions = positionsForMakePath;
        aStarPathFinding.GenerateWalkableGrid();
        //CombineObjectsInChunks();
        managementOpenCloseScene.AdjustLoading(10);
        yield return new WaitForSeconds(1);
        managementOpenCloseScene.AdjustLoading(10);
        yield return new WaitForSeconds(1);
        managementSpawn.StartSpawn();
    }
    public void GetInitialChunk()
    {
        foreach (var chunk in positionsChunks)
        {
            if (chunk.managementChunk.chunkInfo.allBlocks.Contains(spawnPosition))
            {
                initialChunk = chunk.managementChunk.gameObject;
                break;
            }
        }
    }
    public void MakeBridges()
    {
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.forward))
            {
                BuildBridge(positionsChunks[i].managementChunk, positionsChunks[i].managementChunk.transform.position, Vector3.forward);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.back))
            {
                BuildBridge(positionsChunks[i].managementChunk, positionsChunks[i].managementChunk.transform.position, Vector3.back);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.left))
            {
                BuildBridge(positionsChunks[i].managementChunk, positionsChunks[i].managementChunk.transform.position, Vector3.left);
            }
            if (ValidateBridge(positionsChunks[i].positionChunk + Vector3Int.right))
            {
                BuildBridge(positionsChunks[i].managementChunk, positionsChunks[i].managementChunk.transform.position, Vector3.right);
            }
        }
    }
    public List<Vector3> GetPositionsToMakePath()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var block in allBlocksGenerated)
        {
            if (block.isWalkable)
            {
                positions.Add(block.gameObject.transform.position + Vector3.up);
            }
        }
        return positions;
    }
    public void BuildBridge(ManagementChunk chunk, Vector3 positionStartBridge, Vector3 direction)
    {
        int amountBlocks = (int)Mathf.Round(chunkSize / 2);
        for (int i = 0; i <= amountBlocks; i++)
        {
            Vector3 pos = positionStartBridge + direction * i;
            chunk.InstantiateBlock(pos);
            if (Mathf.Abs(direction.x) > 0)
            {
                chunk.InstantiateBlock(pos + Vector3.forward);
                chunk.InstantiateBlock(pos + Vector3.back);
            }
            else if (Mathf.Abs(direction.z) > 0)
            {
                chunk.InstantiateBlock(pos + Vector3.right);
                chunk.InstantiateBlock(pos + Vector3.left);
            }
        }
    }
    public bool ValidateBridge(Vector3Int pos)
    {
        bool contains = false;
        foreach (var chunk in positionsChunks)
        {
            if (chunk.positionChunk == pos)
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    List<PositionChunk> GenerateMap(int ancho, int alto)
    {
        Vector3Int inicio = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        Vector3Int fin = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        while (inicio == fin)
        {
            fin = new Vector3Int(Random.Range(0, ancho), 0, Random.Range(0, alto));
        }

        List<PositionChunk> camino = new List<PositionChunk>
        {
            new PositionChunk { positionChunk = inicio }
        };
        HashSet<Vector3Int> visitados = new HashSet<Vector3Int> { inicio };
        Vector3Int actual = inicio;

        while (actual != fin)
        {
            List<Vector3Int> movimientos = new List<Vector3Int>
        {
            new Vector3Int(actual.x - 1, 0, actual.z),
            new Vector3Int(actual.x + 1, 0, actual.z),
            new Vector3Int(actual.x, 0, actual.z - 1),
            new Vector3Int(actual.x, 0, actual.z + 1)
        };

            movimientos = movimientos
                .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                .OrderBy(_ => Random.value)
                .ToList();

            if (movimientos.Count == 0)
            {
                Debug.LogError("No hay movimientos válidos restantes.");
                break;
            }

            movimientos = movimientos.OrderBy(mov => Vector3Int.Distance(mov, fin) + Random.Range(-0.5f, 0.5f)).ToList();
            actual = movimientos[0];

            visitados.Add(actual);
            camino.Add(new PositionChunk { positionChunk = actual });
        }

        int cantidadRamas = Random.Range(3, 6);
        for (int i = 0; i < cantidadRamas; i++)
        {
            int indiceCamino = Random.Range(1, camino.Count - 1);
            Vector3Int puntoRama = camino[indiceCamino].positionChunk;
            int longitudRama = Random.Range(2, 5);

            Vector3Int ramaActual = puntoRama;
            for (int j = 0; j < longitudRama; j++)
            {
                List<Vector3Int> movimientosRama = new List<Vector3Int>
            {
                new Vector3Int(ramaActual.x - 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x + 1, 0, ramaActual.z),
                new Vector3Int(ramaActual.x, 0, ramaActual.z - 1),
                new Vector3Int(ramaActual.x, 0, ramaActual.z + 1)
            };

                movimientosRama = movimientosRama
                    .Where(mov => mov.x >= 0 && mov.x < ancho && mov.z >= 0 && mov.z < alto && !visitados.Contains(mov))
                    .OrderBy(_ => Random.value)
                    .ToList();

                if (movimientosRama.Count == 0) break;

                ramaActual = movimientosRama[0];
                visitados.Add(ramaActual);
                camino.Add(new PositionChunk { positionChunk = ramaActual });
            }
        }

        return camino;
    }
    [NaughtyAttributes.Button]
    public void CombineObjectsInChunks()
    {
        for (int i = 0; i < positionsChunks.Count; i++)
        {
            positionsChunks[i].managementChunk.CheckObjectInsideChunk();
        }
    }
    [System.Serializable]
    public class DirectionChunk
    {
        public int pos = 0;
        public ValidateDirectionChunk validateDirectionChunk;
    }
    [System.Serializable]
    public class PositionChunk
    {
        public Vector3Int positionChunk = new Vector3Int();
        public ManagementChunk managementChunk;
    }
    public enum ValidateDirectionChunk
    {
        Forward = 0,
        Back = 1,
        Left = 2,
        Rigth = 3
    }
}