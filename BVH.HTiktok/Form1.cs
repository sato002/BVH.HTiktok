using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BVH.HTiktok
{
    public partial class Form1 : Form
    {
        static AdbClient _client;
        static DeviceData _device;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("abc");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!AdbServer.Instance.GetStatus().IsRunning)
            {
                var _server  = new AdbServer();
                StartServerResult result = _server.StartServer(@"C:\Users\haubv\OneDrive\Documents\platform-tools\adb.exe", false);
                if (result != StartServerResult.Started)
                {
                    Console.WriteLine("Can't start adb server");
                    return;
                }
            }

            // Connect Adb.exe and get first device
            _client = new AdbClient();
            _client.Connect("127.0.0.1:62001");
            _device = _client.GetDevices().FirstOrDefault();

            var receiver = new ConsoleOutputReceiver();
            _client.ExecuteRemoteCommand("echo Hello, World", _device, receiver);

            // install tiktok.apk
            PackageManager manager = new PackageManager(_client, _device);
            manager.InstallPackage(@"C:\Users\haubv\OneDrive\Documents\platform-tools\tiktok 24.3.3.apk", reinstall: false);

            _client.StartApp(_device, "tiktok");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        
    }
}
