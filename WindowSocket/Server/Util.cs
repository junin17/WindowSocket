﻿using System;
using System.IO;
using System.Text;

namespace Server
{
    public class Util
	{
		/// <summary>
		/// Write Log
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="text"></param>
		public static void WriteOnLog(string fileName, string text)
		{
			try
			{
				if (Properties.Settings.Default.LogOn)
				{
					using (StreamWriter file = new StreamWriter(FileName(fileName), true))
					{

						file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + text);
					}

					
				}
			}
			catch (Exception ex)
			{
				using (StreamWriter file = new StreamWriter(FileName("LOG_ERROR"), true))
				{
					file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + text);
					file.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": " + ex.ToString());
				}
			}


		}

        /// <summary>
        /// Sets file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
		private static string FileName(string fileName)
		{
			var builder = new StringBuilder();
			builder.Append(AppDomain.CurrentDomain.BaseDirectory);
            builder.Append(@"\");
            builder.Append("Log");

            if (!Directory.Exists(builder.ToString()))
            {
                Directory.CreateDirectory(builder.ToString());
            }

			builder.Append(@"\");
			builder.Append(fileName);
			builder.Append("_");
			builder.Append(DateTime.Now.ToString("yyyyMMdd"));
			builder.Append(".txt");

			return builder.ToString();
		}
	}
}
