# ESD Assessment - Covid data
Author: Eve Ragins

## Tech Stack

### Web
Vue SPA, built with the vue cli.  It is transpiled and hosted as as static site in Azure Storage.

### Covid Data Source
https://about-corona.net/documentation

This seemed like the best solution to easily get meaningful statistics and generate a basic website.  
Additionally, by accessing a 3rd-party resource that's actively maintained, 
there is no chance of stale data or the need to worry about a data synchronization process.

### API
C# Azure Functions

A serverless approach seemed light-weight and a good option for just a couple end-points.

### Database
Azure CosmosDB, another serverless option.

I actually initally wanted to use Azure Table Storage (serverless, and inexpensive), 
but it seems to be largely deprecated in favor of Cosmos Tables.  Since I was "forced" into
using Cosmos anyway, I opted for the SQL option knowing that I wanted to be able to sort by values 
(not something a key-value pair solution lends itself to).  Cosmos is unfortunately more expensive,
but with the serverless option, it's less expensive and appears to be lighter-weight 
than an MS-SQL solution.

Schema is a single table:

```
Queries

QueryText (string)
NumTimesHit (int)
LastUpdated (DateTime)
```

## Developing

### Website

See EsdCovid.Web\readme.md

### Functions

Locally, you must have Azure Storage Emulator installed.

Set the startup project to be functions, and debug as "normal".  You can execute the functions via the browser, a commandline script (ex. curl), or something like Postman.

## Deployment

CI/CD is done with GitHub Actions.  GitHub Actions uses a yaml-based pipeline not unlike azure pipelines.  They can be found in .github/workflows.

Deployment is completely automated the web and "api" layer.

Azure resources were set up and configured manually.  Resources include:
* Azure Functions
* A storage account for Azure Functions  
* A storage account configured with a static site.  (trying to put both in the same storage account broke deployments for some reason)
* A CosmosDB

_Note: Azure DevOps recently changed their pricing model and it no longer seems possible to run pipelines without setting up a paid plan or emailing somebody and waiting several days._

### Utility Scripts

Generate Functions publish profile

> az webapp deployment list-publishing-profiles --name esdcovid-functions --resource-group ESD-Assessment --subscription [subscriptionid] --xml