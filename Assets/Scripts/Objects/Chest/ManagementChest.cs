using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementChest : MonoBehaviour, ManagementChest.IChest
{
    public Animator animator;
    public bool needKey = false;
    public bool isUnlock = false;
    public bool isOpen = false;
    public GameObject bannerKey;
    public GameObject bannerUnlock;
    public ManagementKey.TypeKey typeKeyChest;
    public ProbabilityItems[] probabilityItems;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            if (other.GetComponent<Character>().characterInfo.isPlayer)
            {
                if (!isOpen)
                {
                    if (!needKey)
                    {
                        bannerUnlock.SetActive(true);
                        bannerKey.SetActive(false);
                    }
                    else
                    {
                        if (!isUnlock)
                        {
                            bannerUnlock.SetActive(false);
                            bannerKey.SetActive(true);
                        }
                        else
                        {
                            bannerUnlock.SetActive(true);
                            bannerKey.SetActive(false);
                        }
                    }
                }
                else
                {
                    bannerUnlock.SetActive(false);
                    bannerKey.SetActive(false);
                }
                if (other.GetComponent<Character>().characterInputs.characterActionsInfo.interact.triggered)
                {
                    InteractWhitChest();
                }
            }
        }
    }
    public void InteractWhitChest()
    {
        if (!isOpen)
        {
            if (!needKey)
            {
                OpenChest();
            }
            else
            {
                if (isUnlock)
                {
                    OpenChest();
                }
            }
        }
    }
    public void OpenChest()
    {
        isOpen = true;
        animator.Play("ChestOpen", 0, 0f);
        List<GameObject> objectsSelected = SelectItems();
        int index = 0;
        while(index < objectsSelected.Count)
        {
            index++;
            int amountItems = ValidateAmountItems();
            for (int i = 0; i < amountItems; i++)
            {
                GameObject obj = Instantiate(objectsSelected[i], transform.position, Quaternion.identity);
                obj.GetComponent<ManagementObject>().meshObj.SetActive(false);
                obj.GetComponent<ManagementObject>().colliderForMap.enabled = false;
                obj.GetComponent<ManagementObject>().colliderForPlayer.radius = 1;
                obj.GetComponent<ManagementObject>().objectInfo.canPickUp = true;
                obj.GetComponent<ManagementObject>().objectInfo.amount = 1;
                obj.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
    public int ValidateAmountItems()
    {
        int amount = 0;
        float probability = Random.Range(0, 100);
        if (probability <= 10)
        {
            amount++;
        }
        if (probability <= 20)
        {
            amount++;
        }
        if (probability <= 30)
        {
            amount++;
        }
        if (probability <= 40)
        {
            amount++;
        }
        if (probability <= 100)
        {
            amount++;
        }
        return amount;
    }
    public List<GameObject> SelectItems()
    {
        List<GameObject> objects = new List<GameObject>();
        int numberItems = Random.Range(1, 5);
        int index = 0;
        while (index < numberItems)
        {
            index++;
            Random.InitState(System.DateTime.Now.Millisecond);
            float probabilityItem = Random.Range(0, 100);
            List<ProbabilityItems> paths = new List<ProbabilityItems>();
            for (int i = 0; i < probabilityItems.Length; i++)
            {
                if (probabilityItem <= probabilityItems[i].probability)
                {
                    paths.Add(probabilityItems[i]);
                }
            }
            for (int i = 0; i < paths.Count; i++)
            {                
                GameObject[] objectsSelected = Resources.LoadAll<GameObject>($"Prefabs/Objects/{paths[i].pathObjects}");
                int indexObject = Random.Range(0, objectsSelected.Length - 1);
                if (objects.Count > 0)
                {
                    if (!objects.Contains(objectsSelected[indexObject]))
                    {
                        objects.Add(objectsSelected[indexObject]);
                    }
                }
                else
                {
                    objects.Add(objectsSelected[i]);
                }
            }
        }
        return objects;
    }
    public bool ValidateUnlock(ManagementKey.TypeKey currentKey)
    {
        if (!isUnlock)
        {
            if (typeKeyChest == currentKey)
            {
                isUnlock = true;
                return true;
            }
        }
        return false;
    }
    [System.Serializable]
    public class ProbabilityItems
    {
        public PathObjects pathObjects;
        public float probability;
    }
    public enum PathObjects
    {
        None,
        Keys,
        Potions
    }
    public bool ValidateIsUnlock()
    {
        return isUnlock;
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            if (other.GetComponent<Character>().characterInfo.isPlayer)
            {
                bannerUnlock.SetActive(false);
                bannerKey.SetActive(false);
            }
        }
    }
    public interface IChest
    {
        public bool ValidateUnlock(ManagementKey.TypeKey currentKey);
        public bool ValidateIsUnlock();
    }
}
