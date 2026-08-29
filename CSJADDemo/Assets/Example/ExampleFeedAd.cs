using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

/**
 * 信息流广告代码示例
 * 注：该接口支持融合信息流，并且支持混出功能，即该接口同时支持信息流模板和自渲染。
 * 也支持直接加载csj信息流自渲染代码位。
 */
public class ExampleFeedAd
{
    public static void LoadFeedAd(Example example, bool isM)
    {
        if (example.feedAd != null)
        {
            example.feedAd.Dispose();
            example.feedAd = null;
        }
        var adSlot = new AdSlot.Builder()
            .SetCodeId(isM ? CSJMDAdPositionId.M_NATIVE_NORMAL_ID : CSJMDAdPositionId.CSJ_NATIVE_ID) // 必传
            .SetExpressViewAcceptedSize(350, 400)//期望模板广告view的size,单位dp，高度设置为0,则高度会自适应
            .SetImageAcceptedSize(1080, 600) // 自渲染广告尺寸，单位px
            .SetAdCount(1) //请求广告数量为1条，只支持同一时间显示1条
            .SetMediationAdSlot(
                new MediationAdSlot.Builder()
                    .SetBidNotify(true) // 可选
                    .SetScenarioId("unity-SetScenarioId") // 可选
                    .SetWxAppId("unity-wxAppId") // 可选
                    .SetMuted(true)
                    .SetVolume(0.7f)
                    .SetShakeViewSize(90.0f, 90.0f) // 可选，百度自渲染信息流的摇一摇功能，设置摇一摇图标的大小，单位dp
                    .SetExtraObject(AdConst.KEY_M_MEDIA_AD_ROTATE_VIEW_ENABLE, "0")
                    .SetExtraObject(AdConst.KEY_M_AUTO_PLAY_POLICY, "2")
                    .Build())
            .Build();
        SDK.CreateAdNative().LoadFeedAd(adSlot, new FeedAdListener(example));
    }

    public static void ShowFeedAd(Example example)
    {
        if (example.feedAd == null)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
            return;
        }
        example.feedAd.SetFeedAdInteractionListener(new FeedAdInteractionListener(example));
        example.feedAd.SetFeedAdDislikeListener(new FeedAdDislikeCallback(example));
        example.feedAd.SetVideoAdListener(new FeedVideoListener());
        example.feedAd.SetDownloadListener(new AppDownloadListener(example));
        example.feedAd.SetAdInteractionListener(new TTAdInteractionListener());
        example.feedAd.ShowFeedAd(0, 500);
    }

    // 广告加载监听器
    public class FeedAdListener : IFeedAdListener
    {

        private Example example;

        public FeedAdListener(Example example)
        {
            this.example = example;
        }
        
        public void OnFeedAdLoad(IList<FeedAd> ads)
        {
            Debug.Log("CSJM_Unity " + "Example " + "feedAd loaded, ad size: " + ads.Count);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnFeedAdLoad";
            if (ads.Count > 0)
            {
                this.example.feedAd = ads[0];
                this.example.feedAd.GetMediationManager().SetShakeViewListener(new MyMediationShakeViewListener());
            }
        }

        public void OnError(int code, string message)
        {
            Debug.Log("CSJM_Unity" + "Example " + "feed load fail code: " + code + ", msg: " + message);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "OnFeedAdLoadFail, code: " + code + ", msg: " + message;
        }
        
    }

    // 百度自渲染信息流摇一摇功能，摇一摇view消失时回调
    public class MyMediationShakeViewListener : MediationShakeViewListener
    {
        public void OnDismissed()
        {
            Debug.Log("CSJM_Unity" + "Example" + ": baidu feed shakeView onDismissed");
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
            Debug.Log("CSJM_Unity " + "Example " + "feedAd ad clicked");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "feed ad clicked";
            this.example.feedAd.UploadDislikeEvent("csjm_unity feed dislike test");
        }

        public void OnAdCreativeClick()
        {
            Debug.Log("CSJM_Unity " + "Example " + "feedAd ad CreativeClick");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "feed ad CreativeClick";
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + "feedAd ad show");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                this.example.information.text = "feed ad show";
            
            // log
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
            Debug.Log("CSJM_Unity "+ "Example " + "feed ad dislike OnCancel");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "feed ad dislike OnCancel";
            }
        }

        public void OnShow()
        {
            Debug.Log("CSJM_Unity "+ "Example " + "feed ad dislike OnShow");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "feed ad OnShow:";
            }
        }

        public void OnSelected(int var1, string var2, bool enforce)
        {
            Debug.Log("CSJM_Unity "+ "Example " + "feed ad dislike OnSelected:" + var2);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {     
                this.example.information.text = "feed ad OnSelected: " + var2;
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
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoLoad");
        }

        /// <summary>
        /// Invoke when the video error.
        /// </summary>
        public void OnVideoError(int var1, int var2)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoError");
        }

        /// <summary>
        /// Invoke when the video Ad start to play.
        /// </summary>
        public void OnVideoAdStartPlay(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoAdStartPlay");
        }

        /// <summary>
        /// Invoke when the video Ad paused.
        /// </summary>
        public void OnVideoAdPaused(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoAdPaused");
        }

        /// <summary>
        /// Invoke when the video continue to play.
        /// </summary>
        public void OnVideoAdContinuePlay(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoAdContinuePlay");
        }

        public void OnProgressUpdate(long current, long duration)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnProgressUpdate curr: " + current + ", duration: " + duration);
        }

        public void OnVideoAdComplete(FeedAd feedAd)
        {
            Debug.Log("CSJM_Unity" + "Example " + "OnVideoAdComplete");
        }
    }
    
    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.feedAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.feedAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.feedAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.feedAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.feedAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }
}