using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ManagementPotions : MonoBehaviour, ManagementObject.IObject
{
    public GameObject effect;
    public void DropObject(Character character ,ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Vector3 positionsSpawn = character.transform.position + new Vector3(character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.x, 0.5f, character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.y);
        GameObject potion = Instantiate(objectInfo.objectData.objectInstanceForDrop, positionsSpawn, Quaternion.identity);
        Vector3 directionForce = (character.transform.position - potion.transform.position).normalized;
        potion.GetComponent<Rigidbody>().isKinematic = false;
        potion.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        potion.GetComponent<ManagementObject>().objectInfo.canPickUp = true;
        potion.GetComponent<ManagementObject>().objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.managementCharacterObjects.RefreshObjects();
        character.characterInfo.PlayASound(character.characterInfo.managementCharacterSounds.GetAudioClip(CharacterSoundsScriptableObjec.TypeSound.PickUp), true);
    }

    public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        throw new System.NotImplementedException();
    }

    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        foreach(Character.Statistics potion in objectInfo.objectData.statistics)
        {
            Character.Statistics statistic = character.characterInfo.GetStatisticByType(potion.typeStatistics);
            float value = objectInfo.objectData.isPorcent ? Mathf.Ceil((statistic.maxValue * potion.baseValue) / 100) : potion.baseValue;
            statistic.currentValue += value;
            GameObject floatingText = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingText/FloatingText"), character.gameObject.transform.position, Quaternion.identity);
            FloatingText floatingTextScript = floatingText.GetComponent<FloatingText>();
            floatingTextScript.SendText(floatingTextScript.takeDamage, Mathf.Ceil(value).ToString(), objectInfo.objectData.colorEffect);
            if (statistic.currentValue > statistic.maxValue)
            {
                statistic.currentValue = statistic.maxValue;
            }
        }
        character.characterInfo.PlayASound(objectInfo.objectData.effectAudio, true);
        GameObject potionEffect = Instantiate(effect, character.transform.position + new Vector3(0, 0.05f, 0), Quaternion.identity, character.transform);
        Destroy(potionEffect, 3);
        objectInfo.amount--;
        character.characterInfo.managementCharacterObjects.RefreshObjects();
    }
    public enum TypePotion
    {
        None = 0,
        Heal = 1,
        Mana = 2,
        Stamina = 3
    }
}
