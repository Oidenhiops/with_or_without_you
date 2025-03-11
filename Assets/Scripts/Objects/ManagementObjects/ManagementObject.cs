using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementObject : MonoBehaviour
{
    public GameObject meshObj;
    public Collider colliderForMap;
    public SphereCollider colliderForPlayer;
    public ObjectInfo objectInfo;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().UseObject(character, objectInfo, managementCharacterObjects);
    }
    public void DropObject(Character character ,ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().DropObject(character, objectInfo, managementCharacterObjects);
    }
    public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects)
    {
        GetComponent<IObject>().InitializeObject(character, objectInfo, managementCharacterObjects);
    }
    public void GiveStatistics(){

    }
    public void RemoveStatistics(){

    }
    [System.Serializable]
    public class ObjectInfo
    {
        public ObjectsDataScriptableObject objectData;
        public int amount;
        public bool canPickUp;
    }
    public interface IObject
    {
        public void UseObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
        public void DropObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
        public void InitializeObject(Character character, ManagementCharacterObjects.ObjectsInfo objectInfo, ManagementCharacterObjects managementCharacterObjects);
    }
}
