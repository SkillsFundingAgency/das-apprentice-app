## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprentice App

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">



The Apprentice App provides apprentices a platform to track their apprenticeship progress and access support and guidance articles to aid them throughout their journey.

## How It Works

The Apprentice App is a Progressive Web App in ASP.NET Core wrapped in a native MAUI wrapper. To run this locally, users need to set up two databases - SFA.DAS.ApprenticeAccounts.Database and SFA.DAS.Courses.Database with dummy data.
Users need to have these APIs running locally or in an azure tenant in parallel to this app:
* SFA-DAS-ApprenticeAccounts-Api 
* SFA-DAS-APIM
* SFA-DAS-Courses-Api

## 🚀 Installation

### Pre-Requisites


```
* A clone of this repository
* A code editor that supports .NET8
* SQL Server Management Studio
* Azure Table Explorer
* Azurite

```
### Config

AppSettings.Development.json file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
  "ConfigNames": "SFA.DAS.ApprenticeApp.Pwa",
  "EnvironmentName": "LOCAL",
  "Version": "1.0"
}
```

Azure Table Storage config

Row Key: SFA.DAS.ApprenticeApp.Pwa_1.0

Partition Key: LOCAL

Data:

```json
{
  "AzureAd": {
    "tenant": "<tenantdetails>",
    "identifier": "<identifierurl>"
  },
  "ResourceEnvironmentName": "LOCAL",
  "StubAuth": "true",
  "ApprenticeAppApimApi": {
    "ApiBaseUrl": "https://localhost:5123/",
    "ApiVersion": "1",
    "SubscriptionKey": "key"
  }
}
```
Users should have Azurite running in the background when launching this app locally

## 🔗 External Dependencies


```
This App uses
* SFA-DAS-ApprenticeAccounts-Api 
* SFA-DAS-APIM
* SFA-DAS-Courses-Api

You will need to follow set up instructions for all of these repos in order to run the apprentice app locally

```

## Technologies

```
* ASP.NET CORE
* PWA
* .NET MAUI
* Moq
* NUnit
* FluentAssertions

```

## 🐛 Known Issues


```
* None
```
