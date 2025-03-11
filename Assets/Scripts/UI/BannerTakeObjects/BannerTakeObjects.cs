using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannerTakeObjects : MonoBehaviour
{
    public Image backgroundObject;
    public Image spriteObject;
    public TMP_Text textObject;
    public ManagementCharacterObjects managementCharacterObjects;
    public GameObject objectForTake;
    public TakeButtonInfo[] typeButtons;
    public GameObject takeButton;
    public ManagementLanguage managementLanguage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (CharacterInputs.currentDevice != CharacterInputs.TypeDevice.MOBILE)
        {
            if (takeButton.activeSelf == false)
            {
                takeButton.SetActive(true);
            }

            foreach (TakeButtonInfo button in typeButtons)
            {
                if (button.typeButton == CharacterInputs.currentDevice)
                {
                    button.button.SetActive(true);
                }
                else
                {
                    button.button.SetActive(false);
                }
            }
        }
        else
        {
            takeButton.SetActive(false);
        }
    }
    public void TakeObject()
    {
        managementCharacterObjects.currentObject = objectForTake;
        for (int i = 0; i < managementCharacterObjects.objectsForTake.Count; i++)
        {
            if (managementCharacterObjects.objectsForTake[i] == objectForTake)
            {
                managementCharacterObjects.currentObjectForTakePosition = i;
            }
        }
        managementCharacterObjects.TakeObject();
    }
    [System.Serializable]
    public class TakeButtonInfo 
    {
        public CharacterInputs.TypeDevice typeButton;
        public GameObject button;
    }
}
