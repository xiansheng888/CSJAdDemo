using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

/**
 * 开屏广告代码示例
 * 注：该接口支持融合功能
 */
public class ExampleSplashAd
{
    public static void LoadAndShowSplashAd(Example example, bool isM)
    {
        if (example.splashAd != null)
        {
            example.splashAd.Dispose();
            example.splashAd = null;
        }

        int mSplashExpressWidthDp = 0;
        int mSplashExpressHeightDp = 0;
#if UNITY_ANDROID
        AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityContext = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
        float scale = unityContext.Call<AndroidJavaObject>("getResources").
            Call<AndroidJavaObject>("getDisplayMetrics").Get<float>("density");
        mSplashExpressWidthDp = (int)(Screen.width/scale + 0.5f);//根据设备像素宽度获取设备宽度DP
        mSplashExpressHeightDp = (int)(Screen.height/scale + 0.5f);//根据设备像素高度获取设备高度DP
#endif

        // 开屏自定义兜底，可选
        var mediationSplashReqInfo = new MediationSplashRequestInfo();
        mediationSplashReqInfo.AdnName = AdConst.ADN_PANGLE;
        mediationSplashReqInfo.AppId = CSJMDAdPositionId.M_SPLASH_BASELINE_APPID;
        mediationSplashReqInfo.Appkey = ""; // 穿山甲不需要appkey
        mediationSplashReqInfo.AdnSlotId = CSJMDAdPositionId.M_SPLASH_BASELINE_ID;

        string codeId = isM == false ? CSJMDAdPositionId.CSJ_SPLASH_V_ID : CSJMDAdPositionId.M_SPLASH_EXPRESS_ID;
        var adSlot = new AdSlot.Builder()
            .SetCodeId(codeId) // 必传
            .SetExpressViewAcceptedSize(mSplashExpressWidthDp, mSplashExpressHeightDp)  //普通开屏也需要设置模版size，单位dp
            .SetMediationAdSlot(new MediationAdSlot.Builder()
                .SetScenarioId("ScenarioId") // 可选
                .SetBidNotify(true) // 可选
                .SetMediationSplashRequestInfo(mediationSplashReqInfo) // 可选
                .Build())
            .Build();
        SDK.CreateAdNative().LoadSplashAd(adSlot, new SplashAdListener(example), 3500);
    }

    private static void ShowSplashAd(Example example)
    {
        Debug.Log("CSJM_Unity " + "Example " + "SetSplashInteractionListener Invoke");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            example.information.text = "show Splash";

        example.splashAd.SetSplashInteractionListener(new SplashAdInteractionListener(example));
        example.splashAd.SetDownloadListener(new AppDownloadListener(example));
        example.splashAd.SetAdInteractionListener(new TTAdInteractionListener());
        example.splashAd.ShowSplashAd();
    }

    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.splashAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.splashAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.splashAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.splashAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.splashAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }

    // 广告加载监听器
    public sealed class SplashAdListener : ISplashAdListener
    {
        private Example example;
        public SplashAdListener(Example example)
        {
            this.example = example;
        }

        public void OnSplashLoadFail(int code, string message)
        {
            Debug.Log("CSJM_Unity " + "Example " + "splash load OnSplashLoadFail:" + code + ":" + message +
                      $" on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnSplashLoadFail:" + code + ":" + message;

            if (this.example.splashAd != null)
            {
                this.example.splashAd.Dispose();
                this.example.splashAd = null;
            }
        }

        public void OnSplashLoadSuccess(BUSplashAd ad)
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnSplashLoadSuccess");
#if UNITY_IOS
            example.splashAd = ad;
            ShowSplashAd(example);
#endif
        }

        public void OnSplashRenderSuccess(BUSplashAd ad)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"splash load OnRenderSuccess:on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnRenderSuccess";
#if UNITY_ANDROID
            example.splashAd = ad;
            ShowSplashAd(example);
#endif
        }

        public void OnSplashRenderFail(int code, string message)
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnRenderFailed");
        }
    }

    // 广告展示监听器
    private sealed class SplashAdInteractionListener : ISplashAdInteractionListener
    {
        private Example example;

        public SplashAdInteractionListener(Example example)
        {
            this.example = example;
        }

        /// <summary>
        /// Invoke when the Ad is clicked.
        /// </summary>
        public void OnAdClicked(int type)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"splash Ad OnAdClicked type {type} on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "Splash OnAdClicked";
        }

        /// <summary>
        /// Invoke when the Ad is shown.
        /// </summary>
        public void OnAdDidShow(int type)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"splash Ad OnAdDidShow on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "Splash OnAdDidShow";

            LogMediationInfo(example);
        }

        public void OnAdWillShow(int type)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"splash Ad OnAdWillShow on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "Splash OnAdWillShow";
        }

        public void OnAdClose(int type)
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnAdClose tpye = " + type);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "Splash OnAdClose";

            this.example.splashAd.Dispose();
            this.example.splashAd = null;
        }
    }
}