using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SkillsData", menuName = "ScriptableObjects/SkillsDataScriptableObject", order = 1)]
public class SkillDataScriptableObject : ScriptableObject
{
    public GameObject skillObject;
    public Sprite skillSprite;
    public Character.Statistics cost;
    public bool isPorcent = false;
    public bool isFromBaseValue = false;
    public bool needAnimation = false;
    public CharacterAnimationsInfoScriptableObject.AnimationsInfo skillAnimation = new CharacterAnimationsInfoScriptableObject.AnimationsInfo();
    public CdInfo cdInfo = new CdInfo();
    [System.Serializable]
    public class CdInfo
    {
        public float cd = 0;
        public float currentCD = 0;
    }
    public int textId = 0;
}
