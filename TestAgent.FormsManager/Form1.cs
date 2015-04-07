using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Core;
using TestAgent.Core;
using TestAgent.Services.FileService;
using TestAgent.Services.TestService;

namespace TestAgent.FormsManager
{
    public partial class Form1 : Form
    {
        private FileServiceClientHost _fileClient;
        private TestServiceClientHost _testClient;

        public Form1()
        {
            InitializeComponent();
            AcceptButton = btnConnect;
        }

        public virtual string[] LoadTests(string filename)
        {
            var results = new List<string>();
            var package = new TestPackage(filename);

            CoreExtensions.Host.InitializeService();

            var builder = new TestSuiteBuilder();

            var suite = builder.Build(package);

            foreach (object test in suite.Tests)
            {
                var testSuite = test as TestSuite;
                if (testSuite == null) { continue; }
                results.AddRange(GetSteps(testSuite));
            }

            CoreExtensions.Host.UnloadService();

            return results.ToArray();
        }

        private static string[] GetSteps(TestSuite testSuite)
        {
            var results = new List<string>();

            foreach (var testFixture in testSuite.Tests)
            {
                var fixture = testFixture as TestFixture;
                if (fixture != null)
                {
                    results.AddRange(GetSteps(fixture));
                }
                else
                {
                    var suite = testFixture as TestSuite;
                    if (suite != null)
                    {
                        results.AddRange(GetSteps(suite));
                    }
                }
            }

            return results.ToArray();
        }

        private static string[] GetSteps(TestFixture fixture)
        {
            var results = new List<string>();

            foreach (var test in fixture.Tests)
            {
                var method = test as TestMethod;
                if (method != null)
                {
                    results.Add(method.TestName.FullName);
                }
                else
                {
                    var suite = test as ParameterizedMethodSuite;
                    if (suite != null)
                    {
                        var methodSuite = suite;
                        results.AddRange(GetSteps(methodSuite));
                    }
                }
            }

            return results.ToArray();
        }

        private static string[] GetSteps(ParameterizedMethodSuite methodSuite)
        {
            var results = new List<string>();

            foreach (var test1 in methodSuite.Tests)
            {
                var method = test1 as TestMethod;
                if (method != null)
                {
                    if (results.All(test => test.ToString() != method.TestName.Name))
                    {
                        results.Add(method.TestName.Name);
                    }
                }
            }

            return results.ToArray();
        }

        private void txtHostname_TextChanged(object sender, EventArgs e)
        {
            btnConnect.Enabled = !string.IsNullOrEmpty(txtHostname.Text);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _fileClient = new FileServiceClientHost();
            _fileClient.Connect(txtHostname.Text, 9123);

            _testClient = new TestServiceClientHost();
            _testClient.OutputReceived += TestClientOnOutputReceived;
            _testClient.Connect(txtHostname.Text, 9123);

            txtHostname.Enabled =
            btnConnect.Enabled = _fileClient.State != CommunicationState.Opened &&
                                 _testClient.State != CommunicationState.Opened;

            pnlMain.Enabled = !txtHostname.Enabled;

            AcceptButton = btnRun;
        }

        private void TestClientOnOutputReceived(object sender, string s)
        {
            AppendText(s);
        }
        
        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text)));
            }
            else
            {
                txtOutput.Text += text + Environment.NewLine;
            }
        }

        private void btnSelectFiles_Click(object sender, EventArgs e)
        {
            var openDlg = new OpenFileDialog();
            openDlg.Filter = "All Files|*.*";
            openDlg.Multiselect = true;

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                lstFiles.Items.Clear();
                cbTestfile.Items.Clear();
                foreach (var fileName in openDlg.FileNames)
                {
                    lstFiles.Items.Add(fileName);
                    cbTestfile.Items.Add(fileName);
                }

                if (cbTestfile.Items.Count > 0)
                {
                    cbTestfile.SelectedItem = cbTestfile.Items[0];
                }
            }
        }

        private void cbTestfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSelectedTests.Items.Clear();

            // Try to load all test names from file
            var tests = LoadTests(cbTestfile.Text);
            foreach (var test in tests)
            {
                lstSelectedTests.Items.Add(test);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            // Make sure all information is entered
            if (lstFiles.Items.Count == 0)
            {
                MessageBox.Show("Select some files");
                return;
            }

            if (cbTestfile.SelectedItem == null)
            {
                MessageBox.Show("select test file");
                return;
            }

            if (lstSelectedTests.CheckedItems.Count == 0)
            {
                MessageBox.Show("Select tests to execute");
                return;
            }

            txtOutput.Text = string.Empty;

            // Zip all files
            string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "tmp.zip");

            var files = new List<string>();
            foreach (var item in lstFiles.Items)
            {
                files.Add(item.ToString());
            }
            FilePackager.CompressFiles(tempFile, files.ToArray());

            using (var stream = new FileStream(tempFile, FileMode.Open))
            {
                var uploadResult = _fileClient.Client.UploadFile(new UploadRequest()
                {
                    FileName = "TempFile.zip",
                    Stream = stream
                });

                if (!uploadResult.UploadResult)
                {
                    MessageBox.Show("File upload failed!");
                    return;
                }

                var names = new List<string>();
                foreach (var item in lstSelectedTests.CheckedItems)
                {
                    names.Add(item.ToString());
                }

                _testClient.Client.Register(Environment.MachineName);

                string testFile = Path.GetFileName(cbTestfile.Text);
                var t = new Thread(() =>
                _testClient.Client.StartTest(uploadResult.Token, testFile, TestType.NUnit, names.ToArray()));
                t.Start();
            }

            File.Delete(tempFile);
        }

    }
}
