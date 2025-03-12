using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ManagementData managementData;
    public ManagementOpenCloseScene OpenCloseScene;
    public Coroutine fadeIn;
    public Coroutine fadeOut;
    public void Start()
    {
        managementData.SetAudioMixerData();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool canActivePause = true;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "HomeScene" || SceneManager.GetSceneAt(i).name == "OptionsScene")
                {
                    canActivePause = false;
                }
            }
            if (!FindAnyObjectByType<ManagementOpenCloseScene>().finishLoad)
            {
                canActivePause = false;
            }
            if (canActivePause)
            {
                ChangeSceneSelector(TypeScene.OptionsScene);
            }
        }
    }
    public void ChangeSceneSelector(TypeScene typeScene)
    {
        switch (typeScene)
        {
            case TypeScene.OptionsScene:
                SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
                break;
            case TypeScene.Exit:
                OpenCloseScene.openCloseSceneAnimator.Play("Out");
                OpenCloseScene.openCloseSceneAnimator.SetBool("Out", true);
                if (fadeIn != null) StopCoroutine(fadeIn);
                if (fadeOut != null) StopCoroutine(fadeOut);
                fadeOut = StartCoroutine(FadeOut());
                StartCoroutine(ChangeScene(typeScene));
                break;
            case TypeScene.NextLevel:
                if (fadeIn != null) StopCoroutine(fadeIn);
                if (fadeOut != null) StopCoroutine(fadeOut);
                fadeOut = StartCoroutine(FadeOut());
                OpenCloseScene.openCloseSceneAnimator.Play("Out");
                OpenCloseScene.openCloseSceneAnimator.SetBool("Out", true);
                StartCoroutine(NextLevel());
                break;
            default:
                if (fadeIn != null) StopCoroutine(fadeIn);
                if (fadeOut != null) StopCoroutine(fadeOut);
                fadeOut = StartCoroutine(FadeOut());
                OpenCloseScene.openCloseSceneAnimator.Play("Out");
                OpenCloseScene.openCloseSceneAnimator.SetBool("Out", true);
                StartCoroutine(ChangeScene(typeScene));
                break;
        }
    }
    public IEnumerator FadeIn()
    {
        float decibelsMaster = 20 * Mathf.Log10(ManagementData.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float currentVolumen = 0;
        float volume = 0;
        if (ManagementData.audioMixer.GetFloat(ManagementOptions.TypeSound.Master.ToString(), out volume))
        {
            currentVolumen = volume;
        }
        else
        {
            currentVolumen = -80f;
        }
        while (currentVolumen < decibelsMaster)
        {
            if (ManagementData.saveData.configurationsInfo.soundConfiguration.isMute) break;
            currentVolumen++;
            ManagementData.audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), currentVolumen);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    public IEnumerator ChangeScene(TypeScene typeScene)
    {
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(2);
        if (typeScene != TypeScene.Exit)
        {
            SceneManager.LoadScene(typeScene.ToString());
        }
        else
        {
            Application.Quit();
        }
        ChangedScene();
    }
    public IEnumerator NextLevel()
    {
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(2);
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
        ChangedScene();
    }
    public void EnterScene()
    {
        OpenCloseScene.openCloseSceneAnimator.SetBool("Out", false);
    }
    public void ChangedScene()
    {
        if (fadeIn != null) StopCoroutine(fadeIn);
        if (fadeOut != null) StopCoroutine(fadeOut);
        fadeIn = StartCoroutine(FadeIn());
    }
    public IEnumerator FadeOut()
    {
        float decibelsMaster = 20 * Mathf.Log10(ManagementData.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        while (decibelsMaster > -80)
        {
            if (ManagementData.saveData.configurationsInfo.soundConfiguration.isMute) break;
            decibelsMaster -= 1;
            ManagementData.audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), decibelsMaster);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    public void PlayASound(AudioClip audioClip)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.Play();
        Destroy(audioBox.gameObject, audioBox.clip.length);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.pitch = Random.Range(initialRandomPitch - 0.1f, initialRandomPitch + 0.1f);
        audioBox.Play();
        Destroy(audioBox.gameObject, audioBox.clip.length);
    }

    internal void SetAudioMixerData()
    {
        managementData.SetAudioMixerData();
    }

    public enum TypeScene
    {
        HomeScene = 0,
        OptionsScene = 1,
        GameScene = 2,
        Exit = 3,
        NextLevel = 4
    }
}
