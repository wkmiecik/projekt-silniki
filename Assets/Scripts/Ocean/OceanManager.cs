using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    public GameObject attachedTo;
    public float waterHeight = 50f;
    public float gravity = 9.8f;
    public Vector4 waveA = new Vector4(-1.2f, 1f, 0.1f, 20f);
    public Vector4 waveB = new Vector4(1f, 0.6f, 0.1f, 15f);
    public Vector4 waveC = new Vector4(1f, 13f, 0.1f, 10f);
    public Vector4 waveD = new Vector4(0f, 1f, 0.1f, 5f);
    public Vector4 wavesEnabled = new Vector4(1f, -1f, -1f, -1f);

    Transform ocean;
    [SerializeField] Material oceanMat;


    void Start()
    {
        SetVars();
        UpdateMaterials();
    }


    void SetVars() {
        ocean = GetComponent<Transform>();
    }


    public float GetHeightAtPosition(Vector3 position) {
        float result = 0;
        float startHeight = 0;
        RaycastHit hit;
        Vector3 truePoint;
        truePoint = position;

        for (int i = 0; i < 4; i++) {
            if (Physics.Raycast(new Vector3(truePoint.x, 100, truePoint.z), -Vector3.up, out hit, 200f, LayerMask.GetMask("OceanFloor"))) {
                startHeight = 100 - hit.distance;
            }
            Vector3 iter = GetGerstnerAtPositon(new Vector3(truePoint.x, startHeight, truePoint.z));
            truePoint.x += position.x - iter.x;
            truePoint.z += position.z - iter.z;
            result = iter.y;
        }

        return result;
    }


    public Vector3 GetGerstnerAtPositon(Vector3 pos) {
        Vector3 p = new Vector3(pos.x, pos.y, pos.z);
        if (wavesEnabled.x > 0) p += GerstnerWave(waveA, pos);
        if (wavesEnabled.y > 0) p += GerstnerWave(waveB, pos);
        if (wavesEnabled.z > 0) p += GerstnerWave(waveC, pos);
        if (wavesEnabled.w > 0) p += GerstnerWave(waveD, pos);

        return p;
    }


    Vector3 GerstnerWave(Vector4 wave, Vector3 p) {
        float steepness = wave.z;
        float wavelength = wave.w;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(gravity / k);
        Vector2 d = new Vector2(wave.x, wave.y).normalized;
        float f = k * (Vector2.Dot(d, new Vector2(p.x, p.z)) - (c * Time.time));
        float a = steepness / k;

        Vector3 result = new Vector3(d.x * (a * Mathf.Cos(f)) * transform.localScale.x, a * Mathf.Sin(f) * transform.localScale.y, d.y * (a * Mathf.Cos(f)) * transform.localScale.z);
        return result;
    }


    void OnValidate() {
        if (!oceanMat) SetVars();
        UpdateMaterials();
    }


    void UpdateMaterials() {
        oceanMat.SetFloat("_Gravity", gravity);
        oceanMat.SetFloat("_WaterHeight", waterHeight);
        oceanMat.SetVector("_WaveA", waveA);
        oceanMat.SetVector("_WaveB", waveB);
        oceanMat.SetVector("_WaveC", waveC);
        oceanMat.SetVector("_WaveD", waveD);
        oceanMat.SetVector("_WavesEnabled", wavesEnabled);
    }
}
