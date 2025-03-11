using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementCharacterInstance : MonoBehaviour, ManagementCharacterAnimations.IAnimationInstance
{
    public GameObject objectInstanceSprite;
    public MeshRenderer meshRenderer;
    public Mesh originalMesh;
    public bool needSpritePerTime = false;
    public CharacterAnimationsInfoScriptableObject.AnimationsInfo objectInstanceAnimation;
    public CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo objectInstanceAnimationInfo;
    public void SetInfoForAnimation(Vector2 movement, CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo characterAnimationsInfo)
    {
        if (needSpritePerTime)
        {
            objectInstanceAnimationInfo.currentSpriteIndex = characterAnimationsInfo.currentSpriteIndex;
        }
        StartCoroutine(AnimateSprite(movement));
    }
    public IEnumerator AnimateSprite(Vector2 movement)
    {
        objectInstanceAnimationInfo.currentSpriteIndex = 0;
        Sprite[] sprites = movement.y > 0 ? objectInstanceAnimation.spritesUp : objectInstanceAnimation.spritesDown;
        SetTextureFromAtlas(sprites[objectInstanceAnimationInfo.currentSpriteIndex]);
        while (true)
        {
            yield return new WaitForSeconds(objectInstanceAnimationInfo.currentSpritePerTime);
            objectInstanceAnimationInfo.currentSpriteIndex++;
            sprites = movement.y > 0 ? objectInstanceAnimation.spritesUp : objectInstanceAnimation.spritesDown;
            if (objectInstanceAnimationInfo.currentSpriteIndex < objectInstanceAnimation.spritesDown.Length)
            {
                SetTextureFromAtlas(sprites[objectInstanceAnimationInfo.currentSpriteIndex]);
            }
            else if (objectInstanceAnimation.loop)
            {
                objectInstanceAnimationInfo.currentSpriteIndex = 0;
                SetTextureFromAtlas(sprites[objectInstanceAnimationInfo.currentSpriteIndex]);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public void SetTextureFromAtlas(Sprite spriteFromAtlas)
    {
        Vector2[] uvs = originalMesh.uv;
        Texture2D texture = spriteFromAtlas.texture;
        meshRenderer.material.mainTexture = texture;
        Rect spriteRect = spriteFromAtlas.rect;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i].x = Mathf.Lerp(spriteRect.x / texture.width, (spriteRect.x + spriteRect.width) / texture.width, uvs[i].x);
            uvs[i].y = Mathf.Lerp(spriteRect.y / texture.height, (spriteRect.y + spriteRect.height) / texture.height, uvs[i].y);
        }
        meshRenderer.GetComponent<MeshFilter>().mesh.uv = uvs;
    }
}
