using System;
using System.Threading.Tasks;
using UnityEngine;
using _PacmanGame.Scripts.Actors;
using _PacmanGame.Scripts.Canvas.Fadeout;
using _PacmanGame.Scripts.Canvas.Score;

namespace _PacmanGame.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip Intro;
        public AudioClip Chomp;
        public AudioClip EatGhost;
        public AudioClip ExtraLife;
        public AudioClip EatFruit;
        public AudioClip Die;

        public AudioSource MunchSource;
        public AudioSource OthersSource;
        public AudioSource Siren;

        private const float MUNCH_TOLERANCE = .3f;
        private float munchCooldown;

        private const string VOLUME_PLAYERPREFS_KEY = "VOLUME";

        private void Start()
        {
            var volume = PlayerPrefs.GetFloat(VOLUME_PLAYERPREFS_KEY, .8f);

            OthersSource.volume = volume;
            MunchSource.volume = volume;

            ListenToBindings();
            MunchSource.clip = Chomp;
        }

        private void Update()
        {
            if ( munchCooldown > 0 )
            {
                munchCooldown -= Time.deltaTime;
                return;
            }

            if ( munchCooldown <= 0 && MunchSource.loop )
            {
                MunchSource.loop = false;
            }
        }

        private void OnDestroy()
        {
            RemoveBinding();
        }

        private void PlayMunch()
        {
            munchCooldown = MUNCH_TOLERANCE;
            MunchSource.loop = true;
            if ( !MunchSource.isPlaying )
            {
                MunchSource.Play();
            }
        }

        private void ListenToBindings()
        {
            InstructionsFadeout.StartIntro += () => PlayAudio(Intro);
            InstructionsFadeout.StartGame += () => Siren.Play();
            Pacman.EatDot += PlayMunch;
            Pacman.EatGhost += () => PlayAudio(EatGhost);
            Pacman.EatFruit += () => PlayAudio(EatFruit);
            Pacman.EatPowerDot += FreneticSiren;
            Pacman.Die += () => PlayAudio(Die);
            ScoreManager.ExtraLife += () => PlayAudio(ExtraLife);
        }

        private static void RemoveBinding()
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
            Siren.pitch = 1.5f;
            await Task.Delay(TimeSpan.FromSeconds(5f));
            NormalSiren();
        }

        private void NormalSiren()
        {
            Siren.pitch = 1f;
        }

        public void PlayAudio(AudioClip audioClip)
        {
            OthersSource.PlayOneShot(audioClip);
        }
    }
}