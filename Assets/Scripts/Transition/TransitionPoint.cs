using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,    //同场景切换
        DifferentScene    //不同场景切换
    }
    [Header("Transition Info")]
    public string sceneName;    //要切换到的场景名称
    public TransitionType transitionType;    //切换类型

    public TransitionDestination.DestinationType destinationType;    //目的地类型

    private bool canTrans;

    private void Update()
    {
        if(canTrans && Input.GetKeyDown(KeyCode.E))
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
