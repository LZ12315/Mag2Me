using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItmesMove : MonoBehaviour
{
    [SerializeField] Transform A;
    [SerializeField] Transform B;

    [SerializeField] float moveTime;



    private void Start()
    {
        MoveToB();
    }

    private void MoveToB()
    {
        gameObject.transform.position = A.position;
        gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.5f).OnComplete(() => gameObject.transform.DORotate(new Vector3(0, 0, 180), moveTime - 0.5f));
        gameObject.transform.DOMove(B.position , moveTime).OnComplete(()=> MoveToA());
    }


    private void MoveToA()
    {
        gameObject.transform.position = A.position;
        gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.5f).OnComplete(() => gameObject.transform.DORotate(new Vector3(0, 0, 180), moveTime - 0.5f));
        gameObject.transform.DOMove(B.position, moveTime).OnComplete(()=> MoveToB());
    }



}
