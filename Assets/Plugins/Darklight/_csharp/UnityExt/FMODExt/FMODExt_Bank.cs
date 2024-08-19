using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Darklight.UnityExt.Editor;
using System.Linq;



/// <summary>
/// A serializable class that mannages and stores FMOD bank data
/// </summary>
[System.Serializable]
public class FMODExt_Bank
{
    FMOD.GUID _guid;
    FMOD.Studio.Bank _bank;
    FMOD.Studio.EventDescription[] _events;
    FMOD.Studio.Bus[] _buses;
    List<FMODExt_Bus> _busData;
    public List<FMODExt_Bus> BusData => _busData;
    bool initialized;

    [SerializeField, ShowOnly] string _path;
    public string Path => _path;

    [Header("Buses")]
    [SerializeField, ShowOnly] int _busCount;
    [SerializeField, ShowOnly] FMOD.RESULT _busListOk = FMOD.RESULT.ERR_UNIMPLEMENTED;
    [SerializeField, ShowOnly] List<string> _busPaths = new();

    [Header("Events")]
    [SerializeField, ShowOnly] int _eventCount;

    // --------------------- [[ CONSTRUCTORS ]] --------------------- //
    public FMODExt_Bank(FMOD.Studio.Bank bank)
    {
        _bank = bank;
        initialized = true;

        // Load Identifiers
        bank.getID(out _guid);
        bank.getPath(out _path);

        // Load Buses
        bank.getBusCount(out _busCount);
        _busListOk = bank.getBusList(out _buses);
        _busPaths = _buses.ToList().ConvertAll(b => b.getPath(out string path) == FMOD.RESULT.OK ? path : "ERROR");
        _busData = _buses.ToList().ConvertAll(b => new FMODExt_Bus(b));

        // Load Events
        bank.getEventCount(out _eventCount);
        bank.getEventList(out _events);
    }

    public void UnloadBank()
    {
        if (initialized)
        {
            _bank.unload();
            _events.ToList().Clear();
            _buses.ToList().Clear();
            initialized = false;
            Debug.Log($"Bank {_path} unloaded successfully.");
        }
        else
        {
            Debug.LogWarning($"Bank {_path} is not loaded.");
        }
    }
}
