using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DetectGithubDownloads
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Dictionary<string, string> changelog = new Dictionary<string, string>();
        private string version = "3.5.0";
        private int totalDownloads = 0;
        private string githubLink = "https://api.github.com/repos/capasha/eeditor/releases";
        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Status: Wating";
            label2.Text = "Total Downloads: 0";
            listView1.Items.Clear();
            changelog.Clear();
            richTextBox1.Clear();
            string text;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(githubLink);
                request.Method = "GET";
                request.Accept = "application/vnd.github.v3+json";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:48.0) Gecko/20100101 Firefox/48.0";
   				using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (request.HaveResponse && response != null)
                        {
                            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            {
                                text = reader.ReadToEnd();
                            }
                			dynamic stuff1 = Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                			foreach (var value in stuff1)
                			{
                    			if (value["tag_name"] != null) version = value["tag_name"];
                                if (value["body"] != null) changelog.Add(version, value["body"].ToString());
                            
                                if (value["assets"] != null)
                    			{
                        			foreach (var val in value["assets"])
                        			{
                                    Console.WriteLine(val);
                            			if (val["download_count"] != null)
                            			{
                                			listView1.Items.Add(version).SubItems.Add(val["download_count"].ToString());
                                        totalDownloads += Convert.ToInt32(val["download_count"]);

                                        }
                        			}
                        		}
                        	}
                        	label1.Text = "Status: Got the informations";
                            label2.Text = "Total Downloads: " + totalDownloads;
                        }
                        else 
                        {
                        	label1.Text = "Status: Couldn't get the information";
                        }
                    }
            }
            catch (WebException ee)
            {
                if (ee.Status == WebExceptionStatus.Timeout)
                {
                    label1.Text = "Status: Took too long time to load the site";
                }
                else if (ee.Status == WebExceptionStatus.ProtocolError)
                {
                    label1.Text = "Status: Protocol Error";
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                richTextBox1.Clear();
                richTextBox1.Text = changelog[listView1.SelectedItems[0].Text];
            }
        }
    }
}
