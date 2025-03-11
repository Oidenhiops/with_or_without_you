using System.Collections;
using UnityEngine;

public class ManagementCharacterAttack : MonoBehaviour, Character.ICharacterAttack
{
    [SerializeField] Character character;
    [SerializeField] GameObject posisionAttack;
    ManagementCharacterModelDirection.ICharacterDirection characterDirection;
    Character.ICharacterAnimations characterAnimations;
    float distAttack = 1;
    float distLostTarget = 1;
    float cooldownAttack = 0;
    Character.Statistics costsAttack = new Character.Statistics(Character.TypeStatistics.Sp, 3, 0, 0, 0, 0);
    void Start()
    {
        characterDirection = GetComponent<ManagementCharacterModelDirection.ICharacterDirection>();
        characterAnimations = character.characterInfo.characterAnimations;
    }
    void Update()
    {
        if (cooldownAttack > 0)
        {
            cooldownAttack -= Time.deltaTime;
        }
    }
    public void ValidateAttack()
    {
        if (!character.characterInfo.isPlayer)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, distAttack, LayerMask.GetMask("Player"));
            if (hitColliders.Length > 0 && hitColliders[0].GetComponent<Character>().characterInfo.isActive)
            {
                if (characterAnimations != null && characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.TakeDamage) &&
                    characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.Attack) && cooldownAttack <= 0)
                {
                    characterDirection.SetTarget(hitColliders[0].gameObject);
                    characterAnimations.MakeAnimation(ManagementCharacterAnimations.TypeAnimation.Attack);
                    SetCoolDown();
                    Attack();
                }
            }
        }
        else
        {
            if (characterAnimations != null && characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.TakeDamage) &&
                characterAnimations.ValidateAnimationEnd(ManagementCharacterAnimations.TypeAnimation.Attack) && cooldownAttack <= 0 &&
                character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue - costsAttack.baseValue >= 0)
            {
                character.characterInfo.GetStatisticByType(costsAttack.typeStatistics).currentValue -= costsAttack.baseValue;
                characterAnimations.MakeAnimation(ManagementCharacterAnimations.TypeAnimation.Attack);
                SetCoolDown();
                Attack();
            }
        }
    }
    void SetCoolDown()
    {
        cooldownAttack = 1 / character.characterInfo.GetStatisticByType(Character.TypeStatistics.AtkSpd).currentValue;
    }
    void Attack()
    {
        StartCoroutine(WaitToAttack());
    }
    IEnumerator WaitToAttack()
    {
        while (characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.Attack)
        {
            if (characterAnimations.GetCurrentAnimation().typeAnimation == ManagementCharacterAnimations.TypeAnimation.TakeDamage)
            {
                StopAllCoroutines();
            }
            yield return null;
        }
        while (characterAnimations.GetCurrentAnimation().frameToInstance >= characterAnimations.GetAnimationsInfo().currentSpriteIndex)
        {
            yield return null;
        }
        if (characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.TakeDamage)
        {
            GameObject instance = Instantiate(characterAnimations.GetCurrentAnimation().instanceObj, transform.position, Quaternion.identity);
            instance.layer = character.characterInfo.isPlayer ? LayerMask.NameToLayer("PlayerAttack") : LayerMask.NameToLayer("EnemyAttack");
            instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetDamage(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Atk).currentValue);
            instance.GetComponent<ManagementInstanceAttack.IInstanceAttack>().SetObjectMakeDamage(character.gameObject);
            instance.GetComponent<ManagementCharacterInstance>().SetInfoForAnimation(character.characterInfo.managementCharacterModelDirection.GetDirectionAnimation(), character.characterInfo.characterAnimations.GetAnimationsInfo());
            instance.transform.position = character.characterInfo.characterAttack.GetDirectionAttack().transform.position;
            instance.transform.rotation = character.characterInfo.characterAttack.GetDirectionAttack().transform.rotation;
            instance.transform.localScale = Vector3.one;
            character.characterInfo.PlayASound(CharacterSoundsScriptableObjec.TypeSound.Slash, false);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distAttack);
    }
    public float GetDistLostTarget()
    {
        return distLostTarget;
    }
    public GameObject GetDirectionAttack()
    {
        return posisionAttack;
    }
}
