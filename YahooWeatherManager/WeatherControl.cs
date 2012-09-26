using System;

namespace YahooWeatherManager
{
	public static class WeatherControl
	{
		public const string NoDataResult = "no Data";
		public const string NoCodeResult = "no Code";

		/// <summary>
		/// Gets the day of week with yahoo code
		/// </summary>
		/// <returns>
		/// The day of week.
		/// </returns>
		/// <param name='codeDay'>
		/// Code day.
		/// </param>
		public static DayOfWeek GetDayOfWeek (string codeDay)
		{
			DayOfWeek nodeDayName = DayOfWeek.Monday;
			
			if (codeDay == "MON") {
				nodeDayName = DayOfWeek.Monday;
			} else if (codeDay == "TUE") {
				nodeDayName = DayOfWeek.Tuesday;
			} else if (codeDay == "WED") {
				nodeDayName = DayOfWeek.Wednesday;
			} else if (codeDay == "THU") {
				nodeDayName = DayOfWeek.Thursday;
			} else if (codeDay == "FRI") {
				nodeDayName = DayOfWeek.Friday;
			} else if (codeDay == "SAT") {
				nodeDayName = DayOfWeek.Saturday;
			} else if (codeDay == "SUN") {
				nodeDayName = DayOfWeek.Sunday;
			}
		}
		
		/// <summary>
		/// Decodes day string to DatetTime
		/// </summary>
		/// <param name="today"></param>
		/// <param name="dayDescription"></param>
		/// <returns></returns>
		public static DateTime GetDay(DateTime today, string dayDescription)
		{
			DateTime day = new DateTime(today.Year, today.Month, today.Day,0,0,0);
			string numericTime = dayDescription.Replace("am", "").Replace("pm", "").Trim();
			
			day = day.AddHours(Convert.ToInt32(numericTime.Split(':')[0]));
			day = day.AddMinutes(Convert.ToInt32(numericTime.Split(':')[1]));
			
			if (dayDescription.Contains("pm"))
			{
				day = day.AddHours(12);
			}
			
			return day;
		}
	}
}

