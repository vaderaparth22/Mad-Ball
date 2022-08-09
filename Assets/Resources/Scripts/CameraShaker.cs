using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    #region SINGLETON
    public static CameraShaker Instance = null;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(Instance);
    }
    #endregion

    public float intensity;
    public float time;
    public float onDeathIntensity;
    public float onDeathTime;
    public float dangerHitIntensity;
    public float dangerHitTime;

    CinemachineVirtualCamera cinemachineVirtualCamera;
    CinemachineBasicMultiChannelPerlin basicPerlin;
    float shakerTime;
    float startingIntesity;
    float shakerTimerTotal;

    public void Initialize()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        basicPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera()
    {
        basicPerlin.m_AmplitudeGain = intensity;
        startingIntesity = intensity;
        shakerTimerTotal = time;
        shakerTime = time;
    }

    public void ShakeCameraOnPlayerDeath()
    {
        basicPerlin.m_AmplitudeGain = onDeathIntensity;
        startingIntesity = onDeathIntensity;
        shakerTimerTotal = onDeathTime;
        shakerTime = onDeathTime;
    }

    public void ShakeCameraOnDangerTouch()
    {
        basicPerlin.m_AmplitudeGain = dangerHitIntensity;
        startingIntesity = dangerHitIntensity;
        shakerTimerTotal = dangerHitTime;
        shakerTime = dangerHitTime;
    }

    public void Refresh()
    {
        if(shakerTime > 0)
        {
            shakerTime -= Time.deltaTime;
            if(shakerTime <= 0)
            {
                basicPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntesity, 0f, 1 - (shakerTime / shakerTimerTotal));
            }
        }
    }
}
