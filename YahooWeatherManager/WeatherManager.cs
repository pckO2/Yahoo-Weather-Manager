using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace YahooWeatherManager
{
	public class WeatherManager
	{
		private const string baseUrl = "http://xml.weather.yahoo.com/forecastrss/";
		private string zipcode;
		private bool isMetric;

		public WeatherResult Result { get; set; }

		/// <summary>
		/// Get weather information
		/// </summary>
		/// <param name="code">Zip code or location code (http://edg3.co.uk/snippets/weather-location-codes/spain/)</param>
		/// <param name="isMetric">metric when is outside the USA</param>
		public WeatherManager(string zipcode, bool isMetric)
		{
			this.Result = new WeatherResult();
			this.zipcode = zipcode;
			this.isMetric = isMetric;
		}

		/// <summary>
		/// Gets the weather.
		/// </summary>
		/// <returns>
		/// <c>true</c>, if weather was gotten, <c>false</c> otherwise.
		/// </returns>
		public bool GetWeather()
		{
			bool result = false;
			string forecastUrl = baseUrl + zipcode;
			if (this.isMetric)
			{
				forecastUrl +=  "_c.xml";
			}
			else
			{
				forecastUrl += "_f.xml";
			}
			
			//result like in http://xml.weather.yahoo.com/forecastrss/10021_f.xml (New York Upper East Side in Farenheit)

			try
			{
				XDocument weatherDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), null);

				using (XmlTextReader sr = new XmlTextReader(forecastUrl))
				{
					weatherDoc = XDocument.Load(sr);
				}
				
				if (weatherDoc != null)
				{
					//set a copy
					WeatherResult securityCopy = new WeatherResult();
					securityCopy.CopyWeatherResult(this.Result);

					//get needed Data
					XElement obCurrentConditions = weatherDoc.Descendants("channel").First();
					
					//get actual day and next days forecast
					XElement actualValuesAndForecast = weatherDoc.Descendants("item").First();

					//get actal day node information
					XNode actualValue = actualValuesAndForecast.Nodes().Where(desc => desc.ToString().Contains("yweather") && desc.ToString().Contains("condition") &&
					                                                          desc.ToString().Contains("text") && desc.ToString().Contains("temp")).FirstOrDefault();

					//set prediction
					this.Result.SetWeatherActualForecast(obCurrentConditions,actualValuesAndForecast);
					this.Result.SetWeatherNextForecast(actualValuesAndForecast);
					
					//check if has an error
					if (this.Result.PredictionHasError())
					{
						//revert from copy
						this.Result.CopyWeatherResult(securityCopy);
					}
					else //all correct
					{
						result = true;
					}
				}
				else
				{
					//log.Info("Weather data not retrieved properly.");
				}
				
			}
			catch (Exception ex)
			{
				//log.Error("Wheater Manager error. Reason: " + ex.Message);
			}

			return result;
		}
	}
}