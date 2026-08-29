using System.Threading;
using ByteDance.Union;
using UnityEngine;


public sealed class AppDownloadListener : IAppDownloadListener
{
    private Example example;

    public AppDownloadListener(Example example)
    {
        this.example = example;
    }

    public void OnIdle()
    {
        Debug.Log("CSJM_Unity "+ "Example " + $"OnIdle 下载未开始 on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "OnIdle，下载未开始";
    }

    public void OnDownloadActive(
        long totalBytes, long currBytes, string fileName, string appName)
    {
        Debug.Log("CSJM_Unity "+ "Example " + $"OnDownloadActive 下载中，点击下载区域暂停  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "下载中，点击下载区域暂停";
    }

    public void OnDownloadPaused(
        long totalBytes, long currBytes, string fileName, string appName)
    {
        Debug.Log("CSJM_Unity "+ "Example " + $"OnDownloadPaused 下载暂停，点击下载区域继续  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId} ");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "下载暂停，点击下载区域继续";
    }

    public void OnDownloadFailed(
        long totalBytes, long currBytes, string fileName, string appName)
    {
        Debug.LogError("CSJM_Unity "+ "Example " + $"OnDownloadFailed 下载失败，点击下载区域重新下载  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "下载失败，点击下载区域重新下载";
    }

    public void OnDownloadFinished(
        long totalBytes, string fileName, string appName)
    {
        Debug.Log("CSJM_Unity "+ "Example " + $"OnDownloadFinished 下载完成  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "下载完成";
    }

    public void OnInstalled(string fileName, string appName)
    {
        Debug.Log("CSJM_Unity "+ "Example " + $"OnInstalled 安装完成，点击下载区域打开  on main thread: {Thread.CurrentThread.ManagedThreadId == Example.MainThreadId}");
        if (Thread.CurrentThread.ManagedThreadId == Example.MainThreadId)
            this.example.information.text = "安装完成，点击下载区域打开";
    }
}

