using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName="";    //保存当前场景名称的键，sceneName是一个字符串变量，用于在PlayerPrefs中保存当前场景的名称
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))   //按下esc返回主场景
        {
            SceneController.Instance.TransitionToMain();  
        }
    }
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData,GameManager.Instance.playerStats.characterData.name);   
    }
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData,GameManager.Instance.playerStats.characterData.name);   
    }

    public void Save(Object data,string key)
    {
        var jsonData=JsonUtility.ToJson(data,true); //JsonUtility是Unity提供的一个类，用于在Unity中进行JSON序列化和反序列化。ToJson方法将一个对象转换为JSON格式的字符串，data是要转换的对象，jsonData是转换后的JSON字符串。
        PlayerPrefs.SetString(key, jsonData);    //PlayerPrefs是Unity提供的一个类，用于存储和访问玩家的偏好设置和数据。SetString方法用于将一个字符串值与指定的键关联起来，jsonData是要保存的数据，key是这个数据的标识符。
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);   //将当前场景的名称保存到PlayerPrefs中，sceneName是保存当前场景名称的键，SceneManager.GetActiveScene().name获取当前活动场景的名称。
        PlayerPrefs.Save();    //PlayerPrefs.Save()方法用于将所有未保存的更改写入磁盘。它确保之前使用PlayerPrefs.SetString()、PlayerPrefs.SetInt()或PlayerPrefs.SetFloat()等方法设置的数据被持久化保存。
    }

    public void Load(Object data, string key)
    {
        if(PlayerPrefs.HasKey(key))    //HasKey方法用于检查PlayerPrefs中是否存在指定的键，如果存在则返回true，否则返回false。
        {
            var jsonData=PlayerPrefs.GetString(key);    //GetString方法用于从PlayerPrefs中获取与指定键关联的字符串值，key是要获取的数据的标识符，jsonData是获取到的JSON字符串。
            JsonUtility.FromJsonOverwrite(jsonData, data);   //FromJsonOverwrite方法用于将JSON格式的字符串反序列化为一个对象，并将其覆盖到现有对象中，jsonData是要反序列化的JSON字符串，data是要覆盖的现有对象。
        }
    }
}
