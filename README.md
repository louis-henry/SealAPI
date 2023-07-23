# SealAPI
A development API designed for a technical interview. This project should be used alongside the SealUI project.

## How to Run
- Ensure you have Visual Studio 2022 install with the .NET framework (this project uses .NET 6 so Visual Studio 2019 or older will not work)
- Open the solution file in Visual Studio. 
- Restore NuGet packages (if required)
- Run project

## More Information
- If using a customised url/port, you will need to udpate the url reference in the SealUI project in (src/Constants/Config.tsx)
- In the appsettings.json file, you can define a value for the download link expiry time. The amount should be in minutes and can be found in the "Config" section ("LinkExpiryMins")
