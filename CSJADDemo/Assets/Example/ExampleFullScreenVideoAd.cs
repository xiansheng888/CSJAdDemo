using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

public class ExampleFullScreenVideoAd
{
    public static void LoadFullScreenVideoAd(Example example, bool isM)
    {
        if (example.fullScreenVideoAd != null)
        {
            example.fullScreenVideoAd.Dispose();
            example.fullScreenVideoAd = null;
        }
        var adSlot = new AdSlot.Builder()
            .SetCodeId(isM == false ? CSJMDAdPositionId.CSJ_ExpressFullScreen_V_ID :
            CSJMDAdPositionId.M_INTERSTITAL_FULL_SCREEN_ID) // 必传
            .SetOrientation(AdOrientation.Vertical)
            .SetMediationAdSlot(new MediationAdSlot.Builder()
                .SetScenarioId("ScenarioId") // 可选
                .SetUseSurfaceView(false) // 可选
                .SetBidNotify(true) // 可选
                .Build())
            .Build();
        SDK.CreateAdNative().LoadFullScreenVideoAd(adSlot, new FullScreenVideoAdListener(example));
    }

    public static void ShowFullScreenVideoAd(Example example)
    {
        if (example.fullScreenVideoAd == null)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
            return;
        }

        example.fullScreenVideoAd.SetFullScreenVideoAdInteractionListener(new FullScreenAdInteractionListener(example));
        example.fullScreenVideoAd.SetDownloadListener(new AppDownloadListener(example));
        example.fullScreenVideoAd.SetAdInteractionListener(new TTAdInteractionListener());

        example.fullScreenVideoAd.ShowFullScreenVideoAd();
    }

    // 广告加载监听器
    public sealed class FullScreenVideoAdListener : IFullScreenVideoAdListener
    {
        private Example example;

        public FullScreenVideoAdListener(Example example)
        {
            this.example = example;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + $"OnFullScreenError: {message}  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnFullScreenError: " + message;
        }

        public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
        {
            Debug.Log("CSJM_Unity "+ "Example " + $"OnFullScreenAdLoad  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnFullScreenAdLoad";

            this.example.fullScreenVideoAd = ad;
        }

        public void OnFullScreenVideoCached()
        {
            Debug.Log("CSJM_Unity "+ "Example " + $"OnFullScreenVideoCached  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnFullScreenVideoCached";
        }

        public void OnFullScreenVideoCached(FullScreenVideoAd ad)
        {
        }
    }

    // 广告展示监听器
    public sealed class FullScreenAdInteractionListener : IFullScreenVideoAdInteractionListener
    {
        private Example example;

        public FullScreenAdInteractionListener(Example example)
        {
            this.example = example;
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd show  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd show";
            }

            // log
            LogMediationInfo(example);
        }

        public void OnAdVideoBarClick()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd bar click  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd bar click";
            }
        }

        public void OnAdClose()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd close  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd close";
            }

            if (this.example.fullScreenVideoAd != null)
            {
                this.example.fullScreenVideoAd.Dispose();
                this.example.fullScreenVideoAd = null;
            }
        }

        public void OnVideoComplete()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd complete  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd complete";
            }
        }

        public void OnVideoError()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd OnVideoError  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd OnVideoError";
            }
        }

        public void OnSkippedVideo()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"fullScreenVideoAd OnSkippedVideo  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "fullScreenVideoAd skipped";
            }
        }
    }

    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.fullScreenVideoAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.fullScreenVideoAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.fullScreenVideoAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.fullScreenVideoAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.fullScreenVideoAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }

}
