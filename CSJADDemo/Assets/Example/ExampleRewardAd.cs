using System;
using System.Collections.Generic;
using System.Threading;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using UnityEngine;

/**
 * 激励视频代码示例。
 * 注：该接口支持融合功能
 */
public class ExampleRewardAd
{

    // 加载广告
    public static void LoadReward(Example example, bool isM)
    {
        // 释放上一次广告
        if (example.rewardAd != null)
        {
            example.rewardAd.Dispose();
            example.rewardAd = null;
        }

        // 竖屏
        var codeId = isM ? CSJMDAdPositionId.M_REWARD_VIDEO_V_ID : CSJMDAdPositionId.CSJ_REWARD_V_ID;
        // 创造广告参数对象
        var adSlot = new AdSlot.Builder()
            .SetCodeId(codeId) // 必传
            .SetUserID("180590") // 用户id,必传参数
            .SetOrientation(AdOrientation.Vertical) // 必填参数，期望视频的播放方向
            .SetRewardName("银币") // 可选
            .SetRewardAmount(777) // 可选
            .SetMediaExtra("media_extra") //⚠️设置透传信息(穿山甲广告 或 聚合维度iOS广告时)，需可序列化
            .SetMediationAdSlot(
                new MediationAdSlot.Builder()
#if UNITY_ANDROID  //⚠️设置透传信息(当加载聚合维度Android广告时)
                    .SetExtraObject(AdConst.KEY_GROMORE_EXTRA, "gromore-server-reward-extra-unity") // 可选，设置gromore服务端验证的透传参数
                    .SetExtraObject("pangle", "pangleCustomData") // 可选，不是gromore服务端验证时，用于各个adn的参数透传
                    .SetExtraObject(AdConst.KEY_M_SHOW_ADN_LOAD_ERROR_DETAIL, "1") // 可选，获取各个adn加载失败的信息，在加载失败的errMsg中
#endif
                    .SetScenarioId("reward-m-scenarioId") // 可选
                    .SetExtraObject(AdConst.KEY_M_TWO_STAGE_INFO, "{\"two_stage_reward_type\":\"1\",\"two_stage_basic_reward\":\"888\",\"two_stage_basic_unit\":\"元\",\"two_stage_advanced_reward\":\"33333\",\"two_stage_advanced_unit\":\"元\"}")
                    .SetBidNotify(true) // 可选
                    .SetUseSurfaceView(false) // 可选
                    .Build()
                    )
            
            .Build();
        // 加载广告
        SDK.CreateAdNative().LoadRewardVideoAd(adSlot, new RewardVideoAdListener(example));
    }

    // 展示广告
    public static void ShowReward(Example example)
    {
        if (example.rewardAd == null)
        {
            Debug.LogError("CSJM_Unity " + "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
        }
        else
        {
            // 设置展示阶段的监听器
            example.rewardAd.SetRewardAdInteractionListener(new RewardAdInteractionListener(example));
            example.rewardAd.SetDownloadListener(new AppDownloadListener(example));
            example.rewardAd.SetAdInteractionListener(new TTAdInteractionListener());
            example.rewardAd.ShowRewardVideoAd();
        }
    }

    /**
     * 广告加载监听器
     */
    public sealed class RewardVideoAdListener : IRewardVideoAdListener
    {
        private Example example;
        public RewardVideoAdListener(Example example)
        {
            this.example = example;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("CSJM_Unity " + "Example " + $"OnRewardError:{message} on main thread:{Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "OnRewardError: " + message;
            }
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"OnRewardVideoAdLoad on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");

            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "OnRewardVideoAdLoad";
            }
            this.example.rewardAd = ad;
        }

        public void OnRewardVideoCached()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"OnRewardVideoCached on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId) this.example.information.text = "OnRewardVideoCached";
        }

        public void OnRewardVideoCached(RewardVideoAd ad)
        {
            Debug.Log("CSJM_Unity " + "Example " + $"OnRewardVideoCached RewardVideoAd ad on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        }
    }

    // 广告展示监听器
    public sealed class RewardAdInteractionListener : IRewardAdInteractionListener
    {
        private Example example;

        public RewardAdInteractionListener(Example example)
        {
            this.example = example;
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"rewardVideoAd show on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd show";
            }

            LogMediationInfo(example);
        }

        public void OnAdVideoBarClick()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"rewardVideoAd bar click on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd bar click";
            }
        }

        public void OnAdClose()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"rewardVideoAd close on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd close";
            }

            if (this.example.rewardAd != null)
            {
                this.example.rewardAd.Dispose();
                this.example.rewardAd = null;
            }
        }

        public void OnVideoSkip()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"rewardVideoAd skip on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd skip";
            }
        }

        public void OnVideoComplete()
        {
            Debug.Log("CSJM_Unity " + "Example " + "Example " + $"rewardVideoAd complete on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd complete";
            }
        }

        public void OnVideoError()
        {
            Debug.LogError("CSJM_Unity " + "Example " + $"rewardVideoAd error on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "rewardVideoAd error";
            }
        }

        public void OnRewardArrived(bool isRewardValid, int rewardType, IRewardBundleModel extraInfo)
        {
            var logString = "OnRewardArrived verify:" + isRewardValid + " rewardType:" + rewardType + " extraInfo: " + extraInfo.ToString() +
                            $" on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}";
            Debug.Log("CSJM_Unity " + "Example " + logString);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = logString;
            }
        }
    }

    // 打印广告相关信息
    private static void LogMediationInfo(Example example)
    {
        MediationAdEcpmInfo showEcpm = example.rewardAd.GetMediationManager().GetShowEcpm();
        if (showEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(showEcpm, "GetShowEcpm");
        }

        MediationAdEcpmInfo bestEcpm = example.rewardAd.GetMediationManager().GetBestEcpm();
        if (bestEcpm != null)
        {
            LogUtils.LogMediationAdEcpmInfo(bestEcpm, "GetBestEcpm");
        }

        List<MediationAdEcpmInfo> multiBiddingEcpmList = example.rewardAd.GetMediationManager().GetMultiBiddingEcpm();
        foreach (MediationAdEcpmInfo item in multiBiddingEcpmList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetMultiBiddingEcpm");
        }

        List<MediationAdEcpmInfo> cacheList = example.rewardAd.GetMediationManager().GetCacheList();
        foreach (MediationAdEcpmInfo item in cacheList)
        {
            LogUtils.LogMediationAdEcpmInfo(item, "GetCacheList");
        }

        List<MediationAdLoadInfo> adLoadInfoList = example.rewardAd.GetMediationManager().GetAdLoadInfo();
        foreach (MediationAdLoadInfo item in adLoadInfoList)
        {
            LogUtils.LogAdLoadInfo(item);
        }
    }
}
