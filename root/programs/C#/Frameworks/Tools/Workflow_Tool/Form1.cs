﻿//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：Form1
//* クラス日本語名  ：Workflowシミュレータツール
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/07/23  西野  大介        新規作成
//*
//**********************************************************************************

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Workflow;

//using Touryo.Infrastructure.Business.RichClient.Asynchronous;
//using Touryo.Infrastructure.Business.RichClient.Presentation;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

//using Touryo.Infrastructure.Framework.RichClient.Presentation;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using System.Resources;

namespace Workflow_Tool
{
    /// <summary>Workflowシミュレータツール</summary>
    public partial class Form1 : Form
    {
        /// <summary>Dam</summary>
        BaseDam _dam = null;

        #region 初期化処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>ロード</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 現状SQLServerのみ対応
            this.txtCnnstr.Text = GetConfigParameter.GetConnectionString("ConnectionString_SQL");

            this.txtSubSystemId.Text = GetConfigParameter.GetConfigValue("SubSystemId");
            this.txtWorkflowName.Text = GetConfigParameter.GetConfigValue("WorkflowName");

            // SQLは埋め込まれたリソースを使用する。
            Touryo.Infrastructure.Business.Dao.MyBaseDao.UseEmbeddedResource = true;

            // タブ
            this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
            this.tabControl1.SelectedTab = this.tabControl1.TabPages[0];
        }

        #endregion

        #region ボタン
        
        /// <summary>新しいワークフローを準備します。</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = null;
                DataTable dt2 = null;

                // PreStartWorkflow
                Workflow wf = new Workflow(this._dam);

                // 御中ID
                if (!string.IsNullOrEmpty(this.txtDearSirUID.Text))
                {
                    dt1 = wf.PreStartWorkflow(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text, decimal.Parse(this.txtDearSirUID.Text));
                }
                // 個人ID
                if (!string.IsNullOrEmpty(this.txtUserID.Text))
                {
                    dt2 = wf.PreStartWorkflow(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text, decimal.Parse(this.txtUserID.Text));
                }

                //  開始可能なワークフローを表示
                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                this.dataGridView1.DataSource = dt;

