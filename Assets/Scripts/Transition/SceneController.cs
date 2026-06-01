using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController> ,IEndGameObserver   //泛型单例
{
    public GameObject playerPrefab;   //玩家预制体
    public SceneFader sceneFaderPrefab;   //场景过渡动画
    bool fadeFinished;
    GameObject player;
    NavMeshAgent playerAgent;
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);   //场景切换时不销毁这个对象
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);   //注册结束游戏观察者
        fadeFinished= true; 
    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch(transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationType));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationType));
                break;
        }
    }

    IEnumerator Transition(string sceneName,TransitionDestination.DestinationType destinationTag)
    {
        SaveManager.Instance.SavePlayerData();   //保存玩家数据
        InventoryManager.Instance.SaveData();   //保存背包数据

        if (SceneManager.GetActiveScene().name != sceneName)   //如果目标场景和当前场景不同，则加载目标场景（跨场景传送）
        {
            SceneFader fade = Instantiate(sceneFaderPrefab);   //实例化场景过渡动画
            yield return StartCoroutine(fade.FadeOut(2f));   //播放过渡动画
            yield return SceneManager.LoadSceneAsync(sceneName);   //异步加载目标场景
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);   //在目标场景中生成玩家
            SaveManager.Instance.LoadPlayerData();   //加载玩家数据
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationType destinationType)
    {
        TransitionDestination[] destinations = FindObjectsOfType<TransitionDestination>();   //在当前场景中找到所有的传送目的地
        foreach (TransitionDestination destination in destinations)
        {
            if(destination.destinationType == destinationType)    //找到目的地类型和传入的目的地类型相同的目的地（A传A，B传B）
            {
                return destination;
            }
        }
        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToFiestLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade=Instantiate(sceneFaderPrefab);   //实例化场景过渡动画
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));   //播放过渡动画
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position , GameManager.Instance.GetEntrance().rotation);   //在新场景中生成玩家

            SaveManager.Instance.SavePlayerData();   //保存玩家数据
            InventoryManager.Instance.SaveData();   //保存背包数据

            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);   //实例化场景过渡动画
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished= false;   //确保结束游戏通知只会触发一次，防止重复触发玩家死亡后应该被调用的事件
            StartCoroutine(LoadMain());
        }
    }
}
