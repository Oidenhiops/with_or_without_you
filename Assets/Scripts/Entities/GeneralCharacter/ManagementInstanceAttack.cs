using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagementInstanceAttack : MonoBehaviour, ManagementInstanceAttack.IInstanceAttack
{
    public InstanceAttackInfo instanceAttackInfo = new InstanceAttackInfo();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetDamage(float value)
    {
        instanceAttackInfo.damage = Mathf.CeilToInt(value);
    }

    public void SetObjectMakeDamage(GameObject objectMakeDamage)
    {
        instanceAttackInfo.objectMakeDamage = objectMakeDamage;
    }

    [System.Serializable]
    public class InstanceAttackInfo 
    {
        public Character.TypeDamage typeDamage;
        public float damage = 0;
        public bool isPorcent = false;
        public Color colorDamage = Color.white;
        public GameObject objectMakeDamage;
        public bool canDestroy = false;
        public bool needPush = false;
        public float pushForce = 5;
        public float timeHitStop;
        public bool isMultipleAttack;
        public float timeToRestoreCharacterToHit = 0.1f;
        public List<Character> charactersHited = new List<Character>();
    }
    public interface IInstanceAttack
    {
        public void SetDamage(float value);
        public void SetObjectMakeDamage(GameObject objectMakeDamage);
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Character character = other.GetComponent<Character>();
            if (!instanceAttackInfo.charactersHited.Contains(character))
            {
                instanceAttackInfo.charactersHited.Add(character);
                int damage = !instanceAttackInfo.isPorcent ? (int)instanceAttackInfo.damage : (int)MathF.Round(character.characterInfo.GetStatisticByType(Character.TypeStatistics.Hp).maxValue * instanceAttackInfo.damage / 100);
                character.characterInfo.TakeDamage(damage, instanceAttackInfo.colorDamage, instanceAttackInfo.timeHitStop, instanceAttackInfo.typeDamage);
                if (instanceAttackInfo.needPush && character.characterInfo.isActive)
                {
                    character.characterInfo.isPushed = true;
                    Vector3 direction = (other.transform.position - instanceAttackInfo.objectMakeDamage.transform.position).normalized;
                    Rigidbody rb = other.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.AddForce(direction * instanceAttackInfo.pushForce * rb.mass, ForceMode.Impulse);
                    }
                }
                if (instanceAttackInfo.isMultipleAttack)
                {
                    StartCoroutine(RestoreCharacterToHit(character));
                }
            }
        }
        else if (instanceAttackInfo.canDestroy)
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator RestoreCharacterToHit(Character characterToRestore)
    {
        yield return new WaitForSeconds(instanceAttackInfo.timeToRestoreCharacterToHit);
        if (instanceAttackInfo.charactersHited.Contains(characterToRestore))
        {
            instanceAttackInfo.charactersHited.Remove(characterToRestore);
        }
    }
}
