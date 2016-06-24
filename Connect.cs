﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace IssueCreator
{
    public partial class Connect : System.Windows.Forms.Form
    {
        public string path;
        public string site;
        public string username;
        public SecureString password = new SecureString();
        public ClientContext cc;
        public Web uploadWeb;
        public Site uploadSite;
        public List issuesList;
        public bool connected = false;
        public Configuration configuration;


        public Connect()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textUsername.Text = username;
            textFolder.Text = path;
            linkIssuesList.Tag = site + "/lists/issues";
            linkSite.Tag = site;

            if (password.Length > 0)
            {
                textPassword.Text = "xxxxxxxxxxxxx";
                textPassword.Enabled = false;
            }
            
            FormState();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textPassword.Text) && !string.IsNullOrEmpty(textUsername.Text) && !string.IsNullOrEmpty(textFolder.Text))
            {
                username = textUsername.Text;

                if (textPassword.Text != "xxxxxxxxxxxxx")
                    foreach (char character in textPassword.Text)
                    {
                        password.AppendChar(character);
                    }

                try
                {
                    cc = new ClientContext(site);
                }
                catch (Exception)
                {
                    MessageBox.Show("Site URL is incorrect, you need to adjust this in the config file.");
                    return;
                }

                uploadWeb = cc.Web;
                uploadSite = cc.Site;
                issuesList = uploadWeb.Lists.GetByTitle("Issues");

                cc.Credentials = new SharePointOnlineCredentials(username, password);
                cc.Load(uploadWeb);
                cc.Load(uploadSite);
                cc.Load(issuesList);

                try
                {
                    cc.ExecuteQuery();
                }
                catch (Exception)
                {
                    MessageBox.Show("An error has occurred.");
                }

                buttonConnect.Text = "Connected";
                grpSharePoint.Enabled = false;
                connected = true;

                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
                throw new NullReferenceException();

            username = textUsername.Text;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textFolder.Text = folderBrowserDialog1.SelectedPath;

                path = folderBrowserDialog1.SelectedPath;
                FormState();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void FormState()
        {
            if (string.IsNullOrEmpty(textPassword.Text) || string.IsNullOrEmpty(textUsername.Text) || string.IsNullOrEmpty(textFolder.Text))
                buttonConnect.Enabled = false;
            else
                buttonConnect.Enabled = true;
        }

        private void textUsername_Leave(object sender, EventArgs e)
        {
            FormState();
        }

        private void textPassword_Leave(object sender, EventArgs e)
        {
            FormState();
        }

        private void textPassword_TextChanged(object sender, EventArgs e)
        {
            FormState();
        }

        private void textUsername_TextChanged(object sender, EventArgs e)
        {
            FormState();
        }

        private void linkIssuesList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkIssuesList.Tag.ToString());
        }

        private void linkSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkSite.Tag.ToString());
        }
    }
}
