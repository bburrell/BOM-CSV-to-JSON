using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;

namespace JSONBuilder
{
    public class JSONBuilder
    {
        public IEnumerable<KeyValuePair<DateTime, float?>> Data { get; set; }
        private WeatherDataContainer _weatherData { get; set; }

        public string BuildJson()
        {
            _weatherData = new WeatherDataContainer();
            var dateTimeFormatInfo = new DateTimeFormatInfo();

            var dataToProcess = Data.ToList();

            _weatherData.WeatherData = dataToProcess.Where(q => q.Value.HasValue).GroupBy(x => x.Key.Year)
                .Select(yearGrp =>
            {
                var yearWeatherData = new WeatherDataForYear();
                yearWeatherData.Year = yearGrp.Key;

                //var monthlyContainer = new MonthlyAggregatesContainer();
                //yearWeatherData.MonthlyAggregates = monthlyContainer;

                yearWeatherData.MonthlyAggregates =  yearGrp.GroupBy(p => dateTimeFormatInfo.GetMonthName(p.Key.Month))
                    .Select(monthGrp =>
                {
                    var monthWeatherDataContainer = new WeatherDataForMonthContainer();
                    monthWeatherDataContainer.WeatherDataForMonth = new WeatherDataForMonth();
                    
                    monthWeatherDataContainer.WeatherDataForMonth.Month = monthGrp.Key;
                    CalculateWeatherDataFor(monthGrp, monthWeatherDataContainer.WeatherDataForMonth);

                    return monthWeatherDataContainer;
                }).ToList();

                CalculateWeatherDataFor(yearWeatherData.MonthlyAggregates.Select(p => p.WeatherDataForMonth), yearWeatherData);


                var yearContainer = new WeatherDataForYearContainer();
                yearContainer.WeatherDataForYear = yearWeatherData;
                return yearContainer;
            }).ToList();

            //var root = new
            //{
            //    WeatherData = _weatherData
            //};

            string json = JsonConvert.SerializeObject(_weatherData, new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd",
                    Formatting = Formatting.Indented
                });

            return json;
        }

        private void CalculateWeatherDataFor(IEnumerable<KeyValuePair<DateTime, float?>> values, WeatherDataFor data)
        {
            data.FirstRecordedDate = values.Min(q => q.Key);
            data.LastRecordedDate = values.Max(q => q.Key);
            data.TotalRainfall = values.Sum(q => q.Value.Value);
            data.DaysWithRainfall = values.Where(q => q.Value.HasValue && q.Value != 0).Count();
            data.DaysWithNoRainfall = values.Where(q => !q.Value.HasValue || q.Value == 0).Count();
            data.AverageDailyRainfall = data.TotalRainfall / (data.DaysWithRainfall + data.DaysWithNoRainfall);
        }

        private void CalculateWeatherDataFor(IEnumerable<WeatherDataFor> values, WeatherDataFor data)
        {
            data.FirstRecordedDate = values.Min(q => q.FirstRecordedDate);
            data.LastRecordedDate = values.Max(q => q.LastRecordedDate);
            data.TotalRainfall = values.Sum(q => q.TotalRainfall);
            data.DaysWithRainfall = values.Sum(q => q.DaysWithRainfall);
            data.DaysWithNoRainfall = values.Sum(q => q.DaysWithNoRainfall);
            data.AverageDailyRainfall = data.TotalRainfall / (data.DaysWithRainfall + data.DaysWithNoRainfall);
        }
    }

    internal class WeatherDataContainer
    {
        public IEnumerable<WeatherDataForYearContainer> WeatherData { get; set; }
    }

    internal class WeatherDataForYearContainer
    {
        public WeatherDataForYear WeatherDataForYear { get; set; }
    }

    internal class WeatherDataFor
    {
        [JsonProperty(Order = 1)]
        public DateTime? FirstRecordedDate { get; set; }

        [JsonProperty(Order = 2)]
        public DateTime? LastRecordedDate { get; set; }

        [JsonProperty(Order = 3)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public float TotalRainfall { get; set; }

        [JsonProperty(Order = 4)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public float AverageDailyRainfall { get; set; }

        [JsonProperty(Order = 5)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public int DaysWithNoRainfall { get; set; }

        [JsonProperty(Order = 6)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public int DaysWithRainfall { get; set; }
    }

    internal class WeatherDataForYear : WeatherDataFor
    {
        [JsonProperty(Order = 1)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public int Year { get; set; }

        [JsonProperty(Order = 99)]
        public IEnumerable<WeatherDataForMonthContainer> MonthlyAggregates { get; set; }
    }

    internal class WeatherDataForMonthContainer
    {
        public WeatherDataForMonth WeatherDataForMonth { get; set; }
    }

    internal class WeatherDataForMonth : WeatherDataFor
    {
        [JsonProperty(Order = 1)]
        public string Month { get; set; }
    }


    public class ToStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
