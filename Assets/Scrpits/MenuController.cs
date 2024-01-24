using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    
    public GameObject menuGroup;
    public GameObject settingGroup;
    public GameObject buyGroup;
    public GameObject musicSlider;
    public GameObject VFXSlider;
    public AudioMixer mixer;
    
    private SaveSystem saveSystem;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        saveSystem = new SaveSystem();
        Menu();
        LoadSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play()
    {
        SceneManager.LoadScene(1);

    }
    public void Exit()
    {
        Application.Quit();
    }
    
    public void Menu()
    {
        menuGroup.SetActive(true);
        settingGroup.SetActive(false);
        buyGroup.SetActive(false);
    }
    public void Setting()
    {
        menuGroup.SetActive(false);
        settingGroup.SetActive(true);
        buyGroup.SetActive(false);
    }
    
    public void Buy()
    {
        menuGroup.SetActive(false);
        settingGroup.SetActive(false);
        buyGroup.SetActive(true);
    }
    public void MusicVolume( float volume)
    {
        mixer.SetFloat("Music", volume);
        if(volume <=-20) mixer.SetFloat("Music", -80);
    }
    public void SFXVolume(float volume)
    {
        mixer.SetFloat("SFX", volume);
        if (volume <= -20) mixer.SetFloat("SFX", -80);
    }
    public void LoadSetting()
    {
        var setting=saveSystem.LoadSetting();
        musicSlider.GetComponent<Slider>().value=setting.Item1;
        VFXSlider.GetComponent<Slider>().value=setting.Item2;
    }
    public void SaveSetting()
    {
        saveSystem.SaveSetting(musicSlider.GetComponent<Slider>().value, VFXSlider.GetComponent<Slider>().value);
    }
    public void RemoveADS()
    {
        var temp =saveSystem.LoadPlayer();
        saveSystem.SavePlayer(temp.Item1, true, temp.Item3);
        Menu();
    }
}
