using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioClip battleStartClip;
        [SerializeField] private AudioClip waveAlertClip;
        [SerializeField] private AudioClip unitHitClip;
        [SerializeField] private AudioClip unitDeathClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip defeatClip;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayBattleStart()
        {
            PlayClip(battleStartClip);
        }

        public void PlayWaveAlert()
        {
            PlayClip(waveAlertClip);
        }

        public void PlayUnitHit()
        {
            PlayClip(unitHitClip);
        }

        public void PlayUnitDeath()
        {
            PlayClip(unitDeathClip);
        }

        public void PlayVictory()
        {
            PlayClip(victoryClip);
        }

        public void PlayDefeat()
        {
            PlayClip(defeatClip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (_audioSource == null || clip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(clip);
        }
    }
}