                this._dam.CommitTransaction();
            }
            catch(Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>新しいワークフローを開始します。</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                // 新規ワークフローを取得
                DataRow startWorkflow = null; 
                DataTable dt = (DataTable)this.dataGridView1.DataSource;

                DataGridViewRow dgvr = (this.dataGridView1.SelectedRows[0]); 
                foreach (DataRow dr in dt.Rows)
                {
                    if ((decimal)dr[0] == (decimal)dgvr.Cells[0].Value)
                    {
                        startWorkflow = dr;
                    }
                }

                // ワークフロー管理番号の生成
                if (string.IsNullOrEmpty(txtWorkflowControlNo.Text))
                {
                    txtWorkflowControlNo.Text = Guid.NewGuid().ToString();
                }

                // UserInfoを取得
                decimal fromUserID = decimal.Parse(txtUserID.Text);
                string fromUserInfo = this.GetUserInfo(fromUserID);
                string toUserInfo = this.GetUserInfo((decimal)startWorkflow["ToUserId"]);

                // StartWorkflow
                Workflow wf = new Workflow(this._dam);

                int mailTemplateId = 0;
                mailTemplateId = wf.StartWorkflow(
                    startWorkflow, txtWorkflowControlNo.Text, fromUserID, fromUserInfo, toUserInfo,
                    this.txtWorkflowReserveArea.Text, this.txtCurrentWorkflowReserveArea1.Text, this.dtpReplyDeadline1.Value);
                
                // ★ ココでメールを送信。

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>ワークフロー依頼を取得します。</summary>
        private void button3_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                DataTable dt = new DataTable();
                DataTable dt1 = null;
                DataTable dt2 = null;

                // GetWfRequest
                Workflow wf = new Workflow(this._dam);

                // 御中ID
                if (!string.IsNullOrEmpty(this.txtDearSirUID.Text)
                    && !string.IsNullOrEmpty(this.txtDearSirPTitleId.Text))
                {
                    dt1 = wf.GetWfRequest(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text,
                        decimal.Parse(this.txtDearSirUID.Text),
                        int.Parse(this.txtDearSirPTitleId.Text));
                }
                // 個人ID
                if (!string.IsNullOrEmpty(this.txtUserID.Text))
                {
                    dt2 = wf.GetWfRequest(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text,
                        decimal.Parse(this.txtUserID.Text), null);
                }

                //  開始可能なワークフローを表示
                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                this.dataGridView1.DataSource = dt;

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>ワークフロー依頼を受付ます。</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                // ワークフロー依頼を取得
                DataRow workflowRequest = null;
                DataTable dt = (DataTable)this.dataGridView1.DataSource;

                DataGridViewRow dgvr = (this.dataGridView1.SelectedRows[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    if ((string)dr[0] == (string)dgvr.Cells[0].Value)
                    {
                        workflowRequest = dr;
                    }
                }

                // UserInfoを取得
                decimal userId =  decimal.Parse(txtUserID.Text);
                string userInfo = this.GetUserInfo(userId);

                // AcceptWfRequest
                Workflow wf = new Workflow(this._dam);
                wf.AcceptWfRequest(workflowRequest, userId, userInfo);

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>処理中ワークフロー依頼を取得します。</summary>
        private void button5_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                // GetProcessingWfRequest
                Workflow wf = new Workflow(this._dam);
                this.dataGridView1.DataSource = wf.GetProcessingWfRequest(
                    this.txtSubSystemId.Text, this.txtWorkflowName.Text, decimal.Parse(this.txtUserID.Text));

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>次のワークフロー依頼を取得します。</summary>
        private void button6_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                DataTable dt = null;
                DataTable dt1 = null;
                DataTable dt2 = null;

                // 処理中ワークフロー依頼を取得
                DataRow processingWfReq = null;
                dt = (DataTable)this.dataGridView1.DataSource;

                DataGridViewRow dgvr = (this.dataGridView1.SelectedRows[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    if ((string)dr[0] == (string)dgvr.Cells[0].Value)
                    {
                        processingWfReq = dr;
                    }
                }

                // GetNextWfRequest
                Workflow wf = new Workflow(this._dam);
                dt1 = wf.GetNextWfRequest(processingWfReq, decimal.Parse(this.txtDearSirUID.Text));
                dt2 = wf.GetNextWfRequest(processingWfReq, decimal.Parse(this.txtUserID.Text));

                //  次のワークフロー依頼を表示
                dt = new DataTable();
                
                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                this.dataGridView1.DataSource = dt;

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>ワークフロー承認を依頼します。</summary>
        private void button7_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                // ワークフロー承認依頼を取得
                DataRow nextWorkflow = null;
                DataTable dt = (DataTable)this.dataGridView1.DataSource;

                DataGridViewRow dgvr = (this.dataGridView1.SelectedRows[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    if ((decimal)dr[0] == (decimal)dgvr.Cells[0].Value)
                    {
                        nextWorkflow = dr;
                    }
                }

                // GetTurnBackToUser, StartWorkflow
                Workflow wf = new Workflow(this._dam);

                // UserInfoを取得
                decimal fromUserID = decimal.Parse(this.txtUserID.Text);
                string fromUserInfo = this.GetUserInfo(decimal.Parse(txtUserID.Text));

                decimal toUserID = 0;
                string toUserInfo = "";
                if ((string)nextWorkflow["ActionType"] == "End")
                {
                    // End
                    toUserID = 0;
                    toUserInfo = "";
                }
                else if ((string)nextWorkflow["ActionType"] == "TurnBack")
                {
                    // TurnBack
                    toUserID = wf.GetTurnBackToUser(nextWorkflow, this.txtWorkflowControlNo.Text);
                    toUserInfo = this.GetUserInfo(toUserID);
                }
                else
                {
                    // End, TurnBack以外
                    toUserInfo = this.GetUserInfo((decimal)nextWorkflow["ToUserId"]);
                }

                int mailTemplateId = 0;
                mailTemplateId = wf.RequestWfApproval(
                    nextWorkflow, this.txtWorkflowControlNo.Text,
                    fromUserID, fromUserInfo, toUserID, toUserInfo, 
                    this.txtCurrentWorkflowReserveArea2.Text, this.dtpReplyDeadline2.Value);

                // ★ ココでメールを送信。

                this._dam.CommitTransaction();
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        #endregion

        #region 共通関数

        /// <summary>Damの初期化</summary>
        private void InitDam()
        {
            // 現状SQLServerのみ対応
            this._dam = new DamSqlSvr();
            this._dam.Obj = new MyUserInfo("Workflow_Tool", "127.0.0.1");

            this._dam.ConnectionOpen(this.txtCnnstr.Text);
            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
        }

        /// <summary>ユーザ情報を取得する</summary>
        /// <param name="uid">ユーザID</param>
        /// <returns>ユーザ情報</returns>
        private string GetUserInfo(decimal uid)
        {
            string temp = "";

            DataTable dt = new DataTable();
            DaoM_User dao = new DaoM_User(this._dam);
            dao.PK_Id = uid;
            dao.D2_Select(dt);

            // ユーザ情報の生成
            foreach(object o in dt.Rows[0].ItemArray)
            {
                if (temp == "")
                {
                    temp += o.ToString();
                }
                else
                {
                    temp += "_" + o.ToString();
                }
            }

            return temp;
        }

        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedTabIndex = ((System.Windows.Forms.TabControl)(sender)).SelectedTab.TabIndex;

            switch (selectedTabIndex)
            {
                case 0:
                    this.txtCnnstr.Enabled = true;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = false;
                    this.txtWorkflowControlNo.BackColor = Color.Empty;
                    //---
                    this.txtSubSystemId.Enabled = true;
                    this.txtSubSystemId.BackColor = Color.Yellow; 
                    this.txtWorkflowName.Enabled = true;
                    this.txtWorkflowName.BackColor = Color.Yellow; 
                    //---
                    this.txtDearSirUID.Enabled = true;
                    this.txtDearSirUID.BackColor = Color.Yellow; 
                    this.txtDearSirPTitleId.Enabled = true;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow; 
                    //---
                    break;

                case 1:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = true;
                    this.txtWorkflowControlNo.BackColor = Color.Aqua;
                    //---
                    this.txtSubSystemId.Enabled = false;
                    this.txtSubSystemId.BackColor = Color.Empty;
                    this.txtWorkflowName.Enabled = false;
                    this.txtWorkflowName.BackColor = Color.Empty;
                    //---
                    this.txtDearSirUID.Enabled = false;
                    this.txtDearSirUID.BackColor = Color.Empty;
                    this.txtDearSirPTitleId.Enabled = false;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                case 2:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = false;
                    this.txtWorkflowControlNo.BackColor = Color.Empty;
                    //---
                    this.txtSubSystemId.Enabled = true;
                    this.txtSubSystemId.BackColor = Color.Yellow;
                    this.txtWorkflowName.Enabled = true;
                    this.txtWorkflowName.BackColor = Color.Yellow;
                    //---
                    this.txtDearSirUID.Enabled = true;
                    this.txtDearSirUID.BackColor = Color.Yellow;
                    this.txtDearSirPTitleId.Enabled = true;
                    this.txtDearSirPTitleId.BackColor = Color.Yellow;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                case 3:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = false;
                    this.txtWorkflowControlNo.BackColor = Color.Empty;
                    //---
                    this.txtSubSystemId.Enabled = false;
                    this.txtSubSystemId.BackColor = Color.Empty;
                    this.txtWorkflowName.Enabled = false;
                    this.txtWorkflowName.BackColor = Color.Empty;
                    //---
                    this.txtDearSirUID.Enabled = false;
                    this.txtDearSirUID.BackColor = Color.Empty;
                    this.txtDearSirPTitleId.Enabled = false;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                case 4:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = false;
                    this.txtWorkflowControlNo.BackColor = Color.Empty;
                    //---
                    this.txtSubSystemId.Enabled = true;
                    this.txtSubSystemId.BackColor = Color.Yellow;
                    this.txtWorkflowName.Enabled = true;
                    this.txtWorkflowName.BackColor = Color.Yellow;
                    //---
                    this.txtDearSirUID.Enabled = false;
                    this.txtDearSirUID.BackColor = Color.Empty;
                    this.txtDearSirPTitleId.Enabled = false;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                case 5:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = false;
                    this.txtWorkflowControlNo.BackColor = Color.Empty;
                    //---
                    this.txtSubSystemId.Enabled = false;
                    this.txtSubSystemId.BackColor = Color.Empty;
                    this.txtWorkflowName.Enabled = false;
                    this.txtWorkflowName.BackColor = Color.Empty;
                    //---
                    this.txtDearSirUID.Enabled = true;
                    this.txtDearSirUID.BackColor = Color.Yellow;
                    this.txtDearSirPTitleId.Enabled = false;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                case 6:
                    this.txtCnnstr.Enabled = false;
                    this.txtCnnstr.BackColor = Color.Empty;
                    //---
                    this.txtWorkflowControlNo.Enabled = true;
                    this.txtWorkflowControlNo.BackColor = Color.Yellow;
                    //---
                    this.txtSubSystemId.Enabled = false;
                    this.txtSubSystemId.BackColor = Color.Empty;
                    this.txtWorkflowName.Enabled = false;
                    this.txtWorkflowName.BackColor = Color.Empty;
                    //---
                    this.txtDearSirUID.Enabled = false;
                    this.txtDearSirUID.BackColor = Color.Empty;
                    this.txtDearSirPTitleId.Enabled = false;
                    this.txtDearSirPTitleId.BackColor = Color.Empty;
                    this.txtUserID.Enabled = true;
                    this.txtUserID.BackColor = Color.Yellow;
                    //---
                    break;

                default:
                    break;
            }
        }

        

        

        

        
    }
}