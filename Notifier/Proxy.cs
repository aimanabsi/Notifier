// Decompiled with JetBrains decompiler
// Type: Notifier.Proxy
// Assembly: Notifier, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9DE75DB0-3730-4642-95AB-184A73C2A6F3
// Assembly location: F:\azizo\Repos\YJLAB\Notifier.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Notifier
{
    internal class Proxy
    {
        public static async Task Notify(
          string phn,
          string msgID,
          string smsType = "",
          string pCode = "",
          string vNo = "",
          string timeNo = "",
          NotifyMsg msg = null)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://smaartnets.com");
                string json = JsonConvert.SerializeObject((object)msg);
                FormUrlEncodedContent content = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)new KeyValuePair<string, string>[9]
                {
          new KeyValuePair<string, string>("op", "notify"),
          new KeyValuePair<string, string>("aID", "1"),
          new KeyValuePair<string, string>("brn_no", "2"),
          new KeyValuePair<string, string>(nameof (phn), phn),
          new KeyValuePair<string, string>(nameof (pCode), pCode),
          new KeyValuePair<string, string>("serial", msgID),
          new KeyValuePair<string, string>(nameof (smsType), smsType),
          new KeyValuePair<string, string>("v_no", vNo),
          new KeyValuePair<string, string>("time_no", timeNo)
                });
                HttpResponseMessage result = await client.PostAsync("/yj/gateway.php", (HttpContent)content);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (resultContent == "1")
                    LogWriter.WriteErrorLog("The Notif was recieved and processed succefully by the gateway! ");
                DBConnection.UpdateSentMsg(resultContent);
                json = (string)null;
                content = (FormUrlEncodedContent)null;
                result = (HttpResponseMessage)null;
                resultContent = (string)null;
            }
        }

        public static async Task SendReport(DataTable dataTable)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://smaartnets.com");
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject((object)dataTable);
                StringContent content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync("/yj/brn2_handle_report.php", (HttpContent)content);
                string resultContent = await result.Content.ReadAsStringAsync();
                LogWriter.WriteErrorLog("SentReport " + resultContent);
                try
                {
                    string[] ids = JsonConvert.DeserializeObject<string[]>(resultContent);
                    string[] strArray = ids;
                    for (int index = 0; index < strArray.Length; ++index)
                    {
                        string id = strArray[index];
                        DBConnection.UpdateSentRpt(id);
                        id = (string)null;
                    }
                    strArray = (string[])null;
                    ids = (string[])null;
                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Fucken exxception happend:D" + ex.Message);
                }
                jsonString = (string)null;
                content = (StringContent)null;
                result = (HttpResponseMessage)null;
                resultContent = (string)null;
            }
        }

        public static async Task SendSMS(DataTable dataTable)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://smaartnets.com");
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject((object)dataTable);
                StringContent content = new StringContent(jsonString.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync("/yj/brn2_handle_sms.php", (HttpContent)content);
                string resultContent = await result.Content.ReadAsStringAsync();
                LogWriter.WriteErrorLog(" " + resultContent);
                try
                {
                    string[] ids = JsonConvert.DeserializeObject<string[]>(resultContent);
                    string[] strArray = ids;
                    for (int index = 0; index < strArray.Length; ++index)
                    {
                        string id = strArray[index];
                        DBConnection.UpdateSentMsg(id);
                        id = (string)null;
                    }
                    strArray = (string[])null;
                    ids = (string[])null;
                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Fucken exxception happend:D" + ex.Message);
                }
                jsonString = (string)null;
                content = (StringContent)null;
                result = (HttpResponseMessage)null;
                resultContent = (string)null;
            }
        }

        public static async Task SendReport(DataRow Row)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://smaartnets.com");
                KeyValuePair<string, string>[] paramsArray = new KeyValuePair<string, string>[29];
                paramsArray[0] = new KeyValuePair<string, string>("op", "report");
                int rNo = 1;
                paramsArray[1] = new KeyValuePair<string, string>("p_name", Row["p_name"].ToString());
                paramsArray[2] = new KeyValuePair<string, string>("testdesc", Row["testdesc"].ToString());
                paramsArray[3] = new KeyValuePair<string, string>("testresult", Row["testresult"].ToString());
                paramsArray[4] = new KeyValuePair<string, string>("op_serial", Row["op_serial"].ToString());
                paramsArray[5] = new KeyValuePair<string, string>("p_no", Row["p_no"].ToString());
                paramsArray[6] = new KeyValuePair<string, string>("p_month", Row["p_month"].ToString());
                paramsArray[7] = new KeyValuePair<string, string>("p_year", Row["p_year"].ToString());
                try
                {
                    paramsArray[27] = new KeyValuePair<string, string>("op_type", Row["op_type"].ToString());
                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog(ex.Message);
                }
                try
                {
                    paramsArray[28] = new KeyValuePair<string, string>("brn_no", "1");
                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog(ex.Message);
                }
                paramsArray[8] = new KeyValuePair<string, string>("p_group", Row["p_group"].ToString());
                paramsArray[9] = new KeyValuePair<string, string>("p_age", Row["p_age"].ToString());
                paramsArray[10] = new KeyValuePair<string, string>("p_age_unit", Row["p_age_unit"].ToString());
                paramsArray[11] = new KeyValuePair<string, string>("p_sex", Row["p_sex"].ToString());
                paramsArray[12] = new KeyValuePair<string, string>("p_doctor", Row["p_doctor"].ToString());
                paramsArray[13] = new KeyValuePair<string, string>("testtype", Row["testtype"].ToString());
                paramsArray[14] = new KeyValuePair<string, string>("v_no", Row["v_no"].ToString());
                paramsArray[15] = new KeyValuePair<string, string>("testno", Row["testno"].ToString());
                paramsArray[16] = new KeyValuePair<string, string>("testgroup", Row["testgroup"].ToString());
                paramsArray[18] = new KeyValuePair<string, string>("cash_no", Row["cash_doc"].ToString());
                paramsArray[19] = new KeyValuePair<string, string>("time_no", Row["time_no"].ToString());
                paramsArray[20] = new KeyValuePair<string, string>("dctrname", Row["dctrname"].ToString());
                paramsArray[21] = new KeyValuePair<string, string>("type_name", Row["type_name"].ToString());
                paramsArray[22] = new KeyValuePair<string, string>("comments", Row["comments"].ToString());
                paramsArray[23] = new KeyValuePair<string, string>("testunit", Row["testunit"].ToString());
                paramsArray[24] = new KeyValuePair<string, string>("testrange", Row["testrange"].ToString());
                paramsArray[25] = new KeyValuePair<string, string>("testrange1", Row["testrange1"].ToString());
                paramsArray[26] = new KeyValuePair<string, string>("p_date", Row["p_date"].ToString());
                FormUrlEncodedContent content = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)paramsArray);
                HttpResponseMessage result = await client.PostAsync("/yj/gateway.php", (HttpContent)content);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (resultContent != null && resultContent != "")
                {
                    LogWriter.WriteErrorLog("The Report was recieved and processed succefully by the gateway! " + resultContent);
                    DBConnection.UpdateSentRpt(resultContent);
                }
                paramsArray = (KeyValuePair<string, string>[])null;
                content = (FormUrlEncodedContent)null;
                result = (HttpResponseMessage)null;
                resultContent = (string)null;
            }
        }

        public bool isServerConnected(string server)
        {
            bool flag = false;
            Ping ping = new Ping();
            try
            {
                flag = ping.Send(server).Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
            }
            return flag;
        }
    }
}
