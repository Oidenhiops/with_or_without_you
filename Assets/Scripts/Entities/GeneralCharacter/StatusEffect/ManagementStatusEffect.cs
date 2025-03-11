using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class ManagementStatusEffect : MonoBehaviour
{
    Character character;
    public StatusEffectsInfo statusEffectsInfo;
    public StatusEffectSO statusEffectSO;
    void Start()
    {
        character = GetComponent<Character>();
    }
    void Update()
    {
        if (character.characterInfo.isActive)
        {
            if (statusEffectsInfo.statusEffects.Count > 0)
            {
                for (int i = 0; i < statusEffectsInfo.statusEffects.Count; i++)
                {
                    StatusEffectsData status = statusEffectsInfo.statusEffects.ElementAt(i).Value;
                    status.currentTime -= Time.deltaTime;
                    if (status.currentTime <= 0)
                    {
                        character.characterInfo.managementCharacterHud.DestroyStatusEffect(status.statusEffectSO.typeStatusEffect);
                        statusEffectsInfo.statusEffects.Remove(status.statusEffectSO.typeStatusEffect);
                    }
                    else
                    {
                        status.currentAccumulations = (int)Math.Ceiling(status.currentTime / status.statusEffectSO.timePerAcumulation);
                    }
                }
            }
        }
    }
    [NaughtyAttributes.Button]
    public void TestAddStatus(){
        AddStatus(statusEffectSO);
    }
    public void AddStatus(StatusEffectSO statusEffectSO){
        if (statusEffectsInfo.statusEffects.TryGetValue(statusEffectSO.typeStatusEffect, out StatusEffectsData statusEffectsData)){
            if (statusEffectsData.currentTime + statusEffectSO.timePerAcumulation > statusEffectSO.timePerAcumulation * statusEffectSO.maxAccumulations)
            {
                statusEffectsData.currentAccumulations = statusEffectSO.maxAccumulations;
                statusEffectsData.currentTime = statusEffectSO.timePerAcumulation * statusEffectSO.maxAccumulations;                
            }
            else
            {
                statusEffectsData.currentAccumulations++;
                statusEffectsData.currentTime += statusEffectSO.timePerAcumulation;
            }
        }
        else
        {
            statusEffectsInfo.statusEffects.Add(statusEffectSO.typeStatusEffect, new StatusEffectsData (statusEffectSO, statusEffectSO.timePerAcumulation, 1));
            character.characterInfo.managementCharacterHud.UpdateStatusEffect(statusEffectsInfo.statusEffects[statusEffectSO.typeStatusEffect]);
        }
    }
    [Serializable] public class StatusEffectsInfo
    {
        public SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData> statusEffects = new SerializedDictionary<StatusEffectSO.TypeStatusEffect, StatusEffectsData>();
    }
    [Serializable] public class StatusEffectsData
    {
        public StatusEffectSO statusEffectSO;
        public float currentTime = 0;
        public int currentAccumulations = 0;

        public StatusEffectsData( StatusEffectSO statusEffectSO, float currentTime, int currentAccumulations)
        {
            this.statusEffectSO = statusEffectSO;
            this.currentTime = currentTime;
            this.currentAccumulations = currentAccumulations;
        }
    }
    public interface IStatusEffect
    {
        public void ApplyStatusEffect(Character character, StatusEffectsData statusEffectsData);
        public void RemoveStatusEffect(StatusEffectsData statusEffectsData);
    }
}
