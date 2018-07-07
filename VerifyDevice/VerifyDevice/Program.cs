using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using VerifyDevice.Helper;
using System.IO;
using System.Web.Hosting;

namespace VerifyDevice
{
    static class Program
    {
        static void Main(string[] args)
        {
            CallVerifyDevice().Wait();
            //Console.ReadKey();

        }

        static async Task CallVerifyDevice()
        {
            try
            {
                List<deviceLocation> lstDevice = new List<deviceLocation>();
                deviceManager deviceManagerHelper = new deviceManager();
                lstDevice = deviceManagerHelper.GetDeviceList();
                Console.WriteLine("------------------ Count--" + (lstDevice != null ? lstDevice.Count.ToString() : "0") + "--------------------");
                if (lstDevice != null)
                {
                    string username = System.Configuration.ConfigurationSettings.AppSettings["APIUserName"].ToString();
                    string password = System.Configuration.ConfigurationSettings.AppSettings["APIPassword"].ToString();
                    var byteArray = Encoding.ASCII.GetBytes(username + ":" + password);
                    string Base64Credents = Convert.ToBase64String(byteArray);
                    string BaseAddress = System.Configuration.ConfigurationSettings.AppSettings["ServiceUrl"].ToString();
                    foreach (var item in lstDevice)
                    {

                        Console.WriteLine("------------------Start:- " + item.DeviceId + "/ " + item.Location + "--------------------");
                        HttpClient cons = new HttpClient();
                        cons.BaseAddress = new Uri(BaseAddress);
                        cons.DefaultRequestHeaders.Accept.Clear();
                        cons.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Base64Credents);
                        cons.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        cons.Timeout = TimeSpan.FromMinutes(2);
                        using (cons)
                        {
                            var deviceMgr = new deviceLocation { DeviceId = item.DeviceId, Location = item.Location };
                            HttpResponseMessage res = await PostAsJsonAsync(cons, "api/verifyDevice", deviceMgr);
                            res.EnsureSuccessStatusCode();
                            if (res.IsSuccessStatusCode)
                            {
                                Console.WriteLine("------------------Success--------------------");
                            }
                            else
                            {
                                sendMail mailHelper = new sendMail(sendMail.EMAIL_SENDER, sendMail.EMAIL_CREDENTIALS, sendMail.SMTP_CLIENT);
                                if (mailHelper.SendEMail("alert:" + " Device Failure - ", item.DeviceId, "Location: " + item.Location))
                                    Console.WriteLine("------------------Failure mail sent--------------------");

                            }
                        }
                        Console.WriteLine("------------------End:-" + item.DeviceId + "/ " + item.Location + "--------------------");

                    }
                }            
            }
            catch (Exception ex)
            {
                sendMail mailHelper = new sendMail(sendMail.EMAIL_SENDER, sendMail.EMAIL_CREDENTIALS, sendMail.SMTP_CLIENT);
                if (mailHelper.SendEMail("alert: ", "Batch failure", "Error in contacting PING API" + ex.ToString()))
                    Console.WriteLine("------------------Error: Failure mail sent--------------------" + Environment.NewLine + ex.ToString());
            }
        }

        static async Task<HttpResponseMessage> PostAsJsonAsync(this HttpClient client, string addr, object obj)
        {
            var response = await client.PostAsync(addr, new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(obj),
                    Encoding.UTF8, "application/json"));

            return response;
        }
    }
}
