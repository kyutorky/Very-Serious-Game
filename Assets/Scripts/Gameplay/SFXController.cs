using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SFXController
{
    public List<AudioSource> sources;
    [SerializeField] GameObject sfxSourcesGO;

    public SFXController(GameObject sfxSourcesGO)
    {
        this.sfxSourcesGO = sfxSourcesGO;
        sources = new List<AudioSource>();
        for (int i = 0; i < sfxSourcesGO.transform.childCount; i++)
        {
            sources.Add(sfxSourcesGO.transform.GetChild(i).GetComponent<AudioSource>());
        }
    }

}
