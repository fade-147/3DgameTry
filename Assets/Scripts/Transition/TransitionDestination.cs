using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationType
    {
        ENTER,A,B, C    //进入点，A点，B点，C点
    }

    public DestinationType destinationType;    //目的地类型
}
