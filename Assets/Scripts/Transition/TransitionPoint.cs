using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,DifferentScene
    }

    [Header("Transition Info")]
    //传送门名称
    public string sceneName;
    //传送门类型
    public TransitionType transitionType;
    //传送点标签
    public TransitionDestination.DestinationTag destinationTag;
    //可以传送的布尔值
    private bool canTrans;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canTrans)
        {
            //SceneController 传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
