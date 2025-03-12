using UnityEngine;

[CreateAssetMenu(fileName = "InitialData", menuName = "ScriptableObjects/Character/InitialDataSO", order = 1)]
public class InitialDataSO : ScriptableObject
{
    public Texture2D atlas;
    public Character.CharacterInfo characterInfo = new Character.CharacterInfo();
    public ManagementCharacterSkills.SkillInfo baseSkill;
    public ManagementCharacterObjects.ObjectsInfo[] objects = new ManagementCharacterObjects.ObjectsInfo[0];
    public CharacterAnimationsSO characterAnimations;
    public CharacterSoundsSO characterSounds;
    public InitialDataSO Clone()
    {
        InitialDataSO clone = ScriptableObject.CreateInstance<InitialDataSO>();

        clone.atlas = this.atlas;
        clone.characterInfo = this.characterInfo;
        clone.baseSkill = this.baseSkill;

        clone.name = clone.atlas.name;

        clone.objects = new ManagementCharacterObjects.ObjectsInfo[this.objects.Length];
        for (int i = 0; i < this.objects.Length; i++)
        {
            clone.objects[i] = this.objects[i];
        }

        clone.characterAnimations = Instantiate(this.characterAnimations);
        clone.characterSounds = Instantiate(this.characterSounds);

        return clone;
    }
}