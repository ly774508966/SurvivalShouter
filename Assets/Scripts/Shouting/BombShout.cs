﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class BombShout : MonoBehaviour {
    
    // import splistener library functions
    [DllImport("splistener", CallingConvention = CallingConvention.StdCall)]
    private static extern bool spInitListener(string model_path, string mic_name, int sample_rate, int delay);
    [DllImport("splistener", CallingConvention = CallingConvention.StdCall)]
    private static extern string spGetWords();
    [DllImport("splistener", CallingConvention = CallingConvention.StdCall)]
    private static extern void spCleanUp();
    [DllImport("splistener", CallingConvention = CallingConvention.StdCall)]
    private static extern string spGetError();


    public GameObject bomb;
    public float cooldown = 10.0f;
    private float lastTime;

    void ProcessShout(string shout) {
        if(Time.time - lastTime > cooldown 
            && (shout.IndexOf("bomb") >= 0 
            || shout.IndexOf("explode") >= 0 
            || shout.IndexOf("blow") >= 0)) {
            Instantiate(bomb, 
                        new Vector3(transform.position.x, 
                                    transform.position.y, 
                                    transform.position.z), 
                        Quaternion.identity);
            lastTime = Time.time;
        }

    }

    IEnumerator Listen() {
        string s;
        while (true) {
            s = spGetWords();
            if (s.Length > 0) {
                Debug.Log(s);
                ProcessShout(s);
            }
            yield return null;
        }
    }
    
    void Awake() {
        lastTime = -cooldown;
        if (!spInitListener(Application.dataPath + "/libs/pocketsphinx/model/en-us/", 
            null, 16000, 100)) {
            Debug.Log("splistener failed to initialize!");
            Debug.Log("splistener error: " + spGetError());
        }
        else {
            Debug.Log("splistener initialized successfully!");
            StartCoroutine(Listen());
        }
    }

    void OnApplicationQuit() {
        spCleanUp();
    }
}
