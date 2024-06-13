## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprentice App

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

_Update these badges with the correct information for this project. These give the status of the project at a glance and also sign-post developers to the appropriate resources they will need to get up and running_

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/_projectname_?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=_projectid_&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=564&projectKey=_projectKey_)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/_pageurl_)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)



The Apprentice App provides apprentices a platform to track their apprenticeship progress and access support and guidance articles to aid them throughout their journey.

## How It Works

The Apprentice App is a Progressive Web App in ASP.NET Core wrapped in a native MAUI wrapper. To run this locally, users need to set up two databases - SFA.DAS.ApprenticeAccounts.Database and SFA.DAS.Courses.Database with dummy data.
Users need to have these repos running locally:
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

_Add details of the configuration required to successfully run the project. Adding in the config structure from the das-employer-config repo will help new developers understand what the config looks like and detailing the row keys and partition keys of any config rows will make it obvious where the config needs to be for the project to find it. Adding any further config which does not live in das-employer-config will also assist new developers to get the project running._

```
TODO

* A connection string for either the Apprenticeship Services ASB namespace or a namespace you own for development
* A CosmosDB connection string for either the Apprenticeship Service instance CosmosDB or a CosmosDB you own for development (you can use the emulator)
* Configure the [das-audit](https://github.com/SkillsFundingAgency/das-audit) project as per [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-audit/SFA.DAS.AuditApiClient.json)
* Add an appsettings.Development.json file
    * Add your connection strings for CosmosDB and ASB to the relevant sections of the file
* The CosmosDB will be created automatically if it does not already exist and the credentials you are connected with have the appropriate rights within the Azure tenant otherwise it will need to be created manually using the details in the config below under `CosmosDbSettings`.
```
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
* 
* 
```

## 🐛 Known Issues

_Add any known issues with the project_

_For Example_

```
* Fails when built under VS2019
```