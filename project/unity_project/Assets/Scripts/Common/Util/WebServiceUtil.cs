using UnityEngine;
using System;
using System.Net;
using System.IO;

public class WebServiceUtil
{
    public delegate void WebServiceCallback(bool isSucceed, string result);

    public class AsyncParams
    {
        public HttpWebRequest request;
        public WebServiceCallback callback;

        public AsyncParams(HttpWebRequest request, WebServiceCallback callback)
        {
            this.request = request;
            this.callback = callback;
        }
    }

    /// <summary>
    /// 使用HttpWebRequest进行同步请求
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="callback">Callback.</param>
    public static void HttpWebRequest(string url, WebServiceCallback callback)
    {
//        try
//        {
//            HttpWebRequest request = new HttpWebRequest(new Uri(url));
//            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
//            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
//            {
//                string result = reader.ReadToEnd();
//                if (callback != null)
//                {
//                    callback(response.StatusCode == HttpStatusCode.OK, result);
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            if (callback != null)
//            {
//                callback(false, e.Message);
//            }
//        }
    }

    /// <summary>
    /// 使用HttpWebRequest进行异步请求
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="callback">Callback.</param>
    public static void HttpWebRequestAsync(string url, WebServiceCallback callback)
    {
//        try
//        {
//            HttpWebRequest request = new HttpWebRequest(new Uri(url));
//            AsyncParams asyncParam = new AsyncParams(request, callback);
//            request.BeginGetResponse(new AsyncCallback(HttpWebRequestAsyncCallback), asyncParam);
//        }
//        catch (Exception e)
//        {
//            if (callback != null)
//            {
//                callback(false, e.Message);
//            }
//        }
    }

    /// <summary>
    /// HttpWebRequest异步请求回调
    /// </summary>
    /// <param name="asyncResult">Async result.</param>
    static void HttpWebRequestAsyncCallback(IAsyncResult asyncResult)
    {
//        AsyncParams asyncParams = asyncResult.AsyncState as AsyncParams;
//        try
//        {
//            HttpWebResponse response = asyncParams.request.EndGetResponse(asyncResult) as HttpWebResponse;
//            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
//            {
//                string result = reader.ReadToEnd();
//                ThreadUtil.QueueActionToMainThread(() =>
//                {
//                    if (asyncParams.callback != null)
//                    {
//                        asyncParams.callback(response.StatusCode == HttpStatusCode.OK, result);
//                    }
//                });
//            }
//        }
//        catch (Exception e)
//        {
//            if (asyncParams.callback != null)
//            {
//                asyncParams.callback(false, e.Message);
//            }
//        }
    }
}
