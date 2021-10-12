using Notifier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NotifierService
{
	public partial class NotifierShchd : ServiceBase
	{ 
		private Timer timer1 = null;
		public NotifierShchd()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			/*
			 * -- What to do inside the Service :
				 - Check if the DB Server Connected
				 - Check if there is a new notification msgs
				 - check if the gateway connected 
				 - Send the messages 
				 - when get the response 
				 - update the msgs statuess
			 * 
			 * */
			LogWriter.CleanLog();
			DBConnection.GetConfigurationData();
			timer1 = new Timer();
			this.timer1.Interval = 80000;
			timer1.Enabled = true;
			Library.WriteErrorLog(" Service started!");
			/*
			 *  - Update Configuration Data
			 * */
			

			this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
		}

		private void timer1_Tick(object sender,ElapsedEventArgs e)
		{

			DBConnection.GetReportDataOle3();
			/*
			 *  - Connect to The DB server and check if there is new messages
			 *  
			 * */

			DBConnection.Connect();

			/*
			 * 
			 * */
		


		}
		protected override void OnStop()
		{
			timer1.Enabled = false;
			Library.WriteErrorLog("The windows service has stopped!");
		}
	}
}
