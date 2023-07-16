using Google.Protobuf.WellKnownTypes;

namespace APIAttempt3
{
    public class WeatherReport
    {
        public DateTime? Timestamp { get; set; } //changed from TimeStamp type

        public string? City { get; set; }

        public string? ZipCode { get; set; }

        public string? Type_Of_Severe_Weather { get; set; }

        public int Severity { get; set; }

        public string? Notes { get; set; }
    }
}
