using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

using GlobalEnglish.Utility.Diagnostics;

namespace PerformanceCounterTest
{
	/// <summary>
	/// Summary description for Installer1.
	/// </summary>
	[RunInstaller(true)]
	public class testInstaller : System.Configuration.Install.Installer
	{
		private System.Diagnostics.PerformanceCounterInstaller counterInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public testInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			//Debugger.Break();

			PerformanceCounterFactory category = new PerformanceCounterFactory(typeof(Test));
			// if the counter for a single category are spread across multiple classes use
			// category.AddCounters(typeof(AdditionalTestClass));
			// to add more classes

			// now add the counters to the performance counter installer
			counterInstaller.CategoryName = category.Name;
            counterInstaller.CategoryType = category.CategoryType;
            if (category.Help != null)
				counterInstaller.CategoryHelp = category.Help;
			counterInstaller.Counters.AddRange(category.Counters);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.counterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
			// 
			// testInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.counterInstaller});

		}
		#endregion
	}
}
