using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public sealed class SoundEntry
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Entries")]
    [SerializeField] private List<SoundEntry> m_soundEntries;

    private readonly Dictionary<string, AudioClip> m_soundDictionary =
                     new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        BuildDictonaries();
    }

    private void BuildDictonaries()
    {
        m_soundDictionary.Clear();

        for (int i = 0; i < m_soundEntries.Count; i++)
        {
            var entry = m_soundEntries[i];
            if (entry == null || entry.clip == null) { continue; }
            // Map by clip.name (for string-based access)
            var name = entry.name;
            if (!string.IsNullOrWhiteSpace(name) && !m_soundDictionary.ContainsKey(name))
            {
                m_soundDictionary.Add(name, entry.clip);
            }
        }
    }

    public void Play(AudioSource source, string clipName)
    {
        if (source == null)
        {
            Debug.LogWarning("AudioManager.Play: source is null.");
            return;
        }
        if (string.IsNullOrWhiteSpace(clipName))
        {
            Debug.LogWarning("AudioManager.Play: clipName is empty.");
            return;
        }
        if (!m_soundDictionary.TryGetValue(clipName, out var clip) || clip == null)
        {
            Debug.LogWarning($"AudioManager.Play: clip not found for name '{clipName}'.");
            return;
        }

        source.PlayOneShot(clip);
    }
}
