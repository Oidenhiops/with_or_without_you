using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [NonSerialized] public CharacterInputs characterInputs;
    public CharacterInfo characterInfo;
    void Awake()
    {
        characterInfo.characterObject = gameObject;
        characterInfo.managementCharacterHud = GetComponent<ManagementCharacterHud>();
        characterInfo.managementCharacterModelDirection = GetComponent<ManagementCharacterModelDirection>();
        characterInfo.managementCharacterSounds = GetComponent<ManagementCharacterSounds>();
        characterInfo.managementCharacterObjects = GetComponent<ManagementCharacterObjects>();
        characterInfo.managementCharacterSkills = GetComponent<ManagementCharacterSkills>();
        characterInfo.dissolve = GetComponent<Dissolve>();
        characterInfo.characterAnimations = GetComponent<ICharacterAnimations>();
        characterInfo.characterAttack = GetComponent<ICharacterAttack>();
        characterInfo.characterMove = GetComponent<ICharacterMove>();
        characterInfo.owner = this;
        characterInputs = GetComponent<CharacterInputs>();
        if (TryGetComponent<ManagementPlayerCamera>(out ManagementPlayerCamera IPlayerCamera)) characterInfo.managementPlayerCamera = IPlayerCamera;
    }
    void Start()
    {
        InitializeCharacter();
    }
    [NaughtyAttributes.Button]
    void ActivePlayer()
    {
        characterInfo.characterMove.GetRigidbody().isKinematic = false;
        characterInfo.isActive = true;
    }
    void Update()
    {
        if (characterInfo.isInitialize) HandleHud();
        if (characterInfo.isActive)
        {
            HandleMove();
            HandleAttack();
            HandleObjects();
            HandleSkills();
            HandleCamera();
        }
    }
    void InitializeCharacter()
    {
        if (characterInputs != null) characterInputs.characterActions.Enable();
        if (characterInfo.isPlayer)
        {
            if (ManagementData.saveData.gameInfo.characterInfo.characterSelected != null)
            {
                characterInfo.initialData = ManagementData.saveData.gameInfo.characterInfo.characterSelected.Clone();
            }
            else
            {
                characterInfo.initialData = characterInfo.initialData.Clone();
            }
        }
        characterInfo.InitializeStatistics();
        if (characterInfo.isPlayer && characterInfo.managementCharacterHud != null)
        {
            characterInfo.managementCharacterHud.RefreshCurrentStatistics();
        }
        InitializeSounds();
        InitializeAnimations();
        InitializeObjects();
        InitializeSkills();
        characterInfo.regenerateResources = StartCoroutine(characterInfo.RegenerateResources());
        characterInfo.isInitialize = true;
    }
    void InitializeSounds()
    {
        characterInfo.managementCharacterSounds.soundsInfo = characterInfo.initialData.characterSounds;
    }
    void InitializeAnimations()
    {
        characterInfo.characterAnimations.SetInitialData(characterInfo.initialData);
    }
    void InitializeObjects()
    {
        if (characterInfo.managementCharacterObjects != null) characterInfo.managementCharacterObjects.InitializeObjects();
    }
    void InitializeSkills()
    {
        if (characterInfo.isPlayer && characterInfo.managementCharacterSkills != null && ManagementData.saveData.gameInfo.characterInfo.characterSelected != null) characterInfo.managementCharacterSkills.InitializeSkills();
    }
    void HandleMove()
    {
        if (characterInfo.characterMove != null)
        {
            characterInfo.characterMove.Move();
        }
    }
    public void HandleAttack()
    {
        if (characterInfo.isPlayer)
        {
            if (characterInputs == null) return;
            if (characterInputs.characterActionsInfo.principalAttack.triggered)
            {
                characterInfo.characterAttack.ValidateAttack();
            }
        }
        else
        {
            characterInfo.characterAttack.ValidateAttack();
        }
    }
    void HandleObjects()
    {
        if (characterInfo.managementCharacterObjects) characterInfo.managementCharacterObjects.HandleObjects();
    }
    void HandleSkills()
    {
        if (characterInfo.managementCharacterSkills) characterInfo.managementCharacterSkills.HandleSkills();
    }
    void HandleHud()
    {
        if (characterInfo.managementCharacterHud) characterInfo.managementCharacterHud.HandleHud();
    }
    void HandleCamera()
    {
        if (characterInfo.isPlayer && characterInfo.managementPlayerCamera) characterInfo.managementPlayerCamera.MoveCamera();
    }
    [Serializable] public class CharacterInfo
    {
        #region characterScripts
        [HideInInspector] public ManagementCharacterHud managementCharacterHud;
        [HideInInspector] public ManagementCharacterModelDirection managementCharacterModelDirection;
        [HideInInspector] public ManagementCharacterSounds managementCharacterSounds;
        [HideInInspector] public ManagementCharacterObjects managementCharacterObjects;
        [HideInInspector] public ManagementCharacterSkills managementCharacterSkills;
        [HideInInspector] public ManagementPlayerCamera managementPlayerCamera;
        [HideInInspector] public GameObject characterObject;
        [HideInInspector] public Dissolve dissolve;
        [HideInInspector] public ICharacterAnimations characterAnimations;
        [HideInInspector] public ICharacterAttack characterAttack;
        [HideInInspector] public ICharacterMove characterMove;
        [HideInInspector] public MonoBehaviour owner;
        #endregion
        public InitialDataSO initialData;
        public Coroutine hitStop;
        public Coroutine regenerateResources;
        public bool isPlayer = false;
        public bool isInitialize = false;
        public bool isActive = false;
        public bool isPushed = false;
        public SerializedDictionary<TypeStatistics, Statistics> characterStatistics = new SerializedDictionary<TypeStatistics, Statistics>{
                {TypeStatistics.Hp, new Statistics (TypeStatistics.Hp, 0, 0, 0, 0, 0)},
                {TypeStatistics.Sp, new Statistics (TypeStatistics.Sp, 0, 0, 0, 0, 0)},
                {TypeStatistics.Mp, new Statistics (TypeStatistics.Mp, 0, 0, 0, 0, 0)},
                {TypeStatistics.Atk, new Statistics (TypeStatistics.Atk, 0, 0, 0, 0, 0)},
                {TypeStatistics.AtkSpd, new Statistics (TypeStatistics.AtkSpd, 0, 0, 0, 0, 0)},
                {TypeStatistics.Int, new Statistics (TypeStatistics.Int, 0, 0, 0, 0, 0)},
                {TypeStatistics.Def, new Statistics (TypeStatistics.Def, 0, 0, 0, 0, 0)},
                {TypeStatistics.Res, new Statistics (TypeStatistics.Res, 0, 0, 0, 0, 0)},
                {TypeStatistics.Spd, new Statistics (TypeStatistics.Spd, 0, 0, 0, 0, 0)},
                {TypeStatistics.Crit, new Statistics (TypeStatistics.Crit, 0, 0, 0, 0, 0)},
                {TypeStatistics.CritDmg, new Statistics (TypeStatistics.CritDmg, 0, 0, 0, 0, 0)},
            };
        public Color colorBlood = Color.white;
        #region Constructors
        public Statistics GetStatisticByType(TypeStatistics typeStatistics)
        {
            if (characterStatistics.TryGetValue(typeStatistics, out Statistics statistics))
            {
                return statistics;
            }
            return null;
        }
        public void InitializeStatistics()
        {            
            for (int i = 0; i < characterStatistics.Count; i++)
            {
                TypeStatistics key = characterStatistics.Keys.ElementAt(i);
                characterStatistics[key] = new Statistics(
                    initialData.characterInfo.characterStatistics[key].typeStatistics,
                    initialData.characterInfo.characterStatistics[key].baseValue,
                    initialData.characterInfo.characterStatistics[key].buffValue,
                    initialData.characterInfo.characterStatistics[key].objectValue,
                    initialData.characterInfo.characterStatistics[key].currentValue,
                    initialData.characterInfo.characterStatistics[key].maxValue
                );
                if (characterStatistics[key].maxValue == 0)
                {
                    characterStatistics[key].maxValue = characterStatistics[key].baseValue;
                    characterStatistics[key].currentValue = characterStatistics[key].baseValue;
                }
            }
        }
        public void RefreshCurrentStatistics()
        {
            foreach (var key in characterStatistics.Keys.ToList())
            {
                float basicsValue = characterStatistics[key].baseValue + characterStatistics[key].objectValue;
                float operationValue = basicsValue + basicsValue * characterStatistics[key].buffValue / 100;
                float finalValue = characterStatistics[key].typeStatistics == TypeStatistics.AtkSpd ? operationValue : Mathf.Ceil(operationValue);
                characterStatistics[key].maxValue = finalValue;
                if (characterStatistics[key].typeStatistics != TypeStatistics.Hp && characterStatistics[key].typeStatistics != TypeStatistics.Mp && characterStatistics[key].typeStatistics != TypeStatistics.Sp)
                {
                    characterStatistics[key].currentValue = finalValue;
                }
                else if (characterStatistics[key].currentValue > characterStatistics[key].maxValue)
                {
                    characterStatistics[key].currentValue = characterStatistics[key].maxValue;
                }
            }
        }
        public void TakeDamage(float damage, Color color, float timeHitStop, TypeDamage typeDamage)
        {
            if (isActive)
            {
                int calculatedDamage = (int)Mathf.Ceil(CalculateDamage(damage, typeDamage));
                if (timeHitStop > 0)
                {
                    if (hitStop != null)
                    {
                        owner.StopCoroutine(hitStop);
                    }
                    Time.timeScale = 0;
                    hitStop = owner.StartCoroutine(ApplyHitStop(timeHitStop));
                }
                GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), characterObject.transform.position, Quaternion.identity);
                FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
                floatingTextScript.SendText(floatingTextScript.takeDamage, calculatedDamage.ToString(), color);
                Destroy(floatingText, 2);
                GetStatisticByType(TypeStatistics.Hp).currentValue -= calculatedDamage;
                if (GetStatisticByType(TypeStatistics.Hp).currentValue <= 0)
                {
                    GetStatisticByType(TypeStatistics.Hp).currentValue = 0;
                    Die();
                }
                characterAnimations.MakeAnimation(ManagementCharacterAnimations.TypeAnimation.TakeDamage);
                PlayASound(CharacterSoundsSO.TypeSound.TakeDamage, true);
            }
        }
        float CalculateDamage(float damage, TypeDamage typeDamage)
        {
            if (typeDamage == TypeDamage.TrueDamage)
                return damage;

            TypeStatistics targetStat = typeDamage == TypeDamage.Physic ? TypeStatistics.Def : TypeStatistics.Res;
            Statistics def = GetStatisticByType(targetStat);
            float reduction = def.currentValue / (def.currentValue + 200);
            return damage * (1 - reduction);
        }
        void Die()
        {
            PlayASound(CharacterSoundsSO.TypeSound.Die, true);
            isActive = false;
            characterObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            characterObject.GetComponent<Rigidbody>().isKinematic = true;
            characterObject.GetComponent<Collider>().enabled = false;
            if (isPlayer)
            {
                owner.Invoke("ReloadScene", managementCharacterSounds.GetAudioClip(CharacterSoundsSO.TypeSound.Die).length + 1);
            }
            else
            {
                Destroy(characterObject, 1);
            }
            characterAnimations.GetAnimation(ManagementCharacterAnimations.TypeAnimation.TakeDamage).loop = true;
            dissolve.DissolveObject();
            GameObject bloodInstance = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BloodDieEffect/BloodDieEffect"), characterObject.transform.position, Quaternion.identity);
            bloodInstance.transform.position += Vector3.up / 2;
            var particleSystem = bloodInstance.GetComponent<ParticleSystem>();
            var mainModule = particleSystem.main;
            mainModule.startColor = colorBlood;
        }
        public void PlayASound(CharacterSoundsSO.TypeSound typeSound, bool randomPitch)
        {
            GameObject blockSound = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BlockSound/BlockSound"));
            AudioClip audioClip = managementCharacterSounds.GetAudioClip(typeSound);
            blockSound.GetComponent<ManagementBlockSound>().PlaySound(audioClip, randomPitch);
            Destroy(blockSound, audioClip.length);
        }
        public void PlayASound(AudioClip sound, bool randomPitch)
        {
            GameObject blockSound = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BlockSound/BlockSound"));
            AudioClip audioClip = sound;
            blockSound.GetComponent<ManagementBlockSound>().PlaySound(audioClip, randomPitch);
            Destroy(blockSound, audioClip.length);
        }
        public void PlayASound(AudioClip sound, float initialPitch, bool randomPitch)
        {
            GameObject blockSound = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BlockSound/BlockSound"));
            AudioClip audioClip = sound;
            blockSound.GetComponent<ManagementBlockSound>().PlaySound(audioClip, initialPitch, randomPitch);
            Destroy(blockSound, audioClip.length);
        }
        public IEnumerator RegenerateResources()
        {
            var hp = GetStatisticByType(TypeStatistics.Hp);
            var mp = GetStatisticByType(TypeStatistics.Mp);
            var sp = GetStatisticByType(TypeStatistics.Sp);
            while (hp.currentValue > 0)
            {
                yield return new WaitForSeconds(1);
                if (hp.currentValue < hp.maxValue)
                {
                    hp.currentValue += hp.baseValue * 0.001f;
                    if (hp.currentValue > hp.maxValue)
                    {
                        hp.currentValue = hp.maxValue;
                    }
                }
                if (mp.currentValue < mp.maxValue)
                {
                    mp.currentValue += mp.baseValue * 0.1f;
                    if (mp.currentValue > mp.maxValue)
                    {
                        mp.currentValue = mp.maxValue;
                    }
                }
                if (sp.currentValue < sp.maxValue)
                {
                    sp.currentValue += sp.baseValue * 0.1f;
                    if (sp.currentValue > sp.maxValue)
                    {
                        sp.currentValue = sp.maxValue;
                    }
                }
            }
        }
        IEnumerator ApplyHitStop(float timeHitStop)
        {
            yield return new WaitForSecondsRealtime(timeHitStop);
            Time.timeScale = 1;
        }
        #endregion
    }
    [Serializable] public class Statistics
    {
        public TypeStatistics typeStatistics;
        public float baseValue = 0;
        public float buffValue = 0;
        public float objectValue = 0;
        public float currentValue = 0;
        public float maxValue = 0;
        public Statistics(TypeStatistics typeStatistics, float baseValue, float buffValue, float objectValue, float currentValue, float maxValue)
        {
            this.typeStatistics = typeStatistics;
            this.baseValue = baseValue;
            this.buffValue = buffValue;
            this.objectValue = objectValue;
            this.currentValue = currentValue;
            this.maxValue = maxValue;
        }
    }
    public enum TypeStatistics
    {
        None = 0,
        Hp = 1,
        Mp = 2,
        Sp = 3,
        Atk = 4,
        AtkSpd = 5,
        Int = 6,
        Def = 7,
        Res = 8,
        Spd = 9,
        Crit = 10,
        CritDmg = 11,
    }
    public enum TypeDamage
    {
        None = 0,
        Physic = 1,
        Magic = 2,
        TrueDamage = 3,
    }
    public interface ICharacterAttack
    {
        public void ValidateAttack();
        public float GetDistLostTarget();
        public GameObject GetDirectionAttack();
    }
    public interface ICharacterCamera
    {
        public void MoveCamera();
    }
    public interface ICharacterMove
    {
        public void Move();
        public Rigidbody GetRigidbody();
        public void SetPositionTarget(Transform position);
        public void SetCanMoveState(bool state);
        public void SetTarget(Transform targetPos);
    }
    public interface ICharacterAnimations
    {
        public void SetInitialData(InitialDataSO animationsInfo);
        public bool ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation typeAnimation);
        public void MakeAnimation(ManagementCharacterAnimations.TypeAnimation typeAnimation);
        public GameObject GetCharacterSprite();
        public CharacterAnimationsSO.AnimationsInfo GetAnimation(ManagementCharacterAnimations.TypeAnimation typeAnimation);
        public CharacterAnimationsSO.AnimationsInfo GetCurrentAnimation();
        public CharacterAnimationsSO.CharacterAnimationsInfo GetAnimationsInfo();
    }
}