using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labor_3
{
    public partial class Form1 : Form
    {

        private List<string> blockedWords = new List<string> { "facebook", "twitter", "youtube" };

        private SQLiteHandler dbHandler = new SQLiteHandler();
        public Form1()
        {

            InitializeComponent();

            webBrowser1.Navigate("https://www.duckduckgo.com");
            webBrowser1.ScriptErrorsSuppressed = true;


            toolStripComboBox1.Items.Clear();
            foreach (string szo in blockedWords)
            {
                toolStripComboBox1.Items.Add(szo);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.google.com");
            Task.Run(delegate { WriteLog("Navigate gomb megnyomva"); });

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoBack)
                webBrowser1.GoBack();
            Task.Run(delegate { WriteLog("Back gomb megnyomva"); });
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoForward)
                webBrowser1.GoForward();
            Task.Run(delegate { WriteLog("Forward gomb megnyomva"); });
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
            Task.Run(delegate { WriteLog("Home gomb megnyomva"); });
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                webBrowser1.Navigate(toolStripTextBox1.Text);
            }
        }

        private async void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString().ToLower();
            string talalt = null;

            for (int i = 0; i < blockedWords.Count; i++)
            {
                if (url.Contains(blockedWords[i]))
                {
                    talalt = blockedWords[i];
                    break;
                }
            }

            if (talalt != null)
            {
                e.Cancel = true;
                WriteLog("Blocked page: " + talalt);
                MessageBox.Show("This page is blocked: " + talalt, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void WriteLog(string uzenet)
        {
            string logFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            string sor = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + uzenet;

            System.IO.File.AppendAllText(logFile, sor + Environment.NewLine);
        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string newWord = toolStripTextBox2.Text.Trim().ToLower();

            if (newWord == "")
            {
                MessageBox.Show("No word given!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool wordExists = (from szo in blockedWords
                               where szo == newWord
                               select szo).Any();

            if (wordExists)
            {
                MessageBox.Show("This word already exists in the list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                blockedWords.Add(newWord);
                toolStripComboBox1.Items.Add(newWord);
                toolStripTextBox2.Clear();
                MessageBox.Show("Word added to blocked words.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addKeyWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 kwdForm = new Form2();
            if (kwdForm.ShowDialog() == DialogResult.OK)
            {
                string kwd = kwdForm.getInputText();
                dbHandler.InsertKeyword(kwd);
                MessageBox.Show(string.Format("New keyword: {0}", kwd), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbHandler.ConnectToDb();

            List<string> dbWords = dbHandler.GetAllKeywords();

            for (int i = 0; i < dbWords.Count; i++)
            {
                string word = dbWords[i];
                bool exists = false;

                for (int j = 0; j < blockedWords.Count; j++)
                {
                    if (blockedWords[j] == word)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    blockedWords.Add(word);
                }
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbHandler.DisconnectFromDb();
        }

        private void viewKeywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> keywords = dbHandler.GetAllKeywords();
            MessageBox.Show(string.Join("\n", keywords), "Keywords in Database", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void deleteKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string keyword = Interaction.InputBox("Keyword to delete:", "Delete", "");

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("No keyword given.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dbHandler.DeleteKeyword(keyword);
            MessageBox.Show("Keyword deleted: " + keyword, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
    }
}
