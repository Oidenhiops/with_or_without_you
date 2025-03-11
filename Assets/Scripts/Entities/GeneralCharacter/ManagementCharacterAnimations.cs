using System.Collections;
using UnityEngine;

public class ManagementCharacterAnimations : MonoBehaviour, Character.ICharacterAnimations
{
    [SerializeField] Character character;
    [SerializeField] GameObject characterSprite;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Mesh originalMesh;
    public CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo characterAnimationsInfo;
    ManagementCharacterModelDirection.ICharacterDirection modelDirection;
    public CharacterAnimationsInfoScriptableObject.AnimationsInfo currentAnimation;
    [SerializeField] CharacterAnimationsEffectsInfo characterAnimationsEffectsInfo;
    void Awake()
    {
        modelDirection = GetComponent<ManagementCharacterModelDirection.ICharacterDirection>();
    }
    void Update()
    {
        if (character.characterInfo.isInitialize)
        {
            if (character.characterInfo.isPlayer && character.characterInputs != null)
            {
                if (!currentAnimation.needAnimationEnd)
                {
                    if (character.characterInputs.characterActionsInfo.movement != Vector2.zero)
                    {
                        if (!GetCurrentAnimation(TypeAnimation.Walk))
                        {
                            MakeAnimation(TypeAnimation.Walk);
                        }
                    }
                    else
                    {
                        if (!GetCurrentAnimation(TypeAnimation.Idle))
                        {
                            MakeAnimation(TypeAnimation.Idle);
                        }
                    }
                }
            }
            else
            {
                if (!currentAnimation.needAnimationEnd)
                {
                    if (modelDirection.GetDirectionMovementCharacter() != Vector2.zero)
                    {
                        if (!GetCurrentAnimation(TypeAnimation.Walk))
                        {
                            MakeAnimation(TypeAnimation.Walk);
                        }
                    }
                    else
                    {
                        if (!GetCurrentAnimation(TypeAnimation.Idle))
                        {
                            MakeAnimation(TypeAnimation.Idle);
                        }
                    }
                }
            }
        }
    }
    public void SetInitialData(InitialDataScriptableOject initialData)
    {
        StopAllCoroutines();
        meshRenderer.material.SetTexture("_BaseTexture", initialData.atlas);
        characterAnimationsInfo = new CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo
        {
            animations = initialData.characterAnimations.animationsInfo.animations,
            baseSpritePerTime = initialData.characterAnimations.animationsInfo.baseSpritePerTime,
            currentSpritePerTime = initialData.characterAnimations.animationsInfo.currentSpritePerTime,
            currentSpriteIndex = 0
        };
        GetAnimation(TypeAnimation.Attack).speedSpritesPerTimeMultplier = character.characterInfo.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
        currentAnimation = GetAnimation(TypeAnimation.Idle);
        StartCoroutine(AnimateSprite());
    }
    void ValidateDestroyInstanceAnimation()
    {
        if (currentAnimation.needInstance)
        {
            Destroy(currentAnimation.instance);
        }
    }
    void MakeAnimationEffect(TypeAnimationsEffects typeAnimationsEffects, float duration)
    {
        string nombreMetodo = typeAnimationsEffects.ToString();
        var metodo = this.GetType().GetMethod(nombreMetodo, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (metodo != null)
        {
            if (metodo.ReturnType == typeof(IEnumerator))
            {
                StartCoroutine((IEnumerator)metodo.Invoke(this, new object[] { duration }));
            }
            else
            {
                metodo.Invoke(this, new object[] { duration });
            }
        }
    }
    AnimationEffectInfo GetAnimationEffectInfo(TypeAnimationsEffects typeAnimationsEffects)
    {
        foreach (AnimationEffectInfo animationInfo in characterAnimationsEffectsInfo.animationEffectsInfo)
        {
            if (animationInfo.typeAnimationsEffects == typeAnimationsEffects) return animationInfo;
        }
        return null;
    }
    void SetTextureFromAtlas(Sprite spriteFromAtlas)
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

        // Asigna las UVs actualizadas a la malla
        meshRenderer.GetComponent<MeshFilter>().mesh.uv = uvs;
    }
    bool GetCurrentAnimation(TypeAnimation typeAnimation)
    {
        return currentAnimation.typeAnimation == typeAnimation;
    }
    public bool ValidateAnimationEnd(TypeAnimation typeAnimation)
    {
        return currentAnimation.typeAnimation != typeAnimation;
    }
    public void MakeAnimation(TypeAnimation typeAnimation)
    {
        currentAnimation = GetAnimation(typeAnimation);
        ValidateDestroyInstanceAnimation();
        StopAllCoroutines();
        StartCoroutine(AnimateSprite());
    }
    public GameObject GetCharacterSprite()
    {
        return characterSprite;
    }
    public CharacterAnimationsInfoScriptableObject.AnimationsInfo GetAnimation(TypeAnimation typeAnimation)
    {
        foreach (CharacterAnimationsInfoScriptableObject.AnimationsInfo animation in characterAnimationsInfo.animations)
        {
            if (animation.typeAnimation == typeAnimation)
            {
                return animation;
            }
        }
        return null;
    }
    public CharacterAnimationsInfoScriptableObject.AnimationsInfo GetCurrentAnimation()
    {
        return currentAnimation;
    }
    public CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo GetAnimationsInfo()
    {
        return characterAnimationsInfo;
    }
    #region AnimationsEffects
    IEnumerator Shake(float duration)
    {
        float tiempoTranscurrido = 0f;
        Vector3 initialPos = characterSprite.transform.localPosition;
        AnimationEffectInfo effectInfo = GetAnimationEffectInfo(TypeAnimationsEffects.Shake);

        while (tiempoTranscurrido < duration)
        {
            float desplazamientoX = Mathf.Sin(Time.time * effectInfo.frequency) * effectInfo.amplitude;
            characterSprite.transform.localPosition = initialPos + new Vector3(desplazamientoX, 0, 0);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        initialPos.x = 0f;
        characterSprite.transform.localPosition = initialPos;
    }
    IEnumerator Blink(float duration)
    {
        float tiempoTranscurrido = 0f;
        Material material = characterSprite.GetComponent<MeshRenderer>().material;
        AnimationEffectInfo effectInfo = GetAnimationEffectInfo(TypeAnimationsEffects.Blink);
        duration = characterAnimationsInfo.currentSpritePerTime * GetAnimation(TypeAnimation.TakeDamage).spritesDown.Length;
        while (tiempoTranscurrido < duration)
        {
            if (material.color == Color.white)
            {
                material.SetColor("_Color", effectInfo.colorBlink);
            }
            else
            {
                material.SetColor("_Color", Color.white);
            }
            tiempoTranscurrido += characterAnimationsInfo.currentSpritePerTime;
            yield return new WaitForSeconds(characterAnimationsInfo.currentSpritePerTime);
        }
        material.SetColor("_Color", Color.white);
    }
    #endregion
    IEnumerator AnimateSprite()
    {
        characterAnimationsInfo.currentSpriteIndex = 0;
        Sprite[] sprites = modelDirection.GetDirectionAnimation().y > 0 ? currentAnimation.spritesUp : currentAnimation.spritesDown;
        SetTextureFromAtlas(sprites[characterAnimationsInfo.currentSpriteIndex]);
        if (currentAnimation.typeAnimation == TypeAnimation.Attack)
        {
            GetAnimation(TypeAnimation.Attack).speedSpritesPerTimeMultplier = character.characterInfo.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
        }
        characterAnimationsInfo.currentSpritePerTime = characterAnimationsInfo.baseSpritePerTime / currentAnimation.speedSpritesPerTimeMultplier;
        if (currentAnimation.needInstance)
        {
            if (!currentAnimation.needFrameToInstance)
            {
                currentAnimation.instance = Instantiate(currentAnimation.instanceObj, characterSprite.transform.position, Quaternion.identity, characterSprite.transform);
                currentAnimation.instance.transform.GetChild(0).transform.localRotation = characterSprite.transform.localRotation;
                currentAnimation.instance.GetComponent<IAnimationInstance>().SetInfoForAnimation(modelDirection.GetDirectionAnimation(), characterAnimationsInfo);
            }
            else
            {
                StartCoroutine(ValidateFrameToInstance(currentAnimation.frameToInstance));
            }
        }
        while (true)
        {
            if (currentAnimation.typeAnimation != TypeAnimation.None)
            {
                if (characterAnimationsInfo.currentSpriteIndex == 0)
                {
                    if (currentAnimation.animationsEffects.Length > 0)
                    {
                        for (int i = 0; i < currentAnimation.animationsEffects.Length; i++)
                        {
                            MakeAnimationEffect(currentAnimation.animationsEffects[i], characterAnimationsInfo.currentSpritePerTime * currentAnimation.spritesDown.Length);
                        }
                    }
                }
                yield return new WaitForSeconds(characterAnimationsInfo.currentSpritePerTime);
                characterAnimationsInfo.currentSpriteIndex++;
                sprites = modelDirection.GetDirectionAnimation().y > 0 ? currentAnimation.spritesUp : currentAnimation.spritesDown;
                if (characterAnimationsInfo.currentSpriteIndex < currentAnimation.spritesDown.Length)
                {
                    SetTextureFromAtlas(sprites[characterAnimationsInfo.currentSpriteIndex]);
                }
                else if (currentAnimation.loop)
                {
                    characterAnimationsInfo.currentSpriteIndex = 0;
                    SetTextureFromAtlas(sprites[characterAnimationsInfo.currentSpriteIndex]);
                }
                else
                {
                    currentAnimation = new CharacterAnimationsInfoScriptableObject.AnimationsInfo();
                }
            }
            else
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
    IEnumerator ValidateFrameToInstance(int frame)
    {
        while (characterAnimationsInfo.currentSpriteIndex != frame)
        {
            yield return null;
        }
        currentAnimation.instance = Instantiate(currentAnimation.instanceObj, characterSprite.transform.position, Quaternion.identity, characterSprite.transform);
        currentAnimation.instance.transform.SetParent(characterSprite.transform);
        currentAnimation.instance.transform.localPosition = Vector3.zero;
        currentAnimation.instance.transform.SetParent(transform);
        currentAnimation.instance.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentAnimation.instance.transform.GetChild(0).transform.localRotation = characterSprite.transform.localRotation;
        currentAnimation.instance.GetComponent<IAnimationInstance>().SetInfoForAnimation(modelDirection.GetDirectionAnimation(), characterAnimationsInfo);
    }
    [System.Serializable]
    public class CharacterAnimationsEffectsInfo
    {
        public AnimationEffectInfo[] animationEffectsInfo = new AnimationEffectInfo[]
        {
            new AnimationEffectInfo()
            {
                typeAnimationsEffects = TypeAnimationsEffects.Shake,
                amplitude = 0.1f,
                frequency = 100.0f,
            },
            new AnimationEffectInfo()
            {
                typeAnimationsEffects = TypeAnimationsEffects.Blink,
                amplitude = 0.1f,
                frequency = 100.0f,
            }
        };
    }
    [System.Serializable]
    public class AnimationEffectInfo
    {
        public TypeAnimationsEffects typeAnimationsEffects;
        public float amplitude = 0;
        public float frequency = 0;
        public Color colorBlink = Color.white;
    }
    public enum TypeAnimationsEffects
    {
        None = 0,
        Shake = 1,
        Blink = 2
    }
    public enum TypeAnimation
    {
        None = 0,
        Idle = 1,
        Walk = 2,
        TakeDamage = 3,
        Attack = 4,
        DefaultSkillAttack = 5,
    }
    public interface IAnimationInstance
    {
        public void SetInfoForAnimation(Vector2 movement, CharacterAnimationsInfoScriptableObject.CharacterAnimationsInfo characterAnimationsInfo);
    }
}
