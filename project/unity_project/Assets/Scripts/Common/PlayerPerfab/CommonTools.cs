using System;
using UnityEngine;
using System.IO;

public class CommonTools
{
	/// <summary>
	/// Md5s the sum.
	/// </summary>
	/// <returns>The sum.</returns>
	/// <param name="strToEncrypt">String to encrypt.</param>
	public static string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}

	private static string deviceUID = null;
	/// <summary>
	/// 获取设备唯一标识。iOS的是取IDFA，注意送审时的勾选。
	/// </summary>
	/// <returns>The device user interface.</returns>
	public static string GetDeviceUID(){
		if (deviceUID == null) {
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				//deviceUID = iOSInterfaces.CallGetDeviceUID();  //TODO:需要接入SDK的方法
			}
			else
			{
				deviceUID = SystemInfo.deviceUniqueIdentifier;
			}
		}
		return deviceUID;
	}
}
