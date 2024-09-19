using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Editor;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class FMODExt_Bus
{
    FMOD.Studio.Bus _bus;
    [SerializeField, ShowOnly] string _path;
    [SerializeField, ShowOnly] FMOD.RESULT _loadResult = FMOD.RESULT.ERR_UNIMPLEMENTED;
    public string Path => _path;
    public FMOD.RESULT LoadResult => _loadResult;

    [SerializeField]
    [Range(-80f, 10f)]
    private float busVolume;
    private float volume;

    public FMODExt_Bus(FMOD.Studio.Bus bus)
    {
        this._bus = bus;
        _loadResult = bus.getPath(out _path);
    }

    public FMODExt_Bus(string path)
    {
        _loadResult = FMODUnity.RuntimeManager.StudioSystem.getBus(path, out _bus);
        _path = path;
    }

    public FMOD.RESULT setVolume(float volume) => _bus.setVolume(volume);

    public void Update()
    {
        volume = Mathf.Pow(10.0f, busVolume / 20f);
        _bus.setVolume(volume);
    }
}