using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.OleDb;

namespace Notifier
{
  public  class DBConnection
    {
        /*
         * - Connect to Oracle DB10g 
         * */

        //Connection string
        private static string TableName = "";
        private static string PrmryCol = "";
        private static string PhnCol = "";
        private static string SentFlgCol = "";
        public static string ConnectionString = "";
        private static string DBServer = "";
        private static string Port = "";
        private static string DBServiceName = "";
        private static string ReportTableName = "";
        public static string BrnNo = "";
        public static void  GetConfigurationData()
        {
            TableName = System.Configuration.ConfigurationSettings.AppSettings["TableName"];
            
            PrmryCol = System.Configuration.ConfigurationSettings.AppSettings["PrmryCol"];
            PhnCol = System.Configuration.ConfigurationSettings.AppSettings["PhnCol"];
            PhnCol = "p_tel";
            SentFlgCol = System.Configuration.ConfigurationSettings.AppSettings["SentFlgCol"];
            DBServer = System.Configuration.ConfigurationSettings.AppSettings["DBServer"];
            Port = System.Configuration.ConfigurationSettings.AppSettings["Port"];
            DBServiceName = System.Configuration.ConfigurationSettings.AppSettings["DBServiceName"];
            BrnNo = System.Configuration.ConfigurationSettings.AppSettings["BrnNo"];

            ConnectionString = " Data Source = (DESCRIPTION = (ADDRESS_LIST = "
           + "(ADDRESS=(PROTOCOL=TCP)(HOST="+ "192.168.0.1" + ")(PORT=1521)))"
           + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));"
           + "User Id=smsuser; Password=sms;";
          
        }

       public static void Connect()
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
            {
               
                try {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "select * from "+TableName+" WHERE "+SentFlgCol+" <> '1' AND ROWNUM < 100 ORDER BY op_serial ASC";
                    //  cmd.CommandType = CommandType.Text;
                    OracleDataAdapter dr = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    if (dt.Rows.Count>0)
                    {
                        /*
                         * - Two things to send : 
                         *   - The Notif plain
                         *   - The Test results JSON
                         *  
                         */
                        LogWriter.WriteErrorLog("Start sending SMS ");
                        Task.Run(() => Proxy.SendSMS(dt));

                    }

                   

                    else
                    {
                        LogWriter.WriteErrorLog("No new Notifs to send");

                    }


                  


                }
                catch(Exception ex)
                {
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !"+ex.Message);
                }

                ////Getting the data to send    
            }

        }

        public static void Tst()
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
            {

                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "select * from p_report where rownum<4";
                    //  cmd.CommandType = CommandType.Text;
                    OracleDataAdapter dr = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                  
                   if(dt.Rows.Count>0)
                    {
                    Task.Run( () =>   Proxy.SendReport(dt));
                    }
                    



                    else
                    {
                        LogWriter.WriteErrorLog("No new Notifs to send");
                      
                    }





                }
                catch (Exception ex)
                {
                 
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);
                }

                ////Getting the data to send    
            }

        }

        public static void GetReportData()
        {
        //    GetConfigurationData();

            using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
            {
                string phn;
                LogWriter.WriteErrorLog("Start to send reports ");
                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "select * from p_report WHERE trans_flg <> '1' OR trans_flg IS NULL ";
                    //  cmd.CommandType = CommandType.Text;
                    OracleDataAdapter dr = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        /*
                         *   - Two things to send : 
                         *   - The Notif plain
                         *   - The Test results JSON
                         *  
                         */

                        foreach (DataRow dRow in dt.Rows)
                        {
                            /*
                             *  - Prepate the required data to send 
                             * */
                            //phn = dRow[PhnCol].ToString();
                            //if (phn.Length < 12)
                            //    phn = "967" + phn;
                            //string msgID = dRow[PrmryCol].ToString();

                            Task.Run(() => Proxy.SendReport(dRow));

                        }

                    }

                    else
                    {
                        LogWriter.WriteErrorLog("No new Reports to send");

                    }

         }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message + " : " + TableName);
                }

                ////Getting the data to send    
            }

        }

        public static  void GetReportDataOle3()
        {


            string dbConStr = "" +
                     " Provider = MSDAORA.1; Data Source = (DESCRIPTION = (ADDRESS_LIST = "
         + "(ADDRESS=(PROTOCOL=TCP)(HOST=" + "192.168.0.1" + ")(PORT=1521)))"
         + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));"
         + "User Id=smsuser; Password=sms;";

            using (OleDbConnection conn = new OleDbConnection(dbConStr)) // C#
            {


                try
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "SELECT * FROM p_report WHERE (trans_flg <> '1' OR trans_flg IS NULL) AND ROWNUM < 250 ORDER BY op_serial ASC ";
                    //  cmd.CommandType = CommandType.Text;
                    OleDbDataAdapter dr = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        /*
                         *   - Two things to send : 
                         *   - The Notif plain
                         *   - The Test results JSON
                         *  
                         */
                        LogWriter.WriteErrorLog("Start to send jsreports  ");
                         Task.Run(() => Proxy.SendReport(dt));
                        //foreach (DataRow dRow in dt.Rows)
                        //{
                        //    /*
                        //     *  - Prepate the required data to send 
                        //     * */


                        //    Task.Run(() => Proxy.SendReport(dRow));

                        //}

                    }

                    else
                    {
                        LogWriter.WriteErrorLog("No new Reports to send");

                    }

                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);
                }

                ////Getting the data to send    
            }

        }
        public static void GetReportDataOle4()
        {

            string dbConStr = "" +
                                " Provider = MSDAORA.1; Data Source = (DESCRIPTION = (ADDRESS_LIST = "
                    + "(ADDRESS=(PROTOCOL=TCP)(HOST=" + DBServer + ")(PORT=1521)))"
                    + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));"
                    + "User Id=smsuser; Password=sms;";

            using (OleDbConnection conn = new OleDbConnection(dbConStr)) // C#
            {


                try
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "SELECT * FROM p_report WHERE (trans_flg <> '1' OR trans_flg IS NULL) AND ROWNUM < 200 ORDER BY op_serial ASC ";
                    //  cmd.CommandType = CommandType.Text;
                    OleDbDataAdapter dr = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        /*
                         *   - Two things to send : 
                         *   - The Notif plain
                         *   - The Test results JSON
                         *  
                         */
                        LogWriter.WriteErrorLog("Start to send jsreports  ");
                        Task.Run(() => Proxy.SendReport(dt));


                    }

                    else
                    {
                        LogWriter.WriteErrorLog("No new Reports to send");

                    }

                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);
                }
            }
        }

        public static void GetReportDataOle()
        {


            string dbConStr = "" +
                     " Provider = MSDAORA.1; Data Source = (DESCRIPTION = (ADDRESS_LIST = "
         + "(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.3)(PORT=1521)))"
         + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));"
         + "User Id=smsuser; Password=sms;";

            using (OleDbConnection conn = new OleDbConnection(dbConStr)) // C#
            {


                try
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;
                    //Get the new Un sent msgs
                    cmd.CommandText = "select * from p_report WHERE (trans_flg <> '1' OR trans_flg IS NULL ) AND ROWNUM < 100 ORDER BY op_serial ASC";
                    //  cmd.CommandType = CommandType.Text;
                    OleDbDataAdapter dr = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    dr.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        /*
                         *   - Two things to send : 
                         *   - The Notif plain
                         *   - The Test results JSON
                         *  
                         */
                        LogWriter.WriteErrorLog("Start to send reports ");

                        foreach (DataRow dRow in dt.Rows)
                        {
                            /*
                             *  - Prepate the required data to send 
                             * */


                            Task.Run(() => Proxy.SendReport(dRow));

                        }

                    }

                    else
                    {
                        LogWriter.WriteErrorLog("No new Reports to send");

                    }

                }
                catch (Exception ex)
                {
                    LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);
                }

                ////Getting the data to send    
            }

        }



        public static void UpdateSentMsgs(string [] msgsIDs)
        {
            if (msgsIDs!=null&&msgsIDs.Length>0) {


                string inCondition = " IN (";
                int idCnt = 0;
                foreach (string id in msgsIDs)
                {
                    if (idCnt == 0)
                        inCondition += id;
                    else
                        inCondition += "," + id;
                    idCnt++;
                }
                inCondition += ")";


                using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
                {

                    try
                    {
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;

                        cmd.CommandText = "UPDATE " + TableName + " SET " + SentFlgCol + " =1 WHERE " + PrmryCol + " " + inCondition;
                        int rspns= cmd.ExecuteNonQuery();

                        LogWriter.WriteErrorLog("Response returned from update Stmnt : "+rspns);
                    }


                    catch (Exception ex)
                    {
                        LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);

                    }

                    ////Getting the data to send 

                }
            }

        }



        public static void UpdateSentMsg(string msgID)
        {
            if (msgID != null && msgID!="")
            {
             using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
                {
                    try
                    {
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE " + TableName + " SET " + SentFlgCol + " =1 WHERE " + PrmryCol + " =  " +msgID ;
                      
                        int rspns = cmd.ExecuteNonQuery();
                        LogWriter.WriteErrorLog("Sent Notif Successfully updated : " );
                    }

                    catch (Exception ex)
                    {
                        LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);

                    }

                    ////Getting the data to send 

                }
            }

        }

        public static void UpdateSentRpt(string opSerial)
        {
            if (opSerial != null && opSerial != "")
            {
                using (OracleConnection conn = new OracleConnection(ConnectionString)) // C#
                {
                    try
                    {
                        conn.Open();
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE p_report SET trans_flg =1 WHERE op_serial =  "+ opSerial.Trim()+"";
                        int rspns = cmd.ExecuteNonQuery();

                       // if(rspns==1)
                     //  LogWriter.WriteErrorLog("Sent Rpt Successfully updated : "+rspns);
                    }

                    catch (Exception ex)
                    {
                        LogWriter.WriteErrorLog("Couldn't connect to the DB Server !" + ex.Message);

                    }

                    ////Getting the data to send 

                }
            }

        }
    }
}
