﻿//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：sampleScreen  
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MyType;

using System;
using System.Data;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Public.Db;

namespace ProjectX_sample.Aspx.sample.Crud
{
    /// <summary>サンプル アプリ画面</summary>
    public partial class sampleScreen : MyBaseController
    {
        #region ページロードのUOCメソッド

        /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:
            this.ddlIso.SelectedIndex = 1;
        }

        /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            // TODO:
        }

        #endregion

        #region ＣＲＵＤ処理メソッド

        #region 参照系

        /// <summary>
        /// btnMButton1のクリックイベント（件数取得）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton1_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectCount",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                label.Text = testReturnValue.Obj.ToString() + "件のデータがあります";
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton2のクリックイベント（一覧取得（dt））
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton2_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DT",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.GridView1.DataSource = testReturnValue.Obj;
                this.GridView1.DataBind();
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton3のクリックイベント（一覧取得（ds））
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton3_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DS",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                DataSet ds = (DataSet)testReturnValue.Obj;
                this.GridView1.DataSource = ds.Tables[0];
                this.GridView1.DataBind();
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton4のクリックイベント（一覧取得（dr））
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton4_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DR",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.GridView1.DataSource = testReturnValue.Obj;
                this.GridView1.DataBind();
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton5のクリックイベント（一覧取得（動的sql））
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton5_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DSQL",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 動的SQLの要素を設定
            testParameterValue.OrderColumn = this.ddlOrderColumn.SelectedValue;
            testParameterValue.OrderSequence = this.ddlOrderSequence.SelectedValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.GridView1.DataSource = testReturnValue.Obj;
                this.GridView1.DataBind();
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton6のクリックイベント（参照処理）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton6_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Select",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.TextBox1.Text);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.TextBox1.Text = testReturnValue.ShipperID.ToString();
                this.TextBox2.Text = testReturnValue.CompanyName;
                this.TextBox3.Text = testReturnValue.Phone;
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region 更新系

        /// <summary>
        /// btnMButton7のクリックイベント（追加処理）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton7_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Insert",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 情報の設定
            testParameterValue.CompanyName = this.TextBox2.Text;
            testParameterValue.Phone = this.TextBox3.Text;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                label.Text = testReturnValue.Obj.ToString() + "件追加";
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton8のクリックイベント（更新処理）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton8_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Update",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.TextBox1.Text);
            testParameterValue.CompanyName = this.TextBox2.Text;
            testParameterValue.Phone = this.TextBox3.Text;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                label.Text = testReturnValue.Obj.ToString() + "件更新";
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// btnMButton9のクリックイベント（削除処理）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMButton9_Click(FxEventArgs fxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Delete",
                    this.ddlDap.SelectedValue + "%"
                    + this.ddlMode1.SelectedValue + "%"
                    + this.ddlMode2.SelectedValue + "%"
                    + this.ddlExRollback.SelectedValue,
                    this.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(TextBox1.Text);

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            TestReturnValue testReturnValue =
                (TestReturnValue)myBusiness.DoBusinessLogic(
                    (BaseParameterValue)testParameterValue, iso);

            // 結果表示するメッセージ エリア
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                label.Text = testReturnValue.Obj.ToString() + "件削除";
            }

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion

        #region Ｐ層で例外をスロー

        /// <summary>
        /// btnButton1のクリックイベント（業務例外）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            throw new BusinessApplicationException(
                "Ｐ層で「業務例外」をスロー",
                "Ｐ層で「業務例外」をスロー",
                "Ｐ層で「業務例外」をスロー");
        }

        /// <summary>
        /// btnButton2のクリックイベント（システム例外）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            throw new BusinessSystemException(
                "Ｐ層で「システム例外」をスロー",
                "Ｐ層で「システム例外」をスロー");
        }

        /// <summary>
        /// btnButton3のクリックイベント（その他、一般的な例外）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            throw new Exception("Ｐ層で「その他、一般的な例外」をスロー");
        }

        /// <summary>
        /// btnButton4のクリックイベント（その他、一般的な例外）
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton4_Click(FxEventArgs fxEventArgs)
        {
            this.GridView1.DataSource = null;
            this.GridView1.DataBind();

            return "";
        }

        #endregion

        #region 分離レベルの設定メソッド

        /// <summary>分離レベルの設定</summary>
        private DbEnum.IsolationLevelEnum SelectIsolationLevel()
        {
            if (this.ddlIso.SelectedValue == "NC")
            {
                return DbEnum.IsolationLevelEnum.NotConnect;
            }
            else if (this.ddlIso.SelectedValue == "NT")
            {
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
            else if (this.ddlIso.SelectedValue == "RU")
            {
                return DbEnum.IsolationLevelEnum.ReadUncommitted;
            }
            else if (this.ddlIso.SelectedValue == "RC")
            {
                return DbEnum.IsolationLevelEnum.ReadCommitted;
            }
            else if (this.ddlIso.SelectedValue == "RR")
            {
                return DbEnum.IsolationLevelEnum.RepeatableRead;
            }
            else if (this.ddlIso.SelectedValue == "SZ")
            {
                return DbEnum.IsolationLevelEnum.Serializable;
            }
            else if (this.ddlIso.SelectedValue == "SS")
            {
                return DbEnum.IsolationLevelEnum.Snapshot;
            }
            else if (this.ddlIso.SelectedValue == "DF")
            {
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
            else
            {
                throw new Exception("分離レベルの設定がおかしい");
            }
        }

        #endregion

        #region マスタページ、ユーザコントロールのイベント

        /// <summary>マスタページにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleScreen_btnMPButton_Click(FxEventArgs fxEventArgs)
        {
            this.lblResult.Text = "sampleScreen.masterのbtnMPButtonのClickイベントを、UOC_sampleScreen_btnMPButton_Clickで実行";

            return "";
        }

        /// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleControl1_btnUCButton_Click(FxEventArgs fxEventArgs)
        {
            this.lblResult.Text = "sampleControl.ascxのbtnUCButtonのClickイベントを、UOC_sampleControl1_btnUCButton_Clickで実行";

            return "";
        }

        /// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_sampleControl2_btnUCButton_Click(FxEventArgs fxEventArgs)
        {
            this.lblResult.Text = "sampleControl.ascxのbtnUCButtonのClickイベントを、UOC_sampleControl2_btnUCButton_Clickで実行";

            return "";
        }

        #endregion
    } 
}
