using System;
using UnityEngine;

public class ManagementCharacterSounds : MonoBehaviour
{
    [NonSerialized] public CharacterSoundsScriptableObjec soundsInfo;
    public AudioClip GetAudioClip(CharacterSoundsScriptableObjec.TypeSound typeSound)
    {
        CharacterSoundsScriptableObjec.SoundsInfo sounds = new CharacterSoundsScriptableObjec.SoundsInfo();
        foreach (CharacterSoundsScriptableObjec.SoundsInfo sound in soundsInfo.sounds)
        {
            if (sound.typeSound == typeSound)
            {
                sounds = sound;
                break;
            }
        }
        return sounds.clips[UnityEngine.Random.Range(0, sounds.clips.Length - 1)];
    }
}
