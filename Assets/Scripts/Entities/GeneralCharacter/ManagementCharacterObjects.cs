using System.Collections.Generic;
using UnityEngine;
public class ManagementCharacterObjects : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] ObjectsInfo[] objects = new ObjectsInfo[6];
    public List<GameObject> objectsForTake = new List<GameObject>();
    [SerializeField] ObjectsPositionsInfo[] objectsPositionsInfo;
    int objectSelectedPosition = 0;
    public int currentObjectForTakePosition = 0;
    float delayChangeObjectForTake = 0;
    public GameObject currentObject;
    bool isActive = false;
    void Start()
    {
        isActive = true;
    }
    public void HandleObjects()
    {
        if (isActive)
        {
            if (character.characterInfo.isActive)
            {
                if (character.characterInfo.isPlayer)
                {
                    if (character.characterInputs.characterActionsInfo.changeObjectUp.triggered)
                    {
                        ChangeCurrentObject(true);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObjectDown.triggered)
                    {
                        ChangeCurrentObject(false);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject1.triggered)
                    {
                        ChangeCurrentObject(0);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject2.triggered)
                    {
                        ChangeCurrentObject(1);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject3.triggered)
                    {
                        ChangeCurrentObject(2);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject4.triggered)
                    {
                        ChangeCurrentObject(3);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject5.triggered)
                    {
                        ChangeCurrentObject(4);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeObject6.triggered)
                    {
                        ChangeCurrentObject(5);
                    }
                    if (character.characterInputs.characterActionsInfo.isSecondaryAction)
                    {
                        if (character.characterInputs.characterActionsInfo.useObject.triggered)
                        {
                            DropObject();
                        }
                    }
                    else if (character.characterInputs.characterActionsInfo.useObject.triggered)
                    {
                        UseObject();
                    }
                    if (objectsForTake.Count > 0)
                    {
                        if (delayChangeObjectForTake > 0)
                        {
                            delayChangeObjectForTake -= Time.deltaTime;
                        }
                        if (character.characterInputs.characterActionsInfo.changeObjectForTake.y > 0 && delayChangeObjectForTake <= 0)
                        {
                            ChangeObjectForTake(true);
                        }
                        else if (character.characterInputs.characterActionsInfo.changeObjectForTake.y < 0 && delayChangeObjectForTake <= 0)
                        {
                            ChangeObjectForTake(false);
                        }
                        if (character.characterInputs.characterActionsInfo.interact.triggered)
                        {
                            TakeObject();
                        }
                    }
                }
            }
        }
    }
    void ChangeObjectForTake(bool direction)
    {
        delayChangeObjectForTake = 0.15f;
        if (!direction)
        {
            currentObjectForTakePosition++;
            if (currentObjectForTakePosition > objectsForTake.Count - 1) currentObjectForTakePosition = 0;
        }
        else
        {
            currentObjectForTakePosition--;
            if (currentObjectForTakePosition < 0) currentObjectForTakePosition = objectsForTake.Count - 1;
        }
        currentObject = objectsForTake[currentObjectForTakePosition];
        character.characterInfo.managementCharacterHud.RefreshCurrentObjectForTake(objectsForTake, currentObject, currentObjectForTakePosition);
    }
    public void InitializeObjects()
    {
        if (character.characterInfo.isPlayer && ManagementData.saveData.gameInfo.characterInfo.characterSelected != null) objects = (ObjectsInfo[])ManagementData.saveData.gameInfo.characterInfo.currentObjects.Clone();
        foreach (ObjectsInfo objectsInfo in objects)
        {
            if (objectsInfo.objectData != null)
            {
                if (objectsInfo.amount == 0)
                {
                    objectsInfo.amount = 1;
                }
            }
        }
        currentObjectForTakePosition = 0;
        ChangeCurrentObject(objectSelectedPosition);
        if (character.characterInfo.isPlayer) character.characterInfo.managementCharacterHud.RefreshObjects(objects);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].id = i;
            if (objects[i].objectData != null && objects[i].isUsingItem)
            {
                objects[i].objectData.objectInstanceForDrop.GetComponent<ManagementObject>().InitializeObject(character, objects[i], this);
            }
        }
    }
    public void TakeObject()
    {
        if (currentObject == null)
        {
            currentObject = objectsForTake[currentObjectForTakePosition];
        }
        bool pickUpItem = false;
        ObjectsInfo[] objectsFinded = FindObjects(currentObject);
        if (objectsFinded.Length == 0)
        {
            objectsFinded = objects;
        }
        ManagementObject objectTaked = currentObject.GetComponent<ManagementObject>();
        foreach (ObjectsInfo objectForValidate in objectsFinded)
        {
            if (objectForValidate.objectData != null && CanStackObject(objectForValidate, currentObject) && objectTaked.objectInfo.amount > 0)
            {
                int amountToAdd = ValidateAmountObjectToAdd(objectForValidate, objectTaked);
                objectForValidate.amount += amountToAdd;
                character.characterInfo.managementCharacterHud.SendInformationMessage($"{ManagementData.GetDialog(18, ManagementData.saveData.configurationsInfo.currentLanguage.ToString())} {amountToAdd} {ManagementData.GetDialog(objectTaked.objectInfo.objectData.IDText, ManagementData.saveData.configurationsInfo.currentLanguage.ToString())}", Color.green);
                pickUpItem = true;
            }
        }
        if (objectTaked.objectInfo.amount > 0)
        {
            foreach (ObjectsInfo objectForAdd in objects)
            {
                if (objectForAdd.objectData == null && objectTaked.objectInfo.amount > 0)
                {
                    objectForAdd.objectData = objectTaked.objectInfo.objectData;
                    int amountToAdd = ValidateAmountObjectToAdd(objectForAdd, objectTaked);
                    objectForAdd.amount = amountToAdd;
                    character.characterInfo.managementCharacterHud.SendInformationMessage($"{ManagementData.GetDialog(18, ManagementData.saveData.configurationsInfo.currentLanguage.ToString())} {amountToAdd} {ManagementData.GetDialog(objectTaked.objectInfo.objectData.IDText, ManagementData.saveData.configurationsInfo.currentLanguage.ToString())}", Color.green);
                    pickUpItem = true;
                }
            }
        }
        if (objectTaked.objectInfo.amount > 0)
        {
            bool isFullInventory = true;
            foreach (ObjectsInfo validateFullInventory in objects)
            {
                if (validateFullInventory.objectData != null && validateFullInventory.objectData == objectTaked.objectInfo.objectData && validateFullInventory.amount < validateFullInventory.objectData.maxStack)
                {
                    isFullInventory = false;
                    break;
                }
            }
            if (isFullInventory)
            {
                character.characterInfo.managementCharacterHud.SendInformationMessage($"{ManagementData.GetDialog(19, ManagementData.saveData.configurationsInfo.currentLanguage.ToString())}", Color.red);
            }
        }
        else
        {
            RemoveObjectTaked(objectTaked.gameObject);
            objectTaked.GetComponent<ManagementObject>().objectInfo.canPickUp = false;
            Destroy(objectTaked.gameObject);
        }
        if (pickUpItem)
        {
            character.characterInfo.PlayASound(CharacterSoundsSO.TypeSound.PickUp, false);
        }
        else
        {
            character.characterInfo.PlayASound(CharacterSoundsSO.TypeSound.NotPickup, false);
        }
        RefreshObjects();
        if (objectsForTake.Count > 0)
        {
            if (currentObjectForTakePosition > objectsForTake.Count - 1)
            {
                currentObjectForTakePosition = objectsForTake.Count - 1;
            }
            currentObject = objectsForTake[currentObjectForTakePosition];
            character.characterInfo.managementCharacterHud.RefreshCurrentObjectForTake(objectsForTake, currentObject, currentObjectForTakePosition);
        }
    }
    public void UseObject()
    {
        if (objects[objectSelectedPosition].objectData != null)
        {
            objects[objectSelectedPosition].objectData.objectInstanceForDrop.GetComponent<ManagementObject>().UseObject(character, objects[objectSelectedPosition], this);
        }
    }
    public void DropObject()
    {
        if (objects[objectSelectedPosition].objectData != null)
        {
            objects[objectSelectedPosition].objectData.objectInstanceForDrop.GetComponent<ManagementObject>().DropObject(character, objects[objectSelectedPosition], this);
        }
    }
    void ChangeCurrentObject(bool direction)
    {
        objectSelectedPosition += direction ? 1 : -1;
        if (objectSelectedPosition > objects.Length - 1)
        {
            objectSelectedPosition = 0;
        }
        else if (objectSelectedPosition < 0)
        {
            objectSelectedPosition = objects.Length - 1;
        }

        character.characterInfo.managementCharacterHud.ChangeObject(objectSelectedPosition);
    }
    void ChangeCurrentObject(int position)
    {
        objectSelectedPosition = position;
        character.characterInfo.managementCharacterHud.ChangeObject(objectSelectedPosition);
    }
    int ValidateAmountObjectToAdd(ObjectsInfo objectForIncreaseAmount, ManagementObject objectForDiscountAmount)
    {
        for (int i = 1; i <= objectForDiscountAmount.objectInfo.amount; i++)
        {
            if (objectForIncreaseAmount.amount + i == objectForIncreaseAmount.objectData.maxStack || objectForDiscountAmount.objectInfo.amount - i == 0)
            {
                objectForDiscountAmount.objectInfo.amount -= i;
                return i;
            }
        }
        return 0;
    }
    ObjectsInfo[] FindObjects(GameObject objectToFind)
    {
        List<ObjectsInfo> objectsFinded = new List<ObjectsInfo>();
        foreach (ObjectsInfo objectInfo in objects)
        {
            if (objectInfo.objectData == objectToFind.GetComponent<ManagementObject>().objectInfo.objectData)
            {
                objectsFinded.Add(objectInfo);
            }
        }
        return objectsFinded.ToArray();
    }
    bool CanStackObject(ObjectsInfo objectForValidate, GameObject objectForTake)
    {
        if (objectForValidate.objectData == objectForTake.GetComponent<ManagementObject>().objectInfo.objectData && objectForValidate.amount < objectForValidate.objectData.maxStack)
        {
            return true;
        }
        return false;
    }
    public void RefreshObjects()
    {
        foreach (ObjectsInfo objectsInfo in objects)
        {
            if (objectsInfo.objectData != null)
            {
                if (objectsInfo.amount <= 0)
                {
                    objectsInfo.objectData = null;
                }
            }
        }
        currentObject = null;
        character.characterInfo.managementCharacterHud.RefreshObjects(objects);
    }
    void RemoveObjectTaked(GameObject objectForDelete)
    {
        objectsForTake.Remove(objectForDelete);
        character.characterInfo.managementCharacterHud.RefreshObjectsForTake(objectsForTake);
        if (objectsForTake == null || objectsForTake.Count == 0)
        {
            character.characterInfo.managementCharacterHud.characterUi.objectsUi.bannerTakeObjects.SetActive(false);
        }
    }
    public ObjectsPositionsInfo GetObjectsPositionsInfo(TypeObjectPosition typeObjectPosition)
    {
        foreach (var objectPositionInfo in objectsPositionsInfo)
        {
            if (objectPositionInfo.typeObjectPosition == typeObjectPosition)
            {
                return objectPositionInfo;
            }
        }
        return null;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            if (other.GetComponent<ManagementObject>().objectInfo.canPickUp)
            {
                character.characterInfo.managementCharacterHud.characterUi.objectsUi.bannerTakeObjects.SetActive(true);
                if (!objectsForTake.Contains(other.gameObject))
                {
                    objectsForTake.Add(other.gameObject);
                    character.characterInfo.managementCharacterHud.RefreshObjectsForTake(objectsForTake);
                }
                currentObjectForTakePosition = 0;
                currentObject = objectsForTake[currentObjectForTakePosition];
                character.characterInfo.managementCharacterHud.RefreshCurrentObjectForTake(objectsForTake, currentObject, currentObjectForTakePosition);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Object"))
        {
            RemoveObjectTaked(other.gameObject);
            if (objectsForTake.Count > 0)
            {
                currentObjectForTakePosition = 0;
                currentObject = objectsForTake[currentObjectForTakePosition];
                character.characterInfo.managementCharacterHud.RefreshCurrentObjectForTake(objectsForTake, currentObject, currentObjectForTakePosition);
            }
        }
    }
    [System.Serializable] public class ObjectsInfo
    {
        public ObjectsDataScriptableObject objectData;
        public int amount;
        public bool isUsingItem = false;
        public int id;
        public GameObject objectInstance;
    }
    [System.Serializable] public class ObjectsPositionsInfo{
        public TypeObjectPosition typeObjectPosition = TypeObjectPosition.None;
        public GameObject objectPosition;
    }
    public enum TypeObjectPosition
    {
        None = 0,
        Weapon = 1,
        Ligth = 2
    }
}