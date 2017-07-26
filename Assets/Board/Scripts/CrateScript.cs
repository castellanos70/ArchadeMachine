﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{

    private static int strength = 15;
    public ParticleSystem crateParticles;
    public ParticleSystem spawnParticles;
    public AudioSource crateAudio;
    public Material crateMaterial;

    public AudioClip detonateClipSoft;
    public AudioClip detonateClipHard;
    public AudioClip[] spawnClip;

    public void spawnAnimation(GameObject player)
    {
        Renderer renderer = player.GetComponent<Renderer>();
        Texture texture = renderer.material.mainTexture;

        spawnParticles.GetComponent<Renderer>().material.mainTexture = texture;
        spawnParticles.Emit(10);

        crateAudio.clip = spawnClip[Random.Range(0, spawnClip.Length)];
        crateAudio.Play();
    }

    public void detonate(float speed)
    {
        //ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        //settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 1));
        crateParticles.Emit(10);
        //crateAudio.Stop();
        if (speed >= strength) crateAudio.clip = detonateClipHard;
        else crateAudio.clip = detonateClipSoft;
        crateAudio.Play();
        //crateAudio.clip = clipsDetonate[Random.Range(0, clipsDetonate.Length)];
        GetComponent<Renderer>().enabled = false;

        //crateObject.GetComponent<Renderer>().enabled = false;
        //crateObject.SetActive(false);
        Destroy(gameObject, 1);
    }




    public static int getStrength()
    {
        return strength;
    }
}
