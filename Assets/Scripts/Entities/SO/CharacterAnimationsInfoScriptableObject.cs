using UnityEngine;

[CreateAssetMenu(fileName = "CharacterAnimations", menuName = "ScriptableObjects/Character/CharacterAnimationsSO", order = 1)]
public class CharacterAnimationsSO : ScriptableObject
{
    public CharacterAnimationsInfo animationsInfo;
    [System.Serializable]
    public class CharacterAnimationsInfo
    {
        public float baseSpritePerTime = 0.1f;
        public float currentSpritePerTime = 0.1f;
        public int currentSpriteIndex = 0;
        public AnimationsInfo[] animations;
    }
    [System.Serializable]
    public class AnimationsInfo
    {
        public ManagementCharacterAnimations.TypeAnimation typeAnimation;
        public Sprite[] spritesDown;
        public Sprite[] spritesUp;
        public ManagementCharacterAnimations.TypeAnimationsEffects[] animationsEffects;
        public bool needAnimationEnd = false;
        public bool loop = false;
        public bool needInstance = false;
        public bool needFrameToInstance = false;
        public int frameToInstance = 0;
        public float speedSpritesPerTimeMultplier = 1;
        public GameObject instanceObj;
        public GameObject instance;
    }
}
