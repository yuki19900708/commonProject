using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Text;

public class Main : MonoBehaviour
{

    public Sprite imgae;
    string url = "http://192.168.81.117:8080/Upload.php";
    string CreateURL = "http://192.168.81.117:8080/CreatFolder.php";
    string DelURL = "http://192.168.81.117:8080/DelFolder.php";

    void Start()
    {
        //StartCoroutine(CreatFolder("Photos"));//在服务器上创建文件夹
        //StartCoroutine(DelFolder("Photos"));//在服务器上删除文件夹
        StartCoroutine(Upload());//上传图片到服务器指定的文件夹
    }

    //创建文件夹
    IEnumerator CreatFolder(string FolderName)
    {
        WWWForm wForm = new WWWForm();
        wForm.AddField("FolderName", FolderName);
        WWW w = new WWW(CreateURL, wForm);
        yield return w;
        if (w.isDone)
        {
            Debug.Log("创建文件夹完成");
        }
    }

    //删除文件夹
    IEnumerator DelFolder(string FolderName)
    {
        WWWForm wForm = new WWWForm();
        wForm.AddField("FolderName", FolderName);
        WWW w = new WWW(DelURL, wForm);
        yield return w;
        if (w.isDone)
        {
            Debug.Log("删除文件夹完成");
        }
    }

    //上传图片到指定的文件夹
    IEnumerator Upload()
    {
        byte[] bytes = SpriteToBytes(imgae);//获取图片数据
        WWWForm form = new WWWForm();//创建提交数据表单
        form.AddField("folder", "Photos/");//定义表单字段用来定义文件夹
        form.AddBinaryData("file", bytes, "11.png", "image/png");//字段名，文件数据，文件名，文件类型
        WWW w = new WWW(url, form);
        yield return w;
        if (w.isDone)
        {
            Debug.Log("上传完成");
        }
    }

    //获取图片的二进制数据
    public byte[] SpriteToBytes(Sprite sp)
    {
        Texture2D t = sp.texture;
        byte[] bytes = t.EncodeToJPG();
        return bytes;
    }
}

