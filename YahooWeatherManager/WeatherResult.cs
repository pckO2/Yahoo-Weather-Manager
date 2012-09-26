using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace YahooWeatherManager
{

	public class WeatherResult
	{
		#region Attributes/Properties
		public string DayOfWeekDescription { get; set; }
		public string DayDescription { get; set; }
		public int CurrentTemperature { get; set; }
		public string SunriseDescription { get; set; }
		public string SunsetDescription { get; set; }
		public string WeatherDescription { get; set; }
		public string WeatherDescriptionCode { get; set; }
		public WeatherCode WeatherCode { get; set; }
		public WeatherForecast ActualForecast { get; set; }
		public ObservableCollection<WeatherForecast> NextForecasts { get; set; }
		public DateTime Day { get; set; }
		public DateTime Sunrise { get; set; }
		public DateTime Sunset { get; set; }
		#endregion

		public WeatherResult()
		{
			this.Day = DateTime.Now;
			this.DayOfWeekDescription = this.Day.DayOfWeek.ToString() + ",";
			this.DayDescription = this.Day.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture) + " " + this.Day.Day;
			this.CurrentTemperature = -100;
			this.Sunrise = DateTime.Now;
			this.Sunset = DateTime.Now;
			this.SunriseDescription = WeatherControl.NoDataResult;
			this.SunsetDescription = WeatherControl.NoDataResult;
			this.WeatherDescription = WeatherControl.NoDataResult;
			this.WeatherDescriptionCode = WeatherControl.NoCodeResult;
			this.WeatherCode = WeatherCode.Undefined;
			
			this.ActualForecast = new WeatherForecast();
			this.NextForecasts = new ObservableCollection<WeatherForecast>();
		}
		
		/// <summary>
		/// Copy one weather result to another
		/// </summary>
		/// <param name="copy"></param>
		public void CopyWeatherResult (WeatherResult copy)
		{
			try {
				this.Day = copy.Day;
				this.DayOfWeekDescription = copy.DayOfWeekDescription;
				this.CurrentTemperature = copy.CurrentTemperature;
				this.Sunrise = copy.Sunrise;
				this.Sunset = copy.Sunset;
				this.SunriseDescription = copy.SunriseDescription;
				this.SunsetDescription = copy.SunsetDescription;
				this.WeatherDescription = copy.WeatherDescription;
				this.WeatherDescriptionCode = copy.WeatherDescriptionCode;
				this.WeatherCode = copy.WeatherCode;
			
				this.ActualForecast = copy.ActualForecast;
				this.NextForecasts = new ObservableCollection<WeatherForecast> ();
				foreach (WeatherForecast forecast in copy.NextForecasts) { //or use AddRange()
					this.NextForecasts.Add (forecast); 
				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		/// <summary>
		/// Sets the weather actual forecast.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if weather actual forecast was set, <c>false</c> otherwise.
		/// </returns>
		/// <param name='obCurrentConditions'>
		/// Ob current conditions.
		/// </param>
		public bool SetWeatherActualForecast (XElement obCurrentConditions, XNode actualValue)
		{
			DateTime today = DateTime.Now;
			bool result = true;
			try {
				//get sunset and sunrise
				XNode timeInfoNode = obCurrentConditions.Nodes ().Where (desc => desc.ToString ().Contains ("sunrise") && desc.ToString ().Contains ("sunset")).FirstOrDefault ();
				this.Day = new DateTime (today.Year, today.Month, today.Day);
				this.DayOfWeekDescription = this.Day.DayOfWeek.ToString () + ",";
				this.DayDescription = this.Day.ToString ("MMMM", System.Globalization.CultureInfo.InvariantCulture) + " " + this.Day.Day;
				this.SunriseDescription = ((XElement)timeInfoNode).Attribute ("sunrise").Value.ToString ();
				this.SunsetDescription = ((XElement)timeInfoNode).Attribute ("sunset").Value.ToString ();
				//decode sunrise and sunset
				this.Sunrise = WeatherControl.GetDay (today, this.SunriseDescription);
				this.Sunset = WeatherControl.GetDay (today, this.SunsetDescription);

				//get the rest of the data from the next node
				this.CurrentTemperature = Convert.ToInt32 (((XElement)actualValue).Attribute ("temp").Value.ToString ());
				this.WeatherDescription = ((XElement)actualValue).Attribute ("text").Value.ToString ();
				this.WeatherDescriptionCode = ((XElement)actualValue).Attribute ("code").Value.ToString ();
				this.WeatherCode = (WeatherCode)Enum.Parse (typeof(WeatherCode), this.WeatherDescriptionCode);
			} catch (Exception ex) {
				//log.Error("Error in SetWeatherActualForecast. Reason: " +ex.Message);
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Sets the weather next days forecast.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if weather next forecast was set, <c>false</c> otherwise.
		/// </returns>
		/// <param name='actualValuesAndForecast'>
		/// Actual values and forecast.
		/// </param>
		public bool SetWeatherNextForecast (XElement actualValuesAndForecast)
		{
			DateTime today = DateTime.Now;
			DayOfWeek dayName = today.DayOfWeek;
			bool isFirstCorrected = false;
			bool result = true;

			try {

				//now next days forecast
				List<XNode> nodeForecastAux = actualValuesAndForecast.Nodes ().Where (desc => desc.ToString ().Contains ("forecast") && desc.ToString ().Contains ("day")).ToList ();
				List<XNode> nodeForecast = new List<XNode> ();
				XNode actualDayForecastInfo = null;

				for (int i = 0; i < nodeForecastAux.Count; i++) {
					if (isFirstCorrected) {
						nodeForecast.Add (nodeForecastAux [i]);
					} else {
						string codeDay = ((XElement)nodeForecastAux [i]).Attribute ("day").Value.ToString ().ToUpper ();
						DayOfWeek nodeDayName = WeatherControl.GetDayOfWeek (codeDay.ToUpper ());
					
						//check if the first day is today (it should be)
						if (nodeDayName == dayName) {
							isFirstCorrected = true;
							actualDayForecastInfo = nodeForecastAux[i];
						}
					}
				}

				if(actualDayForecastInfo != null)
				{
					this.ActualForecast = new WeatherForecast (actualDayForecastInfo);
				}
				else
				{
					this.ActualForecast = new WeatherForecast ();
				}

				//we made the next days predictions
				if (nodeForecast.Count > 0) {
					this.NextForecasts.Clear ();
					foreach(XNode prediction in nodeForecast)
					{
						this.NextForecasts.Add (new WeatherForecast (prediction));
					}
				} else { //error, clear result
					this.NextForecasts.Clear ();
					result = false;
				}
			} catch (Exception ex) {
				//log.Error("Error in SetWeatherNextForecast. Reason: " +ex.Message);
				result = false;
			}
		
			return result;
		}

		/// <summary>
		/// Check it the current prediction has error.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if no error was predicted, <c>false</c> otherwise.
		/// </returns>
		public bool PredictionHasError ()
		{
			if (this.WeatherDescription == WeatherControl.NoDataResult && 
				this.WeatherDescriptionCode == WeatherControl.NoCodeResult &&
				this.WeatherCode == WeatherCode.Undefined && 
				this.ActualForecast.WeatherCode == WeatherCode.Undefined)
				return true;
			return false;
		}
	}
}

