using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace BVH.HTiktok
{
    static class Program
    {
        static AdbClient _client;
        static DeviceData _device;
        static int _waitSecond = 5;
        static Random _random = new Random();

        static void Main()
        {
            Console.WriteLine("abc");
            if (!AdbServer.Instance.GetStatus().IsRunning)
            {
                var _server = new AdbServer();
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

            // install tiktok.apk
            PackageManager manager = new PackageManager(_client, _device);
            manager.InstallPackage(@"C:\Users\haubv\OneDrive\Documents\platform-tools\tiktok 24.3.3.apk", reinstall: false);

            //_client.ExecuteRemoteCommand("pm list packages -f", _device, receiver);

            _client.StartApp(_device, "com.ss.android.ugc.trill");

            // Chuyển sang màn Default
            Thread.Sleep(10000);
            XmlDocument screen = _client.DumpScreen(_device);

            ByPassSignUpScreen();

            // Chuyển sang màn Sign up - BirthDay
            Thread.Sleep(3000);

            // Swipe Month
            Element cbMonth = _client.FindElement(_device, "//node[@resource-id='com.ss.android.ugc.trill:id/d9g']", TimeSpan.FromSeconds(_waitSecond));
            _client.Swipe(_device, cbMonth.Area.X, cbMonth.Area.Y, cbMonth.Area.Right, _random.Next(cbMonth.Area.Y, cbMonth.Area.Bottom), 50);
            Thread.Sleep(300);
            // Swipe Day
            Element cbDay = _client.FindElement(_device, "//node[@resource-id='com.ss.android.ugc.trill:id/as1']", TimeSpan.FromSeconds(_waitSecond));
            _client.Swipe(_device, cbDay.Area.X, cbDay.Area.Y, cbDay.Area.Right, _random.Next(cbDay.Area.Y, cbDay.Area.Bottom), 50);
            Thread.Sleep(300);
            // Swipe Year
            Element cbYear = _client.FindElement(_device, "//node[@resource-id='com.ss.android.ugc.trill:id/gqs']", TimeSpan.FromSeconds(_waitSecond));
            _client.Swipe(_device, cbYear.Area.X, cbYear.Area.Y, cbYear.Area.Right, cbYear.Area.Bottom, 10);
            Thread.Sleep(100);
            _client.Swipe(_device, cbYear.Area.X, cbYear.Area.Y, cbYear.Area.Right, cbYear.Area.Bottom, 10);
            Thread.Sleep(100);
            _client.Swipe(_device, cbYear.Area.X, cbYear.Area.Y, cbYear.Area.Right, cbYear.Area.Bottom, 10);
            for (int i = 0; i < _random.Next(2, 8); i++)
            {
                _client.Swipe(_device, cbYear.Area.X, cbYear.Area.Y, cbYear.Area.Right, cbYear.Area.Y+100, 100);
            }

            Element btnNext = _client.FindElement(_device, "//node[@text='Next']", TimeSpan.FromSeconds(_waitSecond));
            btnNext.Click();

            // Chuyển sang màn SignUp - Email
            Thread.Sleep(3000);

            Element btnEmail = _client.FindElement(_device, "//node[@text='Email']", TimeSpan.FromSeconds(_waitSecond));
            btnNext.Click();
            _client.Click(_device, btnEmail.Area.Center.X, btnEmail.Area.Center.Y);

            var email = GetRandomEmail();

            Element txtEmail = _client.FindElement(_device, "//node[@text='Email address']", TimeSpan.FromSeconds(_waitSecond));
            if(txtEmail != null)
            {
                txtEmail.SendText(email);
                Thread.Sleep(300);
                _client.SendKeyEvent(_device, "KEYCODE_NAVIGATE_NEXT");

                // Ấn nút next nếu có
                Element btnNextEmail = _client.FindElement(_device, "//node[@text='Next']", TimeSpan.FromSeconds(_waitSecond));
                Click(btnNextEmail);

                Thread.Sleep(5000);
                var bypassCapt = ByPassCaptchaScene();
                if(bypassCapt)
                {
                    Thread.Sleep(5000);
                    Element txtPassword = _client.FindElement(_device, "//node[@text='Enter password']", TimeSpan.FromSeconds(_waitSecond));
                    if(txtPassword != null)
                    {
                        txtPassword.SendText("Ipvhello2023$");
                        Element btnNextPassword = _client.FindElement(_device, "//node[@text='Next']", TimeSpan.FromSeconds(_waitSecond));
                        Click(btnNextPassword);
                    }

                    // Sang màn tạo username
                    Thread.Sleep(5000);
                    Element btnSkipCreateUsername = _client.FindElement(_device, "//node[@text='Skip']", TimeSpan.FromSeconds(_waitSecond));
                    Click(btnSkipCreateUsername);

                    Thread.Sleep(3000);
                    // Nếu là màn Choose interests thì click skip
                    Element lbChooseInterest = _client.FindElement(_device, "//node[@text='Choose your interests']", TimeSpan.FromSeconds(_waitSecond));
                    if(lbChooseInterest != null)
                    {
                        Element btnSkipInterest = _client.FindElement(_device, "//node[@text='Skip']", TimeSpan.FromSeconds(_waitSecond));
                        Click(btnSkipInterest);
                        Thread.Sleep(3000);
                    }

                    // Nếu là màn Swipe up thì click start watching
                    Element lbSwipeup = _client.FindElement(_device, "//node[@text='Swipe up']", TimeSpan.FromSeconds(_waitSecond));
                    if(lbSwipeup != null)
                    {
                        Element btnStartWatching = _client.FindElement(_device, "//node[@text='Start watching']", TimeSpan.FromSeconds(_waitSecond));
                        Click(btnStartWatching);
                        Thread.Sleep(3000);
                    }

                    // Nếu là màn allow tiktok to access contact thì click Deny
                    Element lbAllowContact = _client.FindElement(_device, "//node[@text='Allow TikTok to access your contacts?']", TimeSpan.FromSeconds(_waitSecond));
                    if (lbAllowContact != null)
                    {
                        Element btnDeny = _client.FindElement(_device, "//node[@text='Deny']", TimeSpan.FromSeconds(_waitSecond));
                        Click(btnDeny);
                        Thread.Sleep(3000);
                    }

                    // Sau cùng đến màn hình có video thì swipe để lướt 1 video
                    Thread.Sleep(2000);
                    _client.Swipe(_device, 934, 1373, 146, 585, 100);

                    // 

                }
            }    
            

            Console.ReadLine();
        }

        static void ByPassSignUpScreen()
        {
            // Nếu ra màn hình có nút Signup
            Element btnSignup = _client.FindElement(_device, "//node[@text='Sign up']", TimeSpan.FromSeconds(_waitSecond));
            if (btnSignup != null)
            {
                btnSignup.Click();
            }

            // Nếu ra màn hình có label Log in to Tiktok, có nút Dont have an account? Signup
            Element btnSignup2 = _client.FindElement(_device, "//node[@text='Don’t have an account? Sign up']", TimeSpan.FromSeconds(_waitSecond));
            if(btnSignup2 != null)
            {
                btnSignup2.Click();
            }

            // Nếu ra màn hình có label Sign up for Tiktok
            Element lbSignUpForTiktok = _client.FindElement(_device, "//node[@text='Sign up for TikTok']", TimeSpan.FromSeconds(_waitSecond));
            if (lbSignUpForTiktok != null)
            {
                Element btnUsePhoneOrEmail = _client.FindElement(_device, "//node[@text='Use phone or email']", TimeSpan.FromSeconds(_waitSecond));
                Click(btnUsePhoneOrEmail);
                return;
            }

            // Nếu là màn Agree and continue
            Element btnAgreeAndContinue = _client.FindElement(_device, "//node[@text='Agree and continue']", TimeSpan.FromSeconds(_waitSecond));
            if (btnAgreeAndContinue != null)
            {
                Click(btnAgreeAndContinue);
                // Chuyển màn 
                Thread.Sleep(3000);

                // Nếu là màn Choose interests thì click skip
                Element btnSkipInterest = _client.FindElement(_device, "//node[@text='Skip']", TimeSpan.FromSeconds(_waitSecond));
                Click(btnSkipInterest);

                // Chuyển màn
                Thread.Sleep(3000);

                // Nếu là màn Swipe up thì swipe
                Element btnStartWatching = _client.FindElement(_device, "//node[@text='Start watching']", TimeSpan.FromSeconds(_waitSecond));
                if (btnStartWatching != null)
                {
                    btnStartWatching.Click();
                }

                // Chuyển sang màn Xem video
                Thread.Sleep(10000);
                _client.Swipe(_device, 200, 500, 800, 1500, 100);
            }
        }

        static bool ByPassCaptchaScene()
        {
            var result = false;

            // captcha loại kéo thả
            if(_client.FindElement(_device, "//node[@text='Drag the puzzle piece into place']", TimeSpan.FromSeconds(_waitSecond)) != null)
            {
                string base64String = String.Empty;
                using (var ms = new MemoryStream())
                {
                    using (var bitmap = _client.GetFrameBuffer(_device).ToImage())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        base64String = Convert.ToBase64String(ms.GetBuffer()); //Get Base64
                    }
                }

                if(!String.IsNullOrEmpty(base64String))
                {
                    var createJobResult = OmoCaptchaProxy.CreateJob(base64String, "24");
                    if(!createJobResult.error && createJobResult.job_id > 0)
                    {
                        for (int i = 0; i < 13; i++)
                        {
                            Thread.Sleep((i + 1) * 2000);
                            var getJobResult = OmoCaptchaProxy.GetResultJob(createJobResult.job_id);
                            if(getJobResult.status == "success" && !getJobResult.error && !String.IsNullOrEmpty(getJobResult.result))
                            {
                                var splitPos = getJobResult.result.Split('|');
                                if(splitPos != null && splitPos.Length == 4)
                                {
                                    _client.Swipe(_device, int.Parse(splitPos[0]), int.Parse(splitPos[1]), int.Parse(splitPos[2]), int.Parse(splitPos[3]), 1000);
                                    result = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            // captcha loại xoay
            else if (_client.FindElement(_device, "//node[@text='Drag the slider to fit the puzzle']", TimeSpan.FromSeconds(_waitSecond)) != null)
            {
                var allImgs = _client.FindElements(_device, "//node[@class='android.widget.Image']", TimeSpan.FromSeconds(_waitSecond));
                List<string> captchaImgs = new List<string>();
                if(allImgs != null && allImgs.Count() > 0)
                {
                    foreach( var img in allImgs)
                    {
                        var iText = img.Attributes["text"];
                        if(!String.IsNullOrWhiteSpace(iText) && iText.Contains("whirl"))
                        {
                            captchaImgs.Add(iText);
                        }
                    }
                }

                if(captchaImgs.Count == 2)
                {
                    var outerBase64 = Utilities.ImageUrlToBase64($"https://p16-security-va.ibyteimg.com/img/security-captcha-oversea-usa/{captchaImgs[0]}.image").Result;
                    var innerBase64 = Utilities.ImageUrlToBase64($"https://p16-security-va.ibyteimg.com/img/security-captcha-oversea-usa/{captchaImgs[1]}.image").Result;
                    
                    if(!String.IsNullOrEmpty(outerBase64) && !String.IsNullOrEmpty(innerBase64))
                    {
                        var createJobResult = OmoCaptchaProxy.CreateJob($"{innerBase64}|{outerBase64}", "23");
                        if (!createJobResult.error && createJobResult.job_id > 0)
                        {
                            for (int i = 0; i < 13; i++)
                            {
                                Thread.Sleep((i + 1) * 2000);
                                var getJobResult = OmoCaptchaProxy.GetResultJob(createJobResult.job_id);
                                if (getJobResult.status == "success" && !getJobResult.error && !String.IsNullOrEmpty(getJobResult.result))
                                {
                                    var btnDrag = _client.FindElement(_device, "//node[@resource-id='secsdk-captcha-drag-wrapper']", TimeSpan.FromSeconds(_waitSecond));
                                    if(btnDrag != null)
                                    {
                                        var dragLength = (795 * int.Parse(getJobResult.result)) / 340;
                                        _client.Swipe(_device, btnDrag.Area.Center.X, btnDrag.Area.Center.Y, btnDrag.Area.Center.X + dragLength, btnDrag.Area.Center.Y, 1000);
                                        result = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                }


            }



            return result;
        }

        static bool Click(Element el)
        {
            if(el != null)
            {
                el.Click();
                return true;
            }

            return false;
        }

        static string GetRandomEmail()
        {
            return $"{Utilities.RandomName()}{Utilities.RandomName()}{Utilities.RandomEmailSubfix(6)}@gmail.com";
        }
    }
}
