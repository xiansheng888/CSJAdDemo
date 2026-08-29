using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

/**
 * Draw信息流广告代码示例
 * 注：该接口支持融合Draw信息流，并且支持混出功能，即该接口同时支持模板和自渲染。
 */
public class ExampleDrawFeedAd
{
    public static void LoadDrawFeedAd(Example example)
    {
        if (example.drawFeedAd != null)
        {
            example.drawFeedAd.Dispose();
            example.drawFeedAd = null;
        }
        var adSlot = new AdSlot.Builder()
            .SetCodeId(CSJMDAdPositionId.M_DRAW_ID) // 必传
            .SetExpressViewAcceptedSize(350, 400) //期望模板广告view的size,单位dp
            .SetImageAcceptedSize(1080, 600) //自渲染广告尺寸，单位px
            .SetAdCount(1) //请求广告数量为1条，只支持同一时间显示1条
            .SetMediationAdSlot(
                new MediationAdSlot.Builder()
                    .SetBidNotify(true) // 可选
                    .SetScenarioId("unity-SetScenarioId") // 可选
                    .SetWxAppId("unity-wxAppId") // 可选
                    .SetMuted(true)
                    .SetVolume(0.7f)
                    .Build())
            .Build();
        SDK.CreateAdNative().LoadDrawFeedAd(adSlot, new DrawFeedAdListener(example));
    }

    public static void ShowDrawFeedAd(Example example)
    {
        if (example.drawFeedAd == null)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
            return;
        }
        example.drawFeedAd.SetFeedAdInteractionListener(new FeedAdInteractionListener(example));
        example.drawFeedAd.SetFeedAdDislikeListener(new FeedAdDislikeCallback(example));
        example.drawFeedAd.SetVideoAdListener(new FeedVideoListener());
        example.drawFeedAd.SetDownloadListener(new AppDownloadListener(example));
        example.drawFeedAd.SetAdInteractionListener(new TTAdInteractionListener());
        example.drawFeedAd.ShowFeedAd(0, 500);
    }

    // 广告加载监听器
    public class DrawFeedAdListener : IDrawFeedAdListener
    {

        private Example example;

        public DrawFeedAdListener(Example example)
        {
            this.example = example;
        }
        
        public void OnDrawFeedAdLoad(IList<DrawFeedAd> ads)
        {
            Debug.Log("CSJM_Unity " + "Example " + "drawFeedAd loaded, ad size: " + ads.Count);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnDrawFeedAdLoad";
            if (ads.Count > 0)
            {
                this.example.drawFeedAd = ads[0];
            }
        }

        public void OnError(int code, string message)
        {
            Debug.Log("CSJM_Unity"+ "Example"+ "drawFeed load fail code: " + code + ", msg: " + message);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnDrawFeedAdLoadFail, code: " + code + ", msg: " + message;
        }
    }

    // 广告展示监听器
    public class FeedAdInteractionListener : IFeedAdInteractionListener
    {

        private Example example;
        
        public FeedAdInteractionListener(Example example)
        {
            this.example = example;
        }
        public void OnAdClicked()
        {
            Debug.Log("CSJM_Unity " + "Example " + "draw feedAd ad clicked");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "draw feed ad clicked";
            this.example.drawFeedAd.UploadDislikeEvent("csjm_unity drawFeed dislike test");
        }

        public void OnAdCreativeClick()
        {
            Debug.Log("CSJM_Unity " + "Example " + "draw feedAd ad CreativeClick");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "draw feed ad CreativeClick";
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + "draw feedAd ad show");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "draw feed ad show";

            LogMediationInfo(example);
        }
    }

    // dislike监听器
    public class FeedAdDislikeCallback : IDislikeInteractionListener
    {
        private Example example;

        public FeedAdDislikeCallback(Example example)
        {
            this.example = example;
        }

        public void OnCancel()
        {
            Debug.Log("CSJM_Unity "+ "Example " + "draw feed ad dislike OnCancel");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "draw feed ad dislike OnCancel";
            }
        }

        public void OnShow()
        {
            Debug.Log("CSJM_Unity "+ "Example " + "draw feed ad dislike OnShow");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "draw feed ad OnShow:";
            }
        }

        public void OnSelected(int var1, string var2, bool enforce)
        {
            Debug.Log("CSJM_Unity "+ "Example " + "draw feed ad dislike OnSelected:" + var2);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {     
                this.example.information.text = "draw feed ad OnSelected: " + var2;
            }
        }
    }

    // 视频播放状态监听器
    public class FeedVideoListener : IVideoAdListener
    {
        /// <summary>
        /// Invoke when the video loaded.
        /// </summary>
        public void OnVideoLoad(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity"+ "Example " + "draw feed OnVideoLoad");
        }

        /// <summary>
        /// Invoke when the video error.
        /// </summary>
        public void OnVideoError(int var1, int var2)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feedOnVideoError");
        }

        /// <summary>
        /// Invoke when the video Ad start to play.
        /// </summary>
        public void OnVideoAdStartPlay(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feed OnVideoAdStartPlay");
        }

        /// <summary>
        /// Invoke when the video Ad paused.
        /// </summary>
        public void OnVideoAdPaused(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feed OnVideoAdPaused");
        }

        /// <summary>
        /// Invoke when the video continue to play.
        /// </summary>
        public void OnVideoAdContinuePlay(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feed OnVideoAdContinuePlay");
        }

        public void OnProgressUpdate(long current, long duration)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feed OnProgressUpdate curr: " + current + ", duration: " + duration);
        }

        public void OnVideoAdComplete(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "draw feed OnVideoAdComplete");
        }
    }

    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.drawFeedAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.drawFeedAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.drawFeedAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.drawFeedAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.drawFeedAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }
}
