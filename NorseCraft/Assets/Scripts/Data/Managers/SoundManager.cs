using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Audio Clips")]
    public List<AudioClip> audioClips;

    private Dictionary<string, AudioClip> audioClipDictionary;

	public void Initialize(GameObject player)
    {
        audioSource = player.GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("SoundManager: Failed to get player audio source.");

        audioClipDictionary = new Dictionary<string, AudioClip>();

        for(int i = 0; i < audioClips.Count; i++)
        {
            audioClipDictionary.Add(audioClips[i].name, audioClips[i]);
        }
    }

    public void PlayAudioClip(string audioTag)
    {
        if (audioClipDictionary.ContainsKey(audioTag))
        {
            audioSource.PlayOneShot(audioClipDictionary[audioTag]);
        }
        else
            Debug.LogError("SoundManager: no audio clip with tag " + audioTag);
    }
}
