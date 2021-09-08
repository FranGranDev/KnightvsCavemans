using UnityEngine;
using System.Collections;

public class Vibration : MonoBehaviour
{
    private static Vibration active;
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject vibrator;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaClass vibrationEffectClass;
    public static int defaultAmplitude;

    private void Awake()
    {
        active = this;
    }
    void OnEnable()
    {
        
#if UNITY_ANDROID && !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        if (getSDKInt() >= 26) {
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
        }
#endif
    }

    public static void Vibrate(float Power)
    {
        if (GameData.Vibration)
        {
            CreateOneShot((long)(50f * Power));
        }
    }
    public static void Vibrate(Man.HitType type)
    {
        if (GameData.Vibration)
        {
            float Power = 0.1f;
            switch(type)
            {
                case Man.HitType.Hit:
                    Power = 0.1f;
                    break;
                case Man.HitType.Punch:
                    Power = 0.5f;
                    break;
                case Man.HitType.Tackle:
                    Power = 0.25f;
                    break;
                case Man.HitType.Fall:
                    Power = 0.5f;
                    break;
            }
            CreateOneShot((long)(100f * Power));
        }
    }

    public static void WinVibrate(float Delay)
    {
        if (GameData.Vibration)
        {
            active.StartCoroutine(active.WinVibrateCour(Delay));
        }
    }
    private IEnumerator WinVibrateCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);

        CreateOneShot(10);
        yield return new WaitForSeconds(0.25f);
        CreateOneShot(15);
        yield return new WaitForSeconds(0.25f);
        CreateOneShot(20);
        yield return new WaitForSeconds(0.5f);
        CreateOneShot(25);
        yield break;
    }

    public static void LoseVibrate(float Delay)
    {
        if (GameData.Vibration)
        {
            active.StartCoroutine(active.LoseVibrateCour(Delay));
        }
    }
    private IEnumerator LoseVibrateCour(float Delay)
    {
        yield return new WaitForSeconds(Delay);

        CreateOneShot(20);
        yield return new WaitForSeconds(0.25f);
        CreateOneShot(10);
        yield return new WaitForSeconds(0.75f);
        CreateOneShot(5);
        yield break;
    }

    public static void CreateOneShot(long milliseconds)
    {

        if (isAndroid())
        {
            if (getSDKInt() >= 26)
            {
                CreateOneShot(milliseconds, defaultAmplitude);
            }
            else
            {
                OldVibrate(milliseconds);
            }
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    public static void CreateOneShot(long milliseconds, int amplitude)
    {

        if (isAndroid())
        {
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createOneShot", new object[] { milliseconds, amplitude });
            }
            else
            {
                OldVibrate(milliseconds);
            }
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    public static void CreateWaveform(long[] timings, int repeat)
    {
        if (isAndroid())
        {
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createWaveform", new object[] { timings, repeat });
            }
            else
            {
                OldVibrate(timings, repeat);
            }
        }
        else
        {
            Handheld.Vibrate();
        }
    }

    public static void CreateWaveform(long[] timings, int[] amplitudes, int repeat)
    {
        if (isAndroid())
        {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26)
            {
                CreateVibrationEffect("createWaveform", new object[] { timings, amplitudes, repeat });
            }
            else
            {
                OldVibrate(timings, repeat);
            }
        }
        //If not android do simple solution for now
        else
        {
            Handheld.Vibrate();
        }

    }

    private static void CreateVibrationEffect(string function, params object[] args)
    {

        AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(function, args);
        vibrator.Call("vibrate", vibrationEffect);
    }

    private static void OldVibrate(long milliseconds)
    {
        vibrator.Call("vibrate", milliseconds);
    }
    private static void OldVibrate(long[] pattern, int repeat)
    {
        vibrator.Call("vibrate", pattern, repeat);
    }

    public static bool HasVibrator()
    {
        return vibrator.Call<bool>("hasVibrator");
    }

    public static bool HasAmplituideControl()
    {
        if (getSDKInt() >= 26)
        {
            return vibrator.Call<bool>("hasAmplitudeControl"); //API 26+ specific
        }
        else
        {
            return false; //If older than 26 then there is no amplitude control at all
        }

    }

    public static void Cancel()
    {
        if (isAndroid())
            vibrator.Call("cancel");
    }

    private static int getSDKInt()
    {
        if (isAndroid())
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }
        else
        {
            return -1;
        }

    }

    private static bool isAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
	    return true;
#else
        return false;
#endif
    }
}