穿山甲官方网站下载的sdk里的带的example,怎么打出apk包，在模拟器或者真机跑起来
环境：
   1. unity3D 2022.3.62  切换Android平台
   2. 夜神或者其他模拟器或者android真机

一些操作：
1.把 Assets\CSJ\Plugins\Android\CSJ.androidlib\build.gradle里的compileSdkVersion和buildToolsVersion改成 2022.3.62 版本unity3D里自带支持的34
    compileSdkVersion 34
    buildToolsVersion '34.0.0'

广告的核心流程，记录   
   1游戏启动初始化时，初始化广告sdk   init，传入appkey   appSecret信息
   2 游戏内逻辑，需要展示广告时，调用load传入代码位id等信息，在load完成回调（还有其他回调）中，可以调用show，展示广告，arrivedReward（奖励下发）回调（还有其他回调）里，处理广告回传的奖励信息，进行奖励的真实下发（复活、道具什么的）。
   3.rewardAd.dispose()，需要在最后调用，清理广告的资源占用。