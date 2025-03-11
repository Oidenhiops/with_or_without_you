using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ManagementKey : MonoBehaviour, ManagementObject.IObject
{
    public TypeKey typeKey;
    public LayerMask layerMask;
    public AudioClip unlockClip;
    public AudioClip noUnlockClip;

    public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        Vector3 positionsSpawn = character.transform.position + new Vector3(character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.x, 0.5f, character.characterInfo.managementCharacterModelDirection.movementDirectionAnimation.y);
        GameObject objectInstance = Instantiate(objectInfo.objectData.objectInstanceForDrop, positionsSpawn, Quaternion.identity, character.gameObject.transform);
        Vector3 directionForce = (character.transform.position - objectInstance.transform.position).normalized;
        objectInstance.GetComponent<Rigidbody>().AddForce(-directionForce * 100);
        objectInstance.GetComponent<ManagementObject>().objectInfo.canPickUp = true;
        objectInstance.GetComponent<ManagementObject>().objectInfo.amount = 1;
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
        if (ValidateUse(character ,out GameObject chest))
        {
            if (!chest.GetComponent<ManagementChest.IChest>().ValidateIsUnlock())
            {
                if (chest.GetComponent<ManagementChest.IChest>().ValidateUnlock(typeKey))
                {
                    character.characterInfo.PlayASound(unlockClip, true);
                    objectInfo.amount--;
                    character.characterInfo.managementCharacterObjects.RefreshObjects();
                }
                else
                {
                    character.characterInfo.PlayASound(noUnlockClip, true);
                }
            }
        }
    }
    public bool ValidateUse(Character character ,out GameObject chest)
    {        
        if (Physics.Raycast(character.transform.position + Vector3.up / 2, 
            character.characterInfo.managementCharacterModelDirection.directionPlayer.transform.forward, out RaycastHit objectHit,
            0.25f, layerMask))
        {
            chest = objectHit.collider.gameObject;
            return true;
        }
        chest = null;
        return false;
    }
    public enum TypeKey
    {
        None = 0,
        General = 1,
        Special = 2
    }
}
