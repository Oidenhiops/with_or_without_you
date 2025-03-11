using UnityEngine;

public class ManagementArmors : MonoBehaviour, ManagementObject.IObject
{
    public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            objectInfo.isUsingItem = false;
            character.characterInfo.RefreshCurrentStatistics();
            if (character.characterInfo.isPlayer)
            {
                character.characterInfo.RefreshCurrentStatistics();
                character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, false);
            }
        }

        Vector3 positionsSpawn = character.transform.position + new Vector3(character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.x, 0.5f, character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.y);
        GameObject armor = Instantiate(objectInfo.objectData.objectInstanceForDrop, positionsSpawn, Quaternion.identity);
        Vector3 directionForce = (character.transform.position - armor.transform.position).normalized;
        armor.GetComponent<Rigidbody>().isKinematic = false;
        armor.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        armor.GetComponent<ManagementObject>().objectInfo.canPickUp = true;
        armor.GetComponent<ManagementObject>().objectInfo.amount = 1;
        objectInfo.amount--;
        character.characterInfo.managementCharacterObjects.RefreshObjects();
        character.characterInfo.PlayASound(character.characterInfo.managementCharacterSounds.GetAudioClip(CharacterSoundsScriptableObjec.TypeSound.PickUp), true);
    }

    public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
        {
            Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
            statistic.objectValue += armorStats.baseValue;
        }
            character.characterInfo.RefreshCurrentStatistics();
            if (character.characterInfo.isPlayer)
            {
                character.characterInfo.RefreshCurrentStatistics();
                character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, true);
            }
    }

    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.isUsingItem = !objectInfo.isUsingItem;
        if (objectInfo.isUsingItem)
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue += armorStats.baseValue;
            }
            character.characterInfo.PlayASound(objectInfo.objectData.effectAudio, 1.1f, false);
        }
        else
        {
            foreach (Character.Statistics armorStats in objectInfo.objectData.statistics)
            {
                Character.Statistics statistic = character.characterInfo.GetStatisticByType(armorStats.typeStatistics);
                statistic.objectValue -= armorStats.baseValue;
            }
            character.characterInfo.PlayASound(objectInfo.objectData.effectAudio, 0.9f, false);
        }
            character.characterInfo.RefreshCurrentStatistics();
            if (character.characterInfo.isPlayer)
            {
                character.characterInfo.RefreshCurrentStatistics();
                character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, objectInfo.isUsingItem);
            }
    }
}
