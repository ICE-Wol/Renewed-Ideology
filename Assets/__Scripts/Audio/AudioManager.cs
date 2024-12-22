using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioNames
{
    SeShootBoon0,  
    SeShootBoon1,  
    SeShootKira0,  
    SeShootKira1,  
    SeShootLaser0,  
    SeShootLaser1,  
    SeShootLaser2,  
    SeShootSlash,  
    SeShootTan,  
    SeShootWater,  
    SeCharge,  
    SeDamage0,  
    SeDamage1,  
    SeEnemyExplode0,  
    SeEnemyExplode1,  
    SeBossExplode,  
    SeFault,  
    SeGraze, 
    SeItem,
    SeMissileLaunch,  
    SeMissileExplode,  
    SeMarisaBomb,  
    SePlayerDead,  
    SePlayerShoot,  
    SeTimeout0,  
    SeTimeout1,  
    SeBonus0,  
    SeBonus1,  
    MusStageDemoMid

}

public enum AudioType
{
    Music,
    Shoot,
    Others
}

[Serializable]
public class Sound
{
    public string name;
    public AudioNames enumName;
    public AudioClip clip;

    [UnityEngine.Range(0f, 1f)]
    public float volume = 0.1f;
    [UnityEngine.Range(0.1f, 3f)]
    public float pitch = 1f;

    public AudioType audioType;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{

    public static AudioManager Manager;
    public List<Sound> sounds;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup ShootGroup;
    public AudioMixerGroup otherGroup;
    public bool playBGM;
    void Awake()
    {
        if (Manager == null) {
            Manager = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            switch (sound.audioType) {
                case AudioType.Music:
                    sound.source.outputAudioMixerGroup = musicGroup;
                    break;
                case AudioType.Shoot:
                    sound.source.outputAudioMixerGroup = ShootGroup;
                    break;
                case AudioType.Others:
                    sound.source.outputAudioMixerGroup = otherGroup;
                    break;
                
            }
        }
        
         //for(int i = 0; i < sounds.Count; i++) {
             //sounds[i].enumName = (AudioNames) i;
            //sounds[i].volume = 0.01f;
         //}
        
        if(playBGM) PlaySound("MusStageDemoMid");
    }

    public void PlaySound(string name)
    {
        var sound = sounds.Find(s => s.name == name);
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        // if(sound.hasRandomPitch) {
        //     sound.source.pitch = UnityEngine.Random.Range(sound.pitch - 0.1f, sound.pitch + 0.1f);
        // }
        sound.source.Play();
    }
    
    public void PlaySound(AudioNames name) {
        var sound = sounds[(int)name];
        if (sound == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        // if(sound.hasRandomPitch) {
        //     sound.source.pitch = UnityEngine.Random.Range(sound.pitch - 0.1f, sound.pitch + 0.1f);
        // }
        sound.source.Play();
    }
}
