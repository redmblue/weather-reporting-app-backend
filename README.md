# Weather Reporting App Backend

This is the .NET C# backend for the Weather Reporting app found here: http://weather-report-system.s3-website.us-east-2.amazonaws.com/
This backend and MySQL database are hosted on Azure App Service in the same instance, while the frontend (Angular using Material and Tailwinds) is hosted in an AWS S3 Bucket.

Available endpoints:  
GET /api/WeatherReport/ - Returns an array of all of the WeatherReports, used for the initial loading of the webpage.  
GET /api/WeatherReport/GetWXReport/{timestamp} - Returns information about the WeatherReport at that timestamp. Timestamp format (UTC time): YYYY-MM-DDTHH\:MM:SSZ - the Z at the end just means UTC time but is required.  
POST /api/WeatherReport/Create - Adds the WeatherReport that you provide to the database.  
POST /api/WeatherReport/Update/{timestamp} - Updates the WeatherReport in the database at the timestamp provided with the WeatherReport that you provide.  
DELETE /api/WeatherReport/Delete/{timestamp} - Deletes the WeatherReport at that timestamp.  

WeatherReport Object:  
```
{  
  TimeStamp: DateTime,  
  City: string,  
  ZipCode: string,  
  Type_Of_Severe_Weather: string,  
  Severity: int,  
  Notes: string?  
}  
```

Frontend: https://github.com/redmblue/weather-reporting-app-frontend/

Backend URL: http://weatherreportapitodeploy20230531141202.azurewebsites.net/ (Need to use available endpoints listed above, such as http://weatherreportapitodeploy20230531141202.azurewebsites.net/api/WeatherReport/)
