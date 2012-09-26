using System;
using System.Xml;
using System.Xml.Linq;


namespace YahooWeatherManager
{
	public class WeatherForecast
	{
		#region Attributes / Properties
		private string DayOfTheWeekDescription { get; set; }
		public int MaximumTemperature { get; set; }
		public int MinimumTemperature { get; set; }
		public string TemperatureDescription { get; set; }
		public DateTime Day { get; set; }
		public string WeatherDescription { get; set; }
		public string WeatherDescriptionCode { get; set; }
		public WeatherCode WeatherCode { get; set; }
		#endregion

		public WeatherForecast()
		{
			this.Day = DateTime.Now;
			this.DayOfTheWeekDescription = this.Day.DayOfWeek.ToString();
			this.MaximumTemperature = 0;
			this.MinimumTemperature = 0;
			this.TemperatureDescription = this.MaximumTemperature + "/" + this.MinimumTemperature;
			this.WeatherDescription = WeatherControl.NoDataResult;
			this.WeatherDescriptionCode = WeatherControl.NoCodeResult;
			this.WeatherCode = WeatherCode.Undefined;
		}
		
		public WeatherForecast(XNode xmlInformation)
		{
			string dateString = ((XElement)xmlInformation).Attribute("date").Value.ToString();
			int day = Convert.ToInt32(dateString.Split(' ')[0]);
			int month = DateTime.ParseExact(dateString.Split(' ')[1], "MMM", System.Globalization.CultureInfo.InvariantCulture).Month;
			int year = Convert.ToInt32(dateString.Split(' ')[2]);
			
			this.Day = new DateTime(year, month, day);
			this.DayOfTheWeekDescription = this.Day.DayOfWeek.ToString();
			this.MaximumTemperature = Convert.ToInt32(((XElement)xmlInformation).Attribute("high").Value.ToString());
			this.MinimumTemperature = Convert.ToInt32(((XElement)xmlInformation).Attribute("low").Value.ToString());
			this.TemperatureDescription = this.MaximumTemperature + "/" + this.MinimumTemperature;
			this.WeatherDescription = ((XElement)xmlInformation).Attribute("text").Value.ToString();
			this.WeatherDescriptionCode = ((XElement)xmlInformation).Attribute("code").Value.ToString();
			this.WeatherCode = (WeatherCode)Enum.Parse(typeof(WeatherCode), this.WeatherDescriptionCode);
		}
	}
}

