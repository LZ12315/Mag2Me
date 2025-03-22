using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private static AudioManager instance;
    public static AudioManager Instance { get => instance ?? (instance = new AudioManager()); }


    /// <summary>
    /// ≤•∑≈“Ù¿÷
    /// </summary>
    /// <param name="path"></param>
    public void PlayMusic(string path)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Music");

        aud = go.GetComponent<AudioSource>();

        aud.clip = Resources.Load<AudioClip>(path);

        aud.Play();
    }


    /// <summary>
    /// »√“Ù¿÷…˘“Ù+=o
    /// </summary>
    /// <param name="o"></param>
    public void SetMusic(float o)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Music");

        aud = go.GetComponent<AudioSource>();

        if (o <= 0)
        {
            if (aud.volume >= 0.1f)
            {
                aud.volume += o;
            }
        }
        else
        {
            aud.volume += o;
        }
    }


    /// <summary>
    /// ≤•∑≈“Ù–ß
    /// </summary>
    /// <param name="path"></param>
    public void PlaySound(string path)
    {

        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Sound");

        aud = go.GetComponent<AudioSource>();


        AudioClip clip = Resources.Load<AudioClip>(path);

        aud.PlayOneShot(clip);
    }




    public void SetSound(float o)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Sound");

        aud = go.GetComponent<AudioSource>();
        aud.volume += o;
    }


    public void PlayBreath(string path)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Breath");

        aud = go.GetComponent<AudioSource>();


        AudioClip clip = Resources.Load<AudioClip>(path);

        aud.PlayOneShot(clip);
    }


    public void SetBreath(float o)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("Breath");

        aud = go.GetComponent<AudioSource>();
        aud.volume += o;
    }



    /// <summary>
    /// …Ë÷√BGM
    /// </summary>
    /// <param name="o"></param>
    public void SetBGM(float o)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("BGM");

        aud = go.GetComponent<AudioSource>();

        if (o <= 0)
        {
            if (aud.volume >= 0.1f)
            {
                aud.volume += o;
            }
        }
        else
        {
            aud.volume += o;
        }
    }


    public void PlayBGM(string path)
    {
        AudioSource aud;
        GameObject go = GameObject.FindGameObjectWithTag("BGM");

        aud = go.GetComponent<AudioSource>();

        aud.clip = Resources.Load<AudioClip>(path);

        aud.Play();
    }


}
