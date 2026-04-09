using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    public SoundLibraryEntry[] entries;

    internal AudioClip GetClipByEntryName(string entryName)
    {
        foreach (var entry in entries)
        {
            if (entry.entryName == entryName)
            {
                return entry.audioClip;
            }
        }

        throw new ArgumentException("Entry not found", nameof(entryName));
    }
    // [Header("Player Clips")]
    // public SoundLibraryEntry walkEntry;
    // public SoundLibraryEntry jumpEntry;
    // public SoundLibraryEntry attackEntry;
    // public SoundLibraryEntry takeDamageEntry;

    // [Header("Trampoline Clips")]
    // public SoundLibraryEntry trampolineBounceEntry;

    // [Header("Jumper Clips")]
    // public SoundLibraryEntry jumperBounceEntry;

    // [Header("Flutter Clips")]
    // public SoundLibraryEntry flutterEntry;
    // public SoundLibraryEntry shootEntry;

    // [Header("Razor Clips")]
    // public SoundLibraryEntry razorEntry;

    // [Header("Walker Spitter Clips")]
    // public SoundLibraryEntry walkerSpitterShootEntry;
    // public SoundLibraryEntry walkerSpitterWalkEntry;
}
