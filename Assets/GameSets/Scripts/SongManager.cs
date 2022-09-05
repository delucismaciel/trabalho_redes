using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    [SerializeField]
    AudioSource good, bad;

    public static SongManager songManager;

    private void Awake()
    {
        songManager = this;
    }

    public void PlayGood()
    {
        good.Play();
    }

    public void PlayBad()
    {
        bad.Play();
    }
}
