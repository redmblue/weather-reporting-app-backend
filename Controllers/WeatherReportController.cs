using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Signers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace APIAttempt3.Controllers
{
    [ApiController]
    public class WeatherReportController : Controller
    {


        private readonly IConfiguration _configuration;
        private MySqlConnection rdsDb;
        public WeatherReportController(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionstring = $"server={_configuration["DbUrl"]};user={_configuration["DbUsername"]};database=weather_reports;port=3306;password={_configuration["DbPassword"]}"; 
            rdsDb = new MySqlConnection(connectionstring);


            try
            {
                System.Diagnostics.Debug.WriteLine("Connecting to MySQL...");
                rdsDb.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            System.Diagnostics.Debug.WriteLine("Done.");
        }
        // GET: WeatherReportController/
        [HttpGet("/api/[controller]")]
        public async Task<ActionResult<WeatherReport>> GetAllReports()
        {
            List<WeatherReport> reportList = new List<WeatherReport>();
            var command = new MySqlCommand("SELECT * FROM wxreports",rdsDb);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                string? string_to_deserialize = "";
                int i = 0;
                DateTime? timestamp_date_time = null;
                while (i <= 5)
                {
                    try
                    {
                        if (reader.GetString(i) != null) { }
                    }
                    catch
                    {
                        break;
                    }
                    switch (i)
                    {
                        case 0:
                            string_to_deserialize += "{\n\"TimeStamp\":";
                            timestamp_date_time = reader.GetDateTime(i);
                            break;
                        case 1:
                            string_to_deserialize += "\"City\":";
                            break;
                        case 2:
                            string_to_deserialize += "\"ZipCode\":";
                            break;
                        case 3:
                            string_to_deserialize += "\"Type_Of_Severe_Weather\":";
                            break;
                        case 4:
                            string_to_deserialize += "\"Severity\":";
                            break;
                        case 5:
                            string_to_deserialize += "\"Notes\":";
                            break;
                    }
                    if (i != 4 && i != 0)
                        string_to_deserialize += " " + "\"" + reader.GetString(i) + "\",\n";
                    else if (i == 0)
                        string_to_deserialize += " " + "\"" + reader.GetDateTime(i) + "\",\n";
                    else if (i == 4)
                        string_to_deserialize += " " + reader.GetString(i) + ",\n";
                    i++;
                }
                string_to_deserialize = string_to_deserialize.Substring(0, string_to_deserialize.Length - 2);
                string_to_deserialize += "\n}";
                System.Diagnostics.Debug.WriteLine(string_to_deserialize);
                var report_to_add = System.Text.Json.JsonSerializer.Deserialize<WeatherReport>(string_to_deserialize);
                report_to_add.Timestamp = timestamp_date_time;
                reportList.Add(report_to_add);

            }
            return Ok(reportList);
        }
        //conversion https://localhost:7071/api/WeatherReport/GetWXReport/2023-05-30T04%3A13%3A50Z from 2023-05-30T04:13:50Z timestamp/DateTime
        [HttpGet("/api/[controller]/GetWXReport/{timestamp}")]
        public async Task<ActionResult<WeatherReport>> GetWeatherReport(string timestamp)
        {
            List<WeatherReport> reportList = new List<WeatherReport>();
            var command = new MySqlCommand("SELECT * FROM wxreports WHERE time_stamp = @timeStamp", rdsDb);
            command.Parameters.AddWithValue("@timeStamp", timestamp);
            var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) //should only be one
            {
                string? string_to_deserialize = "";
                int i = 0;
                DateTime? timestamp_date_time = null;
                while (i <= 5)
                {
                    try
                    {
                        if (reader.GetString(i) != null) { }
                    }
                    catch
                    {
                        break;
                    }
                    switch (i)
                    {
                        case 0:
                            string_to_deserialize += "{\n\"TimeStamp\":";
                            timestamp_date_time = reader.GetDateTime(i);
                            break;
                        case 1:
                            string_to_deserialize += "\"City\":";
                            break;
                        case 2:
                            string_to_deserialize += "\"ZipCode\":";
                            break;
                        case 3:
                            string_to_deserialize += "\"Type_Of_Severe_Weather\":";
                            break;
                        case 4:
                            string_to_deserialize += "\"Severity\":";
                            break;
                        case 5:
                            string_to_deserialize += "\"Notes\":";
                            break;
                    }
                    if (i != 4 && i != 0)
                        string_to_deserialize += " " + "\"" + reader.GetString(i) + "\",\n";
                    else if (i == 0)
                        string_to_deserialize += " " + "\"" + reader.GetDateTime(i) + "\",\n"; //still null for some reason.
                    else if (i == 4)
                        string_to_deserialize += " " + reader.GetString(i) + ",\n";
                    i++;
                }
                string_to_deserialize = string_to_deserialize.Substring(0, string_to_deserialize.Length - 2);
                string_to_deserialize += "\n}";
                System.Diagnostics.Debug.WriteLine(string_to_deserialize);
                var report_to_add = System.Text.Json.JsonSerializer.Deserialize<WeatherReport>(string_to_deserialize); //reader.GetString(0)
                report_to_add.Timestamp = timestamp_date_time;
                reportList.Add(report_to_add);

            }
            if (reportList.Count == 0)
            {
                return NotFound();
            }
            else if(reportList.Count > 1) { 
                return Ok(reportList);
            }
            return Ok(reportList[0]); 
        }


        // POST: WeatherReportController/add
        [HttpPost("/api/[controller]/Create")]
        public async Task<ActionResult<WeatherReport>> CreateWeatherReport(WeatherReport weather_report_to_add)
        {
            System.Diagnostics.Debug.WriteLine("yay");
            MySqlCommand? current_command = new MySqlCommand("");
            try
            {
                if (weather_report_to_add == null)
                {
                    return BadRequest("Error: no weather report");
                }
                else if (weather_report_to_add.Notes != null)
                {
                    var command = new MySqlCommand("INSERT INTO wxreports (time_stamp,city,zip_code,type_of_severe_weather,severity,notes) VALUES (current_timestamp(),@city,@zip_code,@type,@severity,@notes)", rdsDb); //can use current_timestamp()
                    command.Parameters.AddWithValue("@city", weather_report_to_add.City);
                    command.Parameters.AddWithValue("@zip_code", weather_report_to_add.ZipCode);
                    command.Parameters.AddWithValue("@type", weather_report_to_add.Type_Of_Severe_Weather);
                    command.Parameters.AddWithValue("@severity", weather_report_to_add.Severity);
                    command.Parameters.AddWithValue("@notes", weather_report_to_add.Notes);
                    current_command = command;

                }
                else if (weather_report_to_add.Notes == null)
                {
                    var command = new MySqlCommand("INSERT INTO wxreports (time_stamp,city,zip_code,type_of_severe_weather,severity) VALUES (current_timestamp(),@city,@zip_code,@type,@severity)", rdsDb);
                    command.Parameters.AddWithValue("@city", weather_report_to_add.City);
                    command.Parameters.AddWithValue("@zip_code", weather_report_to_add.ZipCode);
                    command.Parameters.AddWithValue("@type", weather_report_to_add.Type_Of_Severe_Weather);
                    command.Parameters.AddWithValue("@severity", weather_report_to_add.Severity);
                    current_command = command;
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error: your weather report to be added object isn't formatted correctly.");
            }
            await current_command.ExecuteNonQueryAsync();
            return CreatedAtAction(nameof(GetWeatherReport), new { timestamp = weather_report_to_add.Timestamp }, weather_report_to_add); 
        }


        [HttpPost("/api/[controller]/Update/{timestamp}")]
        public async Task<ActionResult<WeatherReport>> UpdateWeatherReport(string timestamp, WeatherReport weather_report_to_update)
        {
            var command = new MySqlCommand("", rdsDb);
            if (weather_report_to_update.Notes != null)
            {
                command = new MySqlCommand("UPDATE wxreports SET city=@city, zip_code=@zip_code, type_of_severe_weather=@type, severity=@severity, notes=@notes WHERE time_stamp=STR_TO_DATE(@time_stamp, '%Y-%m-%dT%TZ')", rdsDb);
                command.Parameters.AddWithValue("@city", weather_report_to_update.City);
                command.Parameters.AddWithValue("@zip_code", weather_report_to_update.ZipCode);
                command.Parameters.AddWithValue("@type", weather_report_to_update.Type_Of_Severe_Weather);
                command.Parameters.AddWithValue("@severity", weather_report_to_update.Severity);
                command.Parameters.AddWithValue("@notes", weather_report_to_update.Notes);
                command.Parameters.AddWithValue("@time_stamp", timestamp);
            }
            else
            {
                command = new MySqlCommand("UPDATE wxreports SET city=@city, zip_code=@zip_code, type_of_severe_weather=@type, severity=@severity WHERE time_stamp=STR_TO_DATE(@time_stamp, '%Y-%m-%dT%TZ')", rdsDb);
                command.Parameters.AddWithValue("@city", weather_report_to_update.City);
                command.Parameters.AddWithValue("@zip_code", weather_report_to_update.ZipCode);
                command.Parameters.AddWithValue("@type", weather_report_to_update.Type_Of_Severe_Weather);
                command.Parameters.AddWithValue("@severity", weather_report_to_update.Severity);
                command.Parameters.AddWithValue("@time_stamp", timestamp);
            }
            var rows_changed = await command.ExecuteNonQueryAsync();
            if(rows_changed != 0)
            {
                return NoContent();
            }
            return NotFound();
        }
        [HttpDelete("/api/[controller]/Delete/{timestamp}")]
        public async Task<IActionResult> DeleteUser(string timestamp)
        {
            var command = new MySqlCommand("DELETE FROM wxreports WHERE time_stamp=STR_TO_DATE(@time_stamp, '%Y-%m-%dT%TZ')", rdsDb);
            command.Parameters.AddWithValue("@time_stamp", timestamp);
            var rows_deleted = await command.ExecuteNonQueryAsync();
            if (rows_deleted!=0)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
