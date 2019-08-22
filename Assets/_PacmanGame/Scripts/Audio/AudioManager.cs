using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Actors.Ghosts;
using _PacmanGame.Scripts.Canvas.Fadeout;
using _PacmanGame.Scripts.Canvas.Score;

public class AudioManager : MonoBehaviour
{
    public AudioClip Intro;
    public AudioClip Chomp;
    public AudioClip EatGhost;
    public AudioClip ExtraLife;
    public AudioClip EatFruit;
    public AudioClip Die;

    public AudioSource munchSource;
    public AudioSource othersSource;
    public AudioSource siren;
    
    private float munchTolerance = .3f;
    private float munchCooldown;

    private const string VOLUME_PLAYERPREFS_KEY = "VOLUME";

    private void Start()
    {
        var volume = PlayerPrefs.GetFloat(VOLUME_PLAYERPREFS_KEY, .8f);
        
        othersSource.volume = volume;
        munchSource.volume = volume;
        
        ListenToBindings();
        munchSource.clip = Chomp;
    }

    private void Update()
    {
        if ( munchCooldown > 0 )
        {
            munchCooldown -= Time.deltaTime;
            return;
        }

        if ( munchCooldown <= 0 && munchSource.loop);
        {
            munchSource.loop = false;
        }
    }

    private void OnDestroy()
    {
        RemoveBinding();
    }

    private void PlayMunch()
    {
        munchCooldown = munchTolerance;
        munchSource.loop = true;
        if ( !munchSource.isPlaying )
        {
            munchSource.Play();
        }
    }

    private void ListenToBindings()
    {
        InstructionsFadeout.StartIntro += () => PlayAudio(Intro);
        InstructionsFadeout.StartGame += () => siren.Play();
        Pacman.EatDot += PlayMunch;
        Pacman.EatGhost += () => PlayAudio(EatGhost);
        Pacman.EatFruit += () => PlayAudio(EatFruit);
        Pacman.EatPowerDot += FreneticSiren;
        Pacman.Die += () => PlayAudio(Die);
        ScoreManager.ExtraLife += () => PlayAudio(ExtraLife);
    }

    private void RemoveBinding()
    {
        InstructionsFadeout.StartIntro += null;
        InstructionsFadeout.StartGame += null;
        Pacman.EatDot += null;
        Pacman.EatGhost += null;
        Pacman.EatFruit += null;
        Pacman.EatPowerDot += null;
        Pacman.Die += null;
        ScoreManager.ExtraLife += null; 
    }

    private async void FreneticSiren()
    {
        siren.pitch = 1.5f;
        await Task.Delay(TimeSpan.FromSeconds(5f));
        NormalSiren();
    }

    private void NormalSiren()
    {
        siren.pitch = 1f;
    }

    public void PlayAudio(AudioClip audioClip)
    {
        othersSource.PlayOneShot(audioClip);
    }

}
