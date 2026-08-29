using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

/**
 * 模板banner代码示例
 * 注：该接口支持融合功能。并且支持将信息流混出到banner中。
 */
public class ExampleExpressBannerAd
{
    public static void LoadExpressBannerAd(Example example, bool isM)
    {
        if (example.mExpressBannerAd != null)
        {
            example.mExpressBannerAd.Dispose();
            example.mExpressBannerAd = null;
        }

        int width = (int) (UnityEngine.Screen.width / UnityEngine.Screen.dpi * 160);
        int height = (int)((float)width / 250 * 150); 
        Debug.Log("CSJM_Unity " + "Example " + "express banner w: " + width + ", h: " + height + ", dpi: " + (UnityEngine.Screen.dpi/160));

        string adsRit = isM ? CSJMDAdPositionId.M_BANNER_ID : CSJMDAdPositionId.CSJ_BANNER_ID;
        
        var adSlot = new AdSlot.Builder()
            .SetCodeId(adsRit) // 必传
            .SetSlideIntervalTime(30) // 单位秒。仅当单独使用csj是生效，启用融合时使用的是Gromore线上配置。
            //期望模板广告view的size,单位dp
            .SetExpressViewAcceptedSize(width, height)
            .SetMediationAdSlot(
                new MediationAdSlot.Builder()
                    .SetBidNotify(true) // 可选
                    .SetScenarioId("unity-SetScenarioId") // 可选
                    .SetWxAppId("unity-wxAppId") // 可选
                    .SetAllowShowCloseBtn(true) // 可选
                    .SetMuted(true)
                    .SetVolume(0.7f)
                    .Build())
            .Build();

        SDK.CreateAdNative().LoadExpressBannerAd(adSlot, new ExpressBannerAdListener(example));
    }

    public static void ShowExpressBannerAd(Example example)
    {
        if (example.mExpressBannerAd == null)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
            return;
        }

#if UNITY_ANDROID
        example.mExpressBannerAd.SetSlideIntervalTime(30 * 1000);
#endif
        example.mExpressBannerAd.SetExpressInteractionListener(new ExpressBannerInteractionListener(example));
        example.mExpressBannerAd.SetDislikeCallback(new ExpressAdDislikeCallback(example));
        example.mExpressBannerAd.SetDownloadListener(new AppDownloadListener(example));
        example.mExpressBannerAd.SetAdInteractionListener(new TTAdInteractionListener());
        example.mExpressBannerAd.ShowExpressAd(0, 500);
    }

    // 广告加载监听器
    public sealed class ExpressBannerAdListener : IExpressBannerAdListener
    {
        private Example example;

        public ExpressBannerAdListener(Example example)
        {
            this.example = example;
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "ExpressBannerAdListener";
        }

        public void OnError(int code, string message)
        {
            Debug.Log("CSJM_Unity "+ "Example " + "onExpressAdError: " + message);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "onExpressBannerAdError";
        }

        public void OnBannerAdLoad(ExpressBannerAd ad)
        {
            Debug.Log("CSJM_Unity "+ "Example " + "OnExpressBannerAdLoad");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnExpressBannerAdLoad";
            this.example.mExpressBannerAd = ad;
        }
    }

    // 广告展示监听器
    public sealed class ExpressBannerInteractionListener : IExpressBannerInteractionListener
    {
        private Example example;

        public ExpressBannerInteractionListener(Example example)
        {
            this.example = example;
        }

        public void OnAdClicked()
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnAdClicked");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnAdClicked:";
            this.example.mExpressBannerAd.UploadDislikeEvent("csjm_unity expressBanner dislike test");
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnAdShow");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "OnAdShow";
            }

            LogMediationInfo(example);
        }

        public void OnAdViewRenderError(int code, string message)
        {
            Debug.Log("CSJM_Unity " + "Example " + "express banner OnAdViewRenderError");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "express banner OnAdViewRenderError code: " + code + ", msg: " + message;
        }

        public void OnAdViewRenderSucc(float width, float height)
        {
            Debug.Log("CSJM_Unity " + "Example " + "express banner OnAdViewRenderSucc");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "express banner OnAdViewRenderSucc:";
        }

        public void OnAdClose()
        {
            Debug.Log("CSJM_Unity " + "Example " + "OnAdClose");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnAdClose:";
        }

        public void onAdRemoved()
        {
            Debug.Log("CSJM_Unity " + "Example " + "onAdRemoved");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "onAdRemoved:";
        }
    }

    // dislike监听器
    public sealed class ExpressAdDislikeCallback : IDislikeInteractionListener
    {
        private Example example;

        public ExpressAdDislikeCallback(Example example)
        {
            this.example = example;
        }

        public void OnCancel()
        {
            Debug.Log("CSJM_Unity "+ "Example " + "express banner dislike OnCancel");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "ExpressBannerAdDislikeCallback cancle";
        }

        public void OnShow()
        {
            Debug.Log("CSJM_Unity "+ "Example " + "express banner dislike OnShow");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "ExpressBannerAdDislikeCallback OnShow";
        }

        public void OnSelected(int var1, string var2, bool enforce)
        {
            Debug.Log("CSJM_Unity "+ "Example " + "express banner dislike OnSelected:" + var2);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "ExpressBannerAdDislikeCallback OnSelected";
            //释放广告资源
            if (this.example.mExpressBannerAd != null)
            {
                this.example.mExpressBannerAd.Dispose();
                this.example.mExpressBannerAd = null;
            }
        }
    }

    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.mExpressBannerAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.mExpressBannerAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.mExpressBannerAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.mExpressBannerAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.mExpressBannerAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }
}
