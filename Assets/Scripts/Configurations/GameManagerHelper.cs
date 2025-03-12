using System;
using UnityEngine;

public class GameManagerHelper : MonoBehaviour
{
    [NonSerialized] public GameManager gameManager;
    public AudioSource bgMusic;
    void Start()
    {
        gameManager = GameObject.FindWithTag("InformationBetweenScenes").GetComponent<GameManager>();    
    }
    public void ChangeScene(int typeScene)
    {
        GameManager.TypeScene scene = (GameManager.TypeScene)typeScene;
        gameManager.ChangeSceneSelector(scene);
    }
    public void PlayASound(AudioClip audioClip){
        gameManager.PlayASound(audioClip);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        gameManager.PlayASound(audioClip, initialRandomPitch);
    }
    public void PlayASoundButton(AudioClip audioClip)
    {
        if (!gameManager) return;
        gameManager.PlayASound(audioClip, 1);
    }
    public void SetAudioMixerData()
    {
        gameManager.SetAudioMixerData();
    }
    public void ChangeBGMusic(AudioClip audioClip)
    {
        bgMusic.Stop();
        bgMusic.clip = audioClip;
        bgMusic.Play();
    }
}
