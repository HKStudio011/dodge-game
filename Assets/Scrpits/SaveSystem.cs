using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    
    public SaveSystem() { }
    public System.Tuple<List<string>, bool, float> LoadPlayer()
    {
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.data";
        if(File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Player Player = formatter.Deserialize(fileStream) as Player;
            fileStream.Close();
            return new System.Tuple<List<string>,bool, float>(Player.Achicevments, Player.RemoveADS,Player.HighTime);
        }
        else
        {
            List<string> temp = new List<string>();
            temp.Add("");
            return new System.Tuple<List<string>, bool, float>(temp, false, 0); 
        }
        
    }
    public void SavePlayer(List<string> achicevments, bool removeADS, float highTime)
    {
        BinaryFormatter formatter =new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.data";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        Player Player = new Player(achicevments, removeADS, highTime);
        formatter.Serialize(fileStream, Player);
        fileStream.Close();
    }
    public void SaveSetting(float musicVolume, float vFXVolume)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath+ "/setting.init";
        FileStream fileStream = new FileStream(path,FileMode.Create);
        SettingInit data=new SettingInit(musicVolume, vFXVolume);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }
    public System.Tuple<float,float> LoadSetting()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path  = Application.persistentDataPath + "/setting.init";

        if (File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            SettingInit data = formatter.Deserialize(fileStream) as SettingInit;
            fileStream.Close();
            return new System.Tuple<float, float>(data.MusicVolume, data.VFXVolume1);
        }
        else
        {
            return new System.Tuple<float, float>(0, 0);
        }
    }
}
[System.Serializable]
class SettingInit
{
    private float musicVolume;
    private float VFXVolume;
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float VFXVolume1 { get => VFXVolume; set => VFXVolume = value; }
    public SettingInit() { }
    public SettingInit(float musicVolume, float vFXVolume)
    {
        this.MusicVolume = musicVolume;
        VFXVolume1 = vFXVolume;
    }

    
}
[System.Serializable]
class Player
{
    private float highTime;
    private bool removeADS;
    private List<string> achicevments;
    public float HighTime { get => highTime; set => highTime = value; }
    public List<string> Achicevments { get => achicevments; set => achicevments = value; }
    public bool RemoveADS { get => removeADS; set => removeADS = value; }

    public Player() { }

    public Player(List<string> achicevments, bool removeADS, float highTime)
    {
        this.highTime = highTime;
        this.removeADS = removeADS;
        this.achicevments = achicevments;
    }
}
