using UnityEngine;

public class ManagementTorch : MonoBehaviour, ManagementObject.IObject
{
public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        if (objectInfo.isUsingItem)
        {
            if (character.characterInfo.isPlayer) character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, false);
            objectInfo.isUsingItem = false;
            Destroy(objectInfo.objectInstance);
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
        InstanceTorch(objectInfo, managementCharacterObjects);
        if (character.characterInfo.isPlayer) character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, true);
    }

    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.isUsingItem = !objectInfo.isUsingItem;
        if (objectInfo.objectInstance == null)
        {
            InstanceTorch(objectInfo, managementCharacterObjects);
            objectInfo.objectInstance.SetActive(false);
        }
        if (objectInfo.isUsingItem)
        {
            objectInfo.objectInstance.SetActive(true);
            character.characterInfo.PlayASound(objectInfo.objectData.effectAudio, 1.1f, false);
        }
        else
        {
            objectInfo.objectInstance.SetActive(false);
            character.characterInfo.PlayASound(objectInfo.objectData.effectAudio, 0.9f, false);
        }
        if (character.characterInfo.isPlayer) character.characterInfo.managementCharacterHud.ToggleActiveObject(objectInfo.id, objectInfo.isUsingItem);
        character.characterInfo.RefreshCurrentStatistics();
    }
    public void InstanceTorch(ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        objectInfo.objectInstance = Instantiate(objectInfo.objectData.objectInstanceForUse, managementCharacterObjects.GetObjectsPositionsInfo(ManagementCharacterObjects.TypeObjectPosition.Ligth).objectPosition.transform);
        objectInfo.objectInstance.transform.localPosition = Vector3.zero;
        objectInfo.objectInstance.transform.localRotation = Quaternion.Euler(Vector3.zero);
        objectInfo.objectInstance.transform.localScale = Vector3.one;
    }
}
