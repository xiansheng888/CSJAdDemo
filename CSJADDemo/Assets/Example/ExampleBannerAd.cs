using System.Threading;
using ByteDance.Union;
using UnityEngine;

/**
 * 自渲染banner代码示例
 * 注：仅支持穿山甲代码位，不支持融合
 */
public class ExampleBannerAd
{
    public static void LoadNativeBannerAd(Example example)
    {
        if (example.bannerAd != null)
        {
            example.bannerAd.Dispose();
            example.bannerAd = null;
        }

        int width = UnityEngine.Screen.width;
        int height = width / 600 * 257;

        var adSlot = new AdSlot.Builder()
            .SetCodeId(CSJMDAdPositionId.CSJ_NATIVE_BANNER_ID) // 必传
            .SetImageAcceptedSize(width, height) // 单位px
            .SetNativeAdType(AdSlotType.Banner) // 仅支持banner
            .Build();
        // LoadNativeAd接口仅支持自渲染Banner
        SDK.CreateAdNative().LoadNativeAd(adSlot, new NativeBannerAdListener(example));
    }

    public static void ShowNativeBannerAd(Example example)
    {
        if (example.bannerAd == null)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "请先加载广告");
            example.information.text = "请先加载广告";
            return;
        }
        
        example.bannerAd.SetNativeAdInteractionListener(new NativeBannerAdInteractionListener(example));
        example.bannerAd.SetNativeAdDislikeListener(new NativeBannerAdDislikeCallback(example));
        example.bannerAd.SetDownloadListener(new AppDownloadListener(example));
        example.bannerAd.SetAdInteractionListener(new TTAdInteractionListener());
        example.bannerAd.ShowNativeAd(AdSlotType.Banner, 0, 500); // ShowNativeAd仅支持自渲染Banner
    }

    // 广告加载监听器
    public sealed class NativeBannerAdListener : INativeAdListener
    {
        private Example example;
        public NativeBannerAdListener(Example example)
        {
            this.example = example;
        }

        public void OnError(int code, string message)
        {
            Debug.LogError("CSJM_Unity "+ "Example " + "OnNativeBannerAdError: " + message);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "OnNativeBannerAdError: " + message;
            }
        }

        public void OnNativeAdLoad(NativeAd[] ads)
        {
            if (ads == null || ads.Length <= 0)
            {
                Debug.Log("CSJM_Unity "+ "Example " + $"OnNativeBannerAdLoad ads array is null on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
                if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
                {
                    this.example.information.text = "OnNativeBannerAdLoad ads array is null ";
                }

                return;
            }
            this.example.bannerAd = ads[0];

            Debug.Log("CSJM_Unity "+ "Example " + $"OnNativeAdLoad on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "OnNativeAdLoad";
            }
        }
    }

    public sealed class NativeBannerAdInteractionListener : IInteractionAdInteractionListener
    {
        private Example example;

        public NativeBannerAdInteractionListener(Example example)
        {
            this.example = example;
        }

        public void OnAdCreativeClick()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"NativeAd creative click on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "NativeAd creative click";
            }
        }

        public void OnAdShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"NativeAd show on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "NativeAd show";
            }
        }

        public void OnAdClicked()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"NativeAd click  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "NativeAd click";
            }
            this.example.bannerAd.UploadDislikeEvent("csjm_unity nativeBanner dislike test");
        }

        public void OnAdDismiss()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"NativeAd close  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "NativeAd close";
            }

            //释放广告资源
            example.bannerAd?.Dispose();
        }

        public void onAdRemoved()
        {
            Debug.Log("CSJM_Unity " + "Example " + $"NativeAd onAdRemoved  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "NativeAd onAdRemoved";
            }
        }
    }

    public class NativeBannerAdDislikeCallback : IDislikeInteractionListener
    {
        private Example example;

        public NativeBannerAdDislikeCallback(Example example)
        {
            this.example = example;
        }

        public void OnCancel()
        {
            Debug.Log("CSJM_Unity " + "Example " + "native banner ad dislike OnCancel");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "native banner ad dislike OnCancel";
            }
        }

        public void OnShow()
        {
            Debug.Log("CSJM_Unity " + "Example " + "native banner ad dislike OnShow");
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "native banner ad OnShow:";
            }
        }

        public void OnSelected(int var1, string var2, bool enforce)
        {
            Debug.Log("CSJM_Unity " + "Example " + "native banner ad dislike OnSelected:" + var2);
            if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            {
                this.example.information.text = "native banner ad OnSelected:" + var2;
            }
        }
    }
}