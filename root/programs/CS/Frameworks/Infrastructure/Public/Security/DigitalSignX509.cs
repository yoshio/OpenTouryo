﻿//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：DigitalSignX509
//* クラス日本語名  ：DigitalSignX509クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2018/11/07  西野 大介         DSA証明書のサポートを追加（4.7以上）
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// DigitalSignX509クラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class DigitalSignX509 : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        #region mem & prop

        /// <summary>
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </summary>
#if NET45
         private string _hashAlgorithmName = "";
#else
        private HashAlgorithmName _hashAlgorithmName = new HashAlgorithmName("SHA256");

        /// <summary>RSASignaturePadding</summary>
        public RSASignaturePadding Padding = RSASignaturePadding.Pkcs1;
#endif
        /// <summary>X.509証明書</summary>
        public X509Certificate2 X509Certificate { get; protected set; }

        /// <summary>X.509証明書の秘密鍵</summary>
        public AsymmetricAlgorithm X509PrivateKey
        {
            get
            {
                return this.X509Certificate.PrivateKey;
            }
        }

        /// <summary>X.509証明書の公開鍵</summary>
        public AsymmetricAlgorithm X509PublicKey
        {
            get
            {
                return this.X509Certificate.PublicKey.Key;
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </param>
        public DigitalSignX509(string certificateFilePath, string password, string hashAlgorithmName) :
            this(certificateFilePath, password, hashAlgorithmName, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public DigitalSignX509(string certificateFilePath, string password, string hashAlgorithmName, X509KeyStorageFlags flag)
        {
            this.X509Certificate = new X509Certificate2(certificateFilePath, password, flag);
#if NET45
            this._hashAlgorithmName = hashAlgorithmName;
#else
            this._hashAlgorithmName = new HashAlgorithmName(hashAlgorithmName);
#endif
        }

#if NET45
#else
        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        public DigitalSignX509(string certificateFilePath, string password, HashAlgorithmName hashAlgorithmName) :
            this(certificateFilePath, password, hashAlgorithmName, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public DigitalSignX509(string certificateFilePath, string password, HashAlgorithmName hashAlgorithmName, X509KeyStorageFlags flag)
        {
            this.X509Certificate = new X509Certificate2(certificateFilePath, password, flag);
            this._hashAlgorithmName = hashAlgorithmName;
        }
#endif

        #endregion

        #endregion

        #region デジタル署名(X509Certificate)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            AsymmetricAlgorithm aa = this.GetPrivateKey();

            // デジタル署名
            byte[] signedByte = null;

            if (aa is RSA)
            {
                // RSA
                // *.pfxの場合、ExportParameters(true)して生成し直している。
                RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory(this.X509Certificate.PrivateKey.KeySize);
                rsa.ImportParameters(((RSA)(aa)).ExportParameters(true));

#if NET45
                if (rsa is RSACryptoServiceProvider)
                {
                    signedByte = ((RSACryptoServiceProvider)rsa).SignData(data, this._hashAlgorithmName);
                }
                // NET45にRSACng、RSAOpenSsl等は無し。
#else
                signedByte = rsa.SignData(data, this._hashAlgorithmName, this.Padding); 
#endif
            }
            else if (aa is DSA)
            {
                // DSA
                // *.pfxの場合、ExportParameters(true)して生成し直している。
                DSA dsa = AsymmetricAlgorithmCmnFunc.DsaFactory(this.X509Certificate.PrivateKey.KeySize);
                dsa.ImportParameters(((DSA)(aa)).ExportParameters(true));

                signedByte = dsa.CreateSignature(data);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }

            return signedByte;
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            AsymmetricAlgorithm aa = null;

            // 検証結果フラグ ( true:検証成功, false:検証失敗 )
            bool flg = false;

            if (this.X509Certificate.PrivateKey == null)
            {
                // *.cer
                aa = this.GetPublicKey();
                if (aa is RSA)
                {
                    // RSA
#if NET45
                    if (aa is RSACryptoServiceProvider)
                    {
                        return ((RSACryptoServiceProvider)aa).VerifyData(data, this._hashAlgorithmName, sign);
                    }
                    // NET45にRSACng、RSAOpenSsl等は無し。
#else
                    return ((RSA)aa).VerifyData(data, sign, this._hashAlgorithmName, this.Padding);
#endif
                }
                else if (aa is DSA)
                {
                    // DSA
                    return ((DSA)aa).VerifySignature(data, sign);
                }
                else if (aa is ECDsaCng)
                {
                    // ECDsaCng (不明)
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
                else
                {
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }
            else
            {
                // *.pfx
                aa = this.GetPrivateKey();
                if (aa is RSA)
                {
                    // RSA
                    // *.pfxの場合、ExportParameters(true)して生成し直している。
                    RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory(this.X509Certificate.PrivateKey.KeySize);
                    rsa.ImportParameters(((RSA)(aa)).ExportParameters(true));

#if NET45
                    if (rsa is RSACryptoServiceProvider)
                    {
                        flg = ((RSACryptoServiceProvider)rsa).VerifyData(data, this._hashAlgorithmName, sign);
                    }
                    // NET45にRSACng、RSAOpenSsl等は無し。
#else
                    flg = rsa.VerifyData(data, sign, this._hashAlgorithmName, this.Padding);
#endif
                }
                else if (aa is DSA)
                {
                    // DSA
                    // *.pfxの場合、ExportParameters(true)して生成し直している。
                    DSA dsa = AsymmetricAlgorithmCmnFunc.DsaFactory(this.X509Certificate.PrivateKey.KeySize);
                    dsa.ImportParameters(((DSA)(aa)).ExportParameters(true));
                    flg = dsa.VerifySignature(data, sign);
                }
                else if (aa is ECDsaCng)
                {
                    // ECDsaCng (不明)
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
                else
                {
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
                }
            }

            return flg;
        }

        #endregion

        #region GetKey(RSA、DSAを選択的に)

        /// <summary>GetPrivateKey</summary>
        /// <returns>AsymmetricAlgorithm</returns>
        private AsymmetricAlgorithm GetPrivateKey()
        {
            AsymmetricAlgorithm aa = null;

#if NET45 || NET46 || NETSTD
            aa = this.X509Certificate.PrivateKey;
#else
            if (this.X509Certificate.PublicKey.Oid.FriendlyName.ToUpper() == "DSA")
            {
                // DSA
                aa = this.X509Certificate.GetDSAPrivateKey();
            }
            else
            {
                // RSA
                aa = this.X509Certificate.PrivateKey;
            }
#endif
            return aa;
        }

        /// <summary>GetPublicKey</summary>
        /// <returns>AsymmetricAlgorithm</returns>
        private AsymmetricAlgorithm GetPublicKey()
        {
            AsymmetricAlgorithm aa = null;

#if NET45 || NET46 || NETSTD
            aa = this.X509Certificate.PublicKey.Key;
#else
            if (this.X509Certificate.PublicKey.Oid.FriendlyName.ToUpper() == "DSA")
            {
                // DSA
                aa = this.X509Certificate.GetDSAPublicKey();
            }
            else
            {
                // RSA
                aa = this.X509Certificate.PublicKey.Key;
            }
#endif
            return aa;
        }

        #endregion

        // こちらは、MyDispose (派生の末端を呼ぶ) の実装は不要。
    }
}