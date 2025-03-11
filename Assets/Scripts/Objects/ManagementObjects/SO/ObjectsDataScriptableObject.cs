using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsData", menuName = "ScriptableObjects/ObjectsDataScriptableObject", order = 1)]

public class ObjectsDataScriptableObject : ScriptableObject
{
    public int ID = 0;
    public TypeObject typeObject;
    public int IDText = 0;
    public Character.Statistics[] statistics;
    public bool isPorcent = false;
    public bool canStack = false;
    public float maxStack = 0;
    public Sprite objectSprite;
    public GameObject objectInstanceForDrop;
    public GameObject objectInstanceForUse;
    public Color colorEffect = Color.white;
    public AudioClip effectAudio;    
    public bool isOnlyForMonsters;
    public enum TypeObject
    {
        None = 0,
        Object = 1,
        Weapon = 2,
        Armor = 3,
        ObjectConsumable = 4
    }
}