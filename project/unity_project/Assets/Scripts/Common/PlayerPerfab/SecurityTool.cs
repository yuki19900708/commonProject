using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace GOLib.Tool
{
    /// <summary>
    /// 加密解密工具类，简单对抗常见内存修改器
    /// </summary>
    public class SecurityTool
    {
        private static SecurityTool instance;
        private static string DES_KEY = "GOTECH78";
        private static string DES_IV = "WUJICIKE";
        private int offset;

        private static void CheckInstance()
        {
            if (instance == null)
            {
                instance = new SecurityTool();
                instance.offset = Random.Range(0, 100000);
            }
        }

        /// <summary>
        /// 加密int数值
        /// </summary>
        /// <param name="source">原数值</param>
        /// <returns>加密后的数值</returns>
        public static int EncryptInt(int source)
        {
            CheckInstance();
            return source ^ instance.offset;
        }

        /// <summary>
        /// 解密int数值
        /// </summary>
        /// <param name="result">加密后的数值</param>
        /// <returns>原数值</returns>
        public static int DecryptInt(int result)
        {
            CheckInstance();
            return result ^ instance.offset;
        }

        /// <summary>
        /// 加密float数值
        /// </summary>
        /// <param name="source">原数值</param>
        /// <returns>加密后的数值</returns>
        public static float EncryptFloat(float source)
        {
            CheckInstance();
            return source + instance.offset;
        }

        /// <summary>
        /// 解密float数值
        /// </summary>
        /// <param name="result">加密后的数值</param>
        /// <returns>原数值</returns>
        public static float DecryptFloat(float result)
        {
            CheckInstance();
            return result - instance.offset;
        }

        /// <summary>
        /// 加密double数值
        /// </summary>
        /// <param name="source">原数值</param>
        /// <returns>加密后的数值</returns>
        public static double EncryptDouble(double source)
        {
            CheckInstance();
            return source + instance.offset;
        }
        
        /// <summary>
        /// 解密double数值
        /// </summary>
        /// <param name="result">加密后的数值</param>
        /// <returns>原数值</returns>
        public static double DecryptDouble(double result)
        {
            CheckInstance();
            return result - instance.offset;
        }
        
        /// <summary>
        /// 加密string
        /// </summary>
        /// <param name="stringToEncrypt">原string</param>
        /// <returns>加密后的string</returns>
        public static string EncryptString(string stringToEncrypt)
        {
            CheckInstance();
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                des.Key = Encoding.UTF8.GetBytes(DES_KEY);
                des.IV = Encoding.UTF8.GetBytes(DES_IV);
                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = System.Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        /// <summary>
        /// 解密string
        /// </summary>
        /// <param name="stringToDecrypt">加密后的string</param>
        /// <returns>原string</returns>
        public static string DecryptString(string stringToDecrypt)
        {
            CheckInstance();
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray = System.Convert.FromBase64String(stringToDecrypt);
                des.Key = Encoding.UTF8.GetBytes(DES_KEY);
                des.IV = Encoding.UTF8.GetBytes(DES_IV);
                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }
    }
}