using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyGenTest : MonoBehaviour
{
    [SerializeField] GameObject Enemy;

    Animator ani;


    private void Awake()
    {
        ani = GetComponent<Animator>(); 
    }

    private void Start()
    {
        GenEnemy();
    }

    private void GenEnemy()
    {
        ani.SetBool("isGen",true);
    }


    public void StartGen()
    {
        Enemy.transform.DOScale(2f, 0.2f).
            OnComplete(()=>Enemy.transform.DOScale(1.4f,0.1f).
            OnComplete(()=>ani.SetBool("isGen",false))
            );
    }



}
