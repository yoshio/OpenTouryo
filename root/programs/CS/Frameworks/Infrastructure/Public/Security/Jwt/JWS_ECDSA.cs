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
//* クラス名        ：JWS_ECDSA
//* クラス日本語名  ：JWS ESnnn生成クラス
//*
//*                  RFC 7515 - JSON Web Signature (JWS)
//*                  > A.3.  Example JWS Using ECDSA P-256 SHA-256
//*                  https://tools.ietf.org/html/rfc7515#appendix-A.3
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

using System;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>
    /// JWS ESnnn生成クラス
    /// - ES256 | ECDSA using P-256 and SHA-256
    /// - ES384 | ECDSA using P-384 and SHA-384
    /// - ES512 | ECDSA using P-521 and SHA-512
    /// </summary>
    public abstract class JWS_ECDSA : JWS
    {
        /// <summary>ES</summary>
        public enum ES : int
        {
            /// <summary>256</summary>
            _256,

            /// <summary>384</summary>
            _384,

            /// <summary>512</summary>
            _512
        }

        #region mem & prop & constructor

        /// <summary>JwtConst.ESnnn</summary>
        protected string JwtConstESnnn = "";

        /// <summary>_JWSHeader</summary>
        private JWS_Header _JWSHeader = null;

        /// <summary>JWSHeader</summary>
        public JWS_Header JWSHeader
        {
            protected set
            {
                this._JWSHeader = value;
            }

            get
            {
                return this._JWSHeader;
            }
        }

        /// <summary>Init</summary>
        /// <param name="jwtConstESnnn">string</param>
        public void Init(string jwtConstESnnn)
        {
            this.JwtConstESnnn = jwtConstESnnn;
            this._JWSHeader = new JWS_Header() { alg = jwtConstESnnn };
        }

        #endregion

        #region ESnnn署名・検証

        /// <summary>ESnnnのJWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWSHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // ペイロード
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            string payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 署名
            byte[] temp = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);
            string signEncoded = CustomEncode.ToBase64UrlString(this.Create2(temp)); // 派生を呼ぶ

            return headerEncoded + "." + payloadEncoded + "." + signEncoded;
        }

        /// <summary>ESnnnのJWS検証メソッド</summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWS_Header headerObject = (JWS_Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWS_Header));

            if (headerObject.alg.ToUpper() == this.JwtConstESnnn
                && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);
                return this.Verify2(data, sign); // 派生を呼ぶ
            }
            else
            {
                return false;
            }
        }

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected abstract byte[] Create2(byte[] data);

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected abstract bool Verify2(byte[] data, byte[] sign);

        #endregion
    }
}
