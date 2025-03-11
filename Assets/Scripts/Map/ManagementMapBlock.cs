using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ManagementMapBlock : MonoBehaviour
{
    public ManagementMapBlock managementMapBlock;
    public bool isWalkable = false;
    public MeshRenderer meshRenderer;
    public BlockInfo blockInfo;
    private Vector3 boxSize = new Vector3(0.25f, 0.25f, 0.25f);
    private Vector3 offsetPos = new Vector3(0, 0.5f, 0);
    [SerializeField] List<TypeDirections> directionsBlocks = new List<TypeDirections>();

    // Booleans para activar direcciones espec�ficas
    public bool detectUp = false;
    public bool detectDown = false;
    public bool detectLeft = false;
    public bool detectRight = false;
    public bool detectForward = false;
    public bool detectBack = false;
    public bool detectUpLeft = false;
    public bool detectUpRight = false;
    public bool detectDownLeft = false;
    public bool detectDownRight = false;
    public bool detectUpForward = false;
    public bool detectUpBack = false;
    public bool detectDownForward = false;
    public bool detectDownBack = false;
    public bool detectForwardLeft = false;
    public bool detectForwardRigth = false;
    public bool detectBackLeft = false;
    public bool detectBackRigth = false;
    private Vector3 up = new Vector3(0, 1, 0);
    private Vector3 down = new Vector3(0, -1, 0);
    private Vector3 left = new Vector3( -1, 0, 0);
    private Vector3 right = new Vector3(1, 0, 0);
    private Vector3 forward = new Vector3( 0, 0, 1);
    private Vector3 back = new Vector3( 0, 0, -1);
    public bool drawGizmos;
    public int blockSelectedIndex;

    private void Start()
    {
        
    }

    [NaughtyAttributes.Button]
    public void DrawBlock()
    {
        DetectInSpecifiedDirections();
    }
    public void DetectInSpecifiedDirections()
    {
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            isWalkable = false;
        }
        directionsBlocks.Clear();
        if (detectUp) CheckDirection(up, TypeDirections.Up);
        if (!directionsBlocks.Contains(TypeDirections.Up))
        {
            if (detectDown) CheckDirection(down, TypeDirections.Down);
            if (detectLeft) CheckDirection(left, TypeDirections.Left);
            if (detectRight) CheckDirection(right, TypeDirections.Rigth);
            if (detectForward) CheckDirection(forward, TypeDirections.Forward);
            if (detectBack) CheckDirection(back, TypeDirections.Back);
            if (detectUpLeft) CheckDirection((up + left), TypeDirections.UpLeft);
            if (detectUpRight) CheckDirection((up + Vector3.right), TypeDirections.UpRigth);
            if (detectDownLeft) CheckDirection((down + left), TypeDirections.DownLeft);
            if (detectDownRight) CheckDirection((down + right), TypeDirections.DownRigth);
            if (detectUpForward) CheckDirection((up + forward), TypeDirections.UpForward);
            if (detectUpBack) CheckDirection((up + back), TypeDirections.UpBack);
            if (detectDownForward) CheckDirection((down + forward), TypeDirections.DownForward);
            if (detectDownBack) CheckDirection((down + back), TypeDirections.DownBack);
            if (detectForwardLeft) CheckDirection((forward + left), TypeDirections.ForwardLeft);
            if (detectForwardRigth) CheckDirection((forward + right), TypeDirections.ForwardRight);
            if (detectBackLeft) CheckDirection((back + left), TypeDirections.BackLeft);
            if (detectBackRigth) CheckDirection((back + right), TypeDirections.BackRight);
        }
        SetTextureFromAtlas(meshRenderer,GetTextureByDirection(), false);
        DrawTextures();
    }
    void ValidateCanGeneratePath()
    {

    }
    [NaughtyAttributes.Button]
    void DrawTextures()
    {
        VariationsTextures variationsTextures = GetVariationsByTexture(blockInfo.textures[blockSelectedIndex].spriteFromAtlas);
        if (variationsTextures != null)
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
            }
            Random.InitState(System.DateTime.Now.Millisecond);
            float probability = Random.Range(0, 100);
            meshRenderer.sharedMaterial = variationsTextures.atlasMaterialVariations;
            List<Variations> variations = GetVariationsByProbability(variationsTextures.variations, probability);
            int indexVariation = Random.Range(0, variations.Count);
            Variations variation = variations[indexVariation];
            SetTextureFromAtlas(meshRenderer, variation.sprite, false);
            if (variation.needInstance)
            {
                GameObject instanceVariation = Instantiate(variation.instance, transform.position, Quaternion.identity, transform);
                instanceVariation.transform.position += Vector3.up;
                isWalkable = variations[indexVariation].isWalkable;
                if (variation.needInstanceTexture)
                {
                    SetTextureFromAtlas(instanceVariation.GetComponent<MeshRenderer>(), variation.instanceSpriteFromAtlas, true);
                }
            }
            else
            {
                isWalkable = true;
            }
        }
        else
        {
            isWalkable = true;
        }
    }
    void CheckDirection(Vector3 direction, TypeDirections directionBlock)
    {
        if (DetectManagementMapBlock(GetAdjustedDirection(direction), out GameObject detectedObject))
        {
            if (detectedObject.GetComponent<ManagementMapBlock>().blockInfo.typeBlock == blockInfo.typeBlock)
            {
                directionsBlocks.Add(directionBlock);
            }
        }
    }
    Vector3 GetAdjustedDirection(Vector3 direction)
    {
        return transform.rotation * direction;
    }
    public VariationsTextures GetVariationsByTexture(Sprite mainTexture)
    {
        foreach(VariationsTextures variations in blockInfo.variationsTextures)
        {
            if (variations.mainSprite == mainTexture)
            {
                return variations;
            }
        }
        return null;
    }
    public Sprite GetTextureByDirection()
    {
        if (directionsBlocks.Count > 0)
        {
            for (int i = 0; i < blockInfo.textures.Length; i++)
            {
                if (EqualsDirections(blockInfo.textures[i].rulesDirections,directionsBlocks.ToArray()))
                {
                    blockSelectedIndex = i;                    
                    return blockInfo.textures[i].spriteFromAtlas;
                }
            }
        }
        return blockInfo.textures[0].spriteFromAtlas;
    }    
    public List<Variations> GetVariationsByProbability(Variations[] variationsTextures, float prob)
    {
        List<Variations> variationsList = new List<Variations>();
        foreach(Variations variation in variationsTextures)
        {
            if (variation.probability >= prob)
            {
                variationsList.Add(variation);
            }
        }
        return variationsList;
    }
    public Mesh GetMeshByTexture(Sprite mainTexture, bool isDecorationMesh)
    {
        for (int i = 0; i < blockInfo.meshes.Length; i++)
        {
            if (blockInfo.meshes[i].spriteKey == mainTexture)
            {
                if (!isDecorationMesh)
                {
                    Mesh copia = new Mesh();
                    copia.vertices = blockInfo.meshes[i].mesh.vertices;
                    copia.triangles = blockInfo.meshes[i].mesh.triangles;
                    copia.uv = blockInfo.meshes[i].mesh.uv;
                    copia.normals = blockInfo.meshes[i].mesh.normals;
                    copia.colors = blockInfo.meshes[i].mesh.colors;
                    copia.tangents = blockInfo.meshes[i].mesh.tangents;
                    copia.bounds = blockInfo.meshes[i].mesh.bounds;
                    copia.boneWeights = blockInfo.meshes[i].mesh.boneWeights;
                    copia.bindposes = blockInfo.meshes[i].mesh.bindposes;
                    return copia;
                }
                else
                {
                    Mesh copia = new Mesh();
                    copia.vertices = blockInfo.meshes[i].meshDecoration.vertices;
                    copia.triangles = blockInfo.meshes[i].meshDecoration.triangles;
                    copia.uv = blockInfo.meshes[i].meshDecoration.uv;
                    copia.normals = blockInfo.meshes[i].meshDecoration.normals;
                    copia.colors = blockInfo.meshes[i].meshDecoration.colors;
                    copia.tangents = blockInfo.meshes[i].meshDecoration.tangents;
                    copia.bounds = blockInfo.meshes[i].meshDecoration.bounds;
                    copia.boneWeights = blockInfo.meshes[i].meshDecoration.boneWeights;
                    copia.bindposes = blockInfo.meshes[i].meshDecoration.bindposes;
                    return copia;
                }
            }
        }
        return null;
    }
    public void SetTextureFromAtlas(MeshRenderer meshRenderer,Sprite spriteFromAtlas, bool isDecorationMesh)
    {
        Mesh newMesh = GetMeshByTexture(spriteFromAtlas, isDecorationMesh);
        Vector2[] uvs = meshRenderer.GetComponent<MeshFilter>().mesh.uv;        
        if (newMesh != null)
        {
            meshRenderer.GetComponent<MeshFilter>().mesh = newMesh;
            uvs = newMesh.uv;
            if (GetComponent<MeshCollider>() != null)
            {
                GetComponent<MeshCollider>().sharedMesh = newMesh;
            }
        }
        //Obt�n la textura del sprite
        Texture2D texture = spriteFromAtlas.texture;

        // Asigna la textura al material
        meshRenderer.material.mainTexture = texture;

        // Ajustar las coordenadas UV seg�n el sprite seleccionado
        Rect spriteRect = spriteFromAtlas.textureRect;

        // Normalizar las UVs en el rango [0,1] basadas en el rect�ngulo del sprite
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }

        // Asigna las UVs actualizadas a la malla
        meshRenderer.GetComponent<MeshFilter>().mesh.uv = uvs;
    }
    public bool EqualsDirections(TypeDirections[] directions1, TypeDirections[] directions2)
    {
        // Convierte ambas listas a conjuntos y compara si son iguales
        return new HashSet<TypeDirections>(directions1).SetEquals(directions2);
    }
    public bool DetectManagementMapBlock(Vector3 direction, out GameObject detectedObject)
    {
        detectedObject = null;
        Vector3 targetPosition = transform.position + offsetPos + direction;
        Collider[] hitColliders = Physics.OverlapBox(targetPosition, boxSize, Quaternion.identity);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<ManagementMapBlock>() != null)
            {
                detectedObject = collider.gameObject;
                return true;
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;
            Vector3 center = transform.position;

            // Dibujar Gizmos solo para las direcciones activadas
            if (detectUp) DrawGizmo(center, up);
            if (detectDown) DrawGizmo(center, down);
            if (detectLeft) DrawGizmo(center, left);
            if (detectRight) DrawGizmo(center, right);
            if (detectForward) DrawGizmo(center, forward);
            if (detectBack) DrawGizmo(center, back);
            if (detectUpLeft) DrawGizmo(center, (up + left));
            if (detectUpRight) DrawGizmo(center, (up + right));
            if (detectDownLeft) DrawGizmo(center, (down + left));
            if (detectDownRight) DrawGizmo(center, (down + right));
            if (detectUpForward) DrawGizmo(center, (up + forward));
            if (detectUpBack) DrawGizmo(center, (up + back));
            if (detectDownForward) DrawGizmo(center, (down + forward));
            if (detectDownBack) DrawGizmo(center, (down + back));
            if (detectForwardLeft) DrawGizmo(center, (forward + left));
            if (detectForwardRigth) DrawGizmo(center, (forward + right));
            if (detectBackLeft) DrawGizmo(center, (back + left));
            if (detectBackRigth) DrawGizmo(center, (back + right));
        }
    }

    // M�todo auxiliar para dibujar los Gizmos
    private void DrawGizmo(Vector3 center, Vector3 direction)
    {
        // Calcula la posici�n de destino
        Vector3 destination = center + offsetPos + direction;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(destination, boxSize);
    }
    [System.Serializable]
    public class BlockInfo
    {
        public Material atlasMaterial;
        public TypeBlock typeBlock;
        public TexturesInfo[] textures;
        public VariationsTextures[] variationsTextures;
        public BlockMeshes[] meshes;
    }
    [System.Serializable]
    public class TexturesInfo
    {
        public TypeDirections[] rulesDirections;
        public Sprite spriteFromAtlas;
    }
    [System.Serializable]
    public class VariationsTextures 
    {        
        public Material atlasMaterialVariations;
        public Sprite mainSprite;
        public Variations[] variations;
    }
    [System.Serializable]
    public class Variations
    {
        public float probability;
        public Sprite sprite;
        public bool needInstance;
        public GameObject instance;
        public bool needInstanceTexture;
        public Sprite instanceSpriteFromAtlas;
        public bool isWalkable = false;
    }
    [System.Serializable]
    public class BlockMeshes
    {
        public Sprite spriteKey;
        public Mesh mesh;
        public Mesh meshDecoration;
    }
    public enum TypeDirections
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Rigth = 4,
        Forward = 5,
        Back = 6,
        UpLeft = 7,
        UpRigth = 8,
        DownLeft = 9,
        DownRigth = 10,
        UpForward = 11,
        UpBack = 12,
        DownForward = 13,
        DownBack = 14,
        ForwardLeft = 15,
        ForwardRight = 16,
        BackLeft = 17,
        BackRight = 18,
    }
    public enum TypeBlock
    {
        Void = 0,
        Block = 1,
        Stairs = 2
    }
}
