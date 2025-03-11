using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementCharacterSkills : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] ManagementCharacterHud managementCharacterHud;
    [SerializeField] ManagementCharacterAnimations managementCharacterAnimations;
    [SerializeField] SkillInfo[] currentSkills = new SkillInfo[4];
    int currentSkillIndex = 0;
    int amountSkills = 0;
    bool usingSkill;
    public void HandleSkills()
    {
        if (character.characterInfo.isActive)
        {
            if (currentSkills[0].skillData != null)
            {
                foreach (SkillInfo skill in currentSkills)
                {
                    if (skill.skillData != null && skill.cdInfo.currentCD > 0)
                    {
                        skill.cdInfo.currentCD -= Time.deltaTime;
                    }
                }
                managementCharacterHud.RefreshSkillsTimer(currentSkills);
            }
            if (usingSkill)
            {
                UseSkill();
            }
            else
            {
                if (character.characterInfo.isPlayer)
                {
                    if (character.characterInputs.characterActionsInfo.changeSkillUp.triggered)
                    {
                        ChangeSkill(true);
                    }
                    else if (character.characterInputs.characterActionsInfo.changeSkillDown.triggered)
                    {
                        ChangeSkill(false);
                    }
                    else if (character.characterInputs.characterActionsInfo.useSkill.triggered)
                    {
                        ValidateUseSkill();
                    }
                }
            }
        }
    }
    void ChangeSkill(bool isUp)
    {
        RefreshAmountSkills();
        currentSkillIndex += isUp ? 1 : -1;
        if (currentSkillIndex < 0)
        {
            if (amountSkills > 0)
            {
                currentSkillIndex = amountSkills - 1;
            }
            else
            {
                currentSkillIndex = 0;
            }
        }
        else if (currentSkillIndex > amountSkills - 1)
        {
            currentSkillIndex = 0;
        }
        managementCharacterHud.ChangeCurrentSkill(false, currentSkillIndex);
    }
    public void ChangeSkill(int pos)
    {
        RefreshAmountSkills();        
        if (pos == 0 || pos < amountSkills)
        {
            currentSkillIndex = pos;
            managementCharacterHud.ChangeCurrentSkill(false, currentSkillIndex);
        }
    }
    void RefreshAmountSkills()
    {
        amountSkills = 0;
        foreach (SkillInfo skill in currentSkills)
        {
            if (skill.skillData != null)
            {
                amountSkills++;
            }
            else
            {
                skill.cdInfo.cd = 0;
                skill.cdInfo.currentCD = 0;
            }
        }
    }
    void ValidateUseSkill()
    {
        if (currentSkills[currentSkillIndex].cdInfo.currentCD <= 0)
        {
            if (currentSkills[currentSkillIndex].skillData.cost.typeStatistics == Character.TypeStatistics.Hp)
            {
                if (currentSkills[currentSkillIndex].skillData.isPorcent)
                {
                    if (currentSkills[currentSkillIndex].skillData.isFromBaseValue)
                    {
                        int amount = (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).baseValue * currentSkills[currentSkillIndex].skillData.cost.baseValue) / 100;
                        if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount > 1)
                        {                            
                            character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                            currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                            InitializeUsingSkill();
                        }
                    }
                    else
                    {
                        int amount = (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue * currentSkills[currentSkillIndex].skillData.cost.baseValue) / 100;
                        if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount > 1)
                        {
                            character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                            currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                            InitializeUsingSkill();
                        }
                    }
                }
                else if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 1)
                {
                    int amount = (int)Mathf.Ceil(currentSkills[currentSkillIndex].skillData.cost.baseValue);
                    character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                    currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                    InitializeUsingSkill();
                }
            }
            else if (currentSkills[currentSkillIndex].skillData.isPorcent)
            {
                if (currentSkills[currentSkillIndex].skillData.isFromBaseValue)
                {
                    int amount = (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).baseValue * currentSkills[currentSkillIndex].skillData.cost.baseValue) / 100;
                    if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount >= 0)
                    {
                        character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                        currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                        InitializeUsingSkill();
                    }
                }
                else
                {
                    int amount = (int)Mathf.Ceil(character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue * currentSkills[currentSkillIndex].skillData.cost.baseValue) / 100;
                    if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - amount >= 0)
                    {
                        character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                        currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                        InitializeUsingSkill();
                    }
                }
            }
            else if (character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue - currentSkills[currentSkillIndex].skillData.cost.baseValue >= 0)
            {
                int amount = (int)Mathf.Ceil(currentSkills[currentSkillIndex].skillData.cost.baseValue);
                character.characterInfo.GetStatisticByType(currentSkills[currentSkillIndex].skillData.cost.typeStatistics).currentValue -= amount;
                currentSkills[currentSkillIndex].cdInfo.currentCD = currentSkills[currentSkillIndex].cdInfo.cd;
                InitializeUsingSkill();
            }
        }
    }
    void InitializeUsingSkill()
    {
        if (currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            managementCharacterAnimations.MakeAnimation(currentSkills[currentSkillIndex].skillData.skillAnimation.typeAnimation);
        }
        usingSkill = true;
    }
    void UseSkill()
    {
        if (!currentSkills[currentSkillIndex].skillData.needAnimation)
        {
            usingSkill = false;
        }
        else
        {
            if (managementCharacterAnimations.currentAnimation.typeAnimation == currentSkills[currentSkillIndex].skillData.skillAnimation.typeAnimation)
            {
                if (managementCharacterAnimations.currentAnimation.frameToInstance == managementCharacterAnimations.characterAnimationsInfo.currentSpriteIndex)
                {
                    usingSkill = false;
                    GameObject skill = Instantiate(currentSkills[currentSkillIndex].skillData.skillObject, transform.position, Quaternion.identity, transform);
                    skill.GetComponent<ISkill>().SendInformation(character.characterInfo.characterStatistics, character);
                }
            }
            else
            {
                usingSkill = false;
            }
        }
    }
    public void InitializeSkills()
    {
        if (ManagementData.saveData.gameInfo.characterInfo.characterSelected != null)
        {
            currentSkills = ManagementData.saveData.gameInfo.characterInfo.currentSkills;
        }
        for (int i = 0; i < currentSkills.Length; i++)
        {
            if (currentSkills[i].skillData != null)
            {
                currentSkills[i].cdInfo = new SkillDataScriptableObject.CdInfo
                {
                    cd = currentSkills[i].skillData.cdInfo.cd
                };
            }
        }
        managementCharacterHud.RefreshSkillsSprites(currentSkills);

        RefreshAmountSkills();

        if (amountSkills > 0)
        {
            managementCharacterHud.ChangeCurrentSkill(false, currentSkillIndex);
        }
        else
        {
            managementCharacterHud.ChangeCurrentSkill(true, 0);
        }
    }
    [Serializable] public class SkillInfo
    {
        public SkillDataScriptableObject skillData;
        public SkillDataScriptableObject.CdInfo cdInfo = new SkillDataScriptableObject.CdInfo();
    }
    public interface ISkill
    {
        public void SendInformation(Dictionary<Character.TypeStatistics, Character.Statistics> statistics, Character character);
    }
}
