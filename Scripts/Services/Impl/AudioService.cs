using System.Collections.Generic;
using Reversi.Audio;
using Reversi.Common;
using Reversi.DB;
using UnityEngine;
using AudioInfo = Reversi.Services.IAudioService.AudioInfo;

namespace Reversi.Services.Impl
{
    public class AudioService : IAudioService
    {
        /// <summary>
        /// AudioManagerオブジェクト名
        /// </summary>
        private static readonly string AudioManagerObjectName = "AudioManager";

        /// <summary>
        /// Audioファイル格納パス
        /// </summary>
        private const string AudioPath = "Audio/";

        /// <summary>
        /// キャッシュしたAudioClip
        /// key: ファイル名
        /// </summary>
        private readonly IDictionary<string, AudioClip> _cachedAudioDictionary = new Dictionary<string, AudioClip>();

        /// <summary>
        /// AudioSource
        /// </summary>
        private readonly AudioSource _seAudioSource;  // SE再生用
        private readonly AudioSource _bgmAudioSource; // BGM再生用

        /// <summary>
        /// OnOffフラグ
        /// </summary>
        private bool _isSeOff = false;
        private bool _isBgmOff = false;

        /// <summary>
        /// Audio情報
        /// </summary>
        private readonly Dictionary<ReversiAudioType, AudioInfo> _audioInfos;

        public AudioService(PlayerPrefsRepository playerPrefsRepository)
        {
            // オーディオ情報を登録
            _audioInfos = ReversiAudioUtil.GetAudioInfos();

            // AudioManagerオブジェクトを作成
            var audioManager = GameObject.Find(AudioManagerObjectName);
            if (audioManager == null)
            {
                audioManager = new GameObject(AudioManagerObjectName);
                audioManager.AddComponent<DontDestroyBehaviour>();
                _seAudioSource = audioManager.AddComponent<AudioSource>();
                _bgmAudioSource = audioManager.AddComponent<AudioSource>();
            }

            // PlayerPrefsからオーディオ情報を設定
            _isSeOff = playerPrefsRepository.GetIsSeVolumeOff();
            _isBgmOff = playerPrefsRepository.GetIsBgmVolumeOff();
        }

        private ReversiAudioType GetReversiAudioType(string name)
        {
            foreach (var audioInfoKeyValue in _audioInfos)
            {
                if (audioInfoKeyValue.Value.Name == name)
                {
                    return audioInfoKeyValue.Key;
                }
            }
            return ReversiAudioType.None;
        }

        /// <summary>
        /// 効果音再生
        /// </summary>
        public void PlayOneShot(ReversiAudioType audioType)
        {
            // VolumeOff指定されていれば何もしない
            if (_isSeOff) return;

            var audioClip = LoadAudioClip(audioType);
            var volume = _seVolume * _audioInfos[audioType].Volume;
            _seAudioSource.PlayOneShot(audioClip, volume);
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        public void PlayBGM(ReversiAudioType audioType)
        {
            // 再生中なら止める
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }

            _bgmAudioSource.clip = LoadAudioClip(audioType);
            _bgmAudioSource.volume = _bgmVolume * _audioInfos[audioType].Volume;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.Play();

            // VolumeOff指定されている場合
            // ONにした時に再生されるよう、一時停止にしておく
            if (_isBgmOff)
            {
                _bgmAudioSource.Pause();
            }
        }

        /// <summary>
        /// BGM停止
        /// </summary>
        public void StopBGM()
        {
            // 再生中なら止める
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }
        }

        /// <summary>
        /// SEピッチ変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>
        public void ChangeSePitch(float pitch)
        {
            _seAudioSource.pitch = pitch;
        }

        /// <summary>
        /// ボリューム
        /// </summary>
        private float _bgmVolume = 1.0f;
        private float _seVolume = 1.0f;

        /// <summary>
        /// BGMボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBgmVolume(float volume)
        {
            _bgmVolume = volume;

            // 再生中のBGM音量を変更
            if (_bgmAudioSource.isPlaying)
            {
                var audioType = GetReversiAudioType(_bgmAudioSource.name);
                _bgmAudioSource.volume = _bgmVolume * _audioInfos[audioType].Volume;
            }
        }

        /// <summary>
        /// SEボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSeVolume(float volume)
        {
            _seVolume = volume;
        }

        /// <summary>
        /// AudioClipの読み込み
        /// </summary>
        /// <returns>AudioClip</returns>
        private AudioClip LoadAudioClip(ReversiAudioType audioType)
        {
            // ファイル名をキーとしてキャッシュする
            var fileName = _audioInfos[audioType]?.Name;
            if (!_cachedAudioDictionary.ContainsKey(fileName))
            {
                var audioClip = Resources.Load(AudioPath + fileName) as AudioClip;
                _cachedAudioDictionary.Add(fileName, audioClip);
            }
            return _cachedAudioDictionary[fileName];
        }

        /// <summary>
        /// BGMのOffフラグを設定
        /// </summary>
        public bool ChangeBgmVolumeOnOff()
        {
            _isBgmOff = !_isBgmOff;

            // 再生中のBGMを一旦停止する
            if (_isBgmOff)
            {
                _bgmAudioSource.Pause();
            }
            else
            {
                _bgmAudioSource.UnPause();
            }
            return _isBgmOff;
        }

        public bool GetIsBgmVolumeOff()
        {
            return _isBgmOff;
        }

        /// <summary>
        /// SEのOnOffフラグを設定
        /// </summary>
        public bool ChangeSeVolumeOnOff()
        {
            _isSeOff = !_isSeOff;
            return _isSeOff;
        }

        public bool GetIsSeVolumeOff()
        {
            return _isSeOff;
        }
    }
}
