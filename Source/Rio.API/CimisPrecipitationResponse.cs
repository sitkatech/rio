using System;
using System.Collections.Generic;

namespace Rio.API
{
    public class CimisPrecipitationResponse
    {
        public ResponseData Data { get; set; }
    }

    public class ResponseData
    {
        public List<Provider> Providers { get; set; }
    }

    public class Provider
    {
        public List<Record> Records { get; set; }
    }

    public class Record
    {
        public DateTime Date { get; set; }
        public Precipitation DayPrecip { get; set; }
    }

    public class Precipitation
    {
        public decimal? Value { get; set; }
    }

}