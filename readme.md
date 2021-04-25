# ESD Assessment - Covid data
Author: Eve Ragins

## Summary

A mobile-first web site for viewing covid statistics by country.

### Checklist

-[x] shows covid-19 stats
-[x] is a website
-[x] mobile friendly
-[x] user friendly
-[x] user can query (via an autocomplete filter)
-[x] user can view query history
-[x] uses ci/cd
-[x] uses git
-[x] uses azure devops
-[x] front-end of my choice (vue)
-[x] C# backend
-[x] has a database
-[x] schema "exposed" below 
-[ ] has unit tests
* Web: Could hypothetically extract some of the filter/map functions or do component level testing, but it's a simple UI and easy to test.
* C#: Very little worth unit testing -- it's a thin layer between HTTP and CosmosDB and 80% of the test code would just be mocks. Not much value there.
-[ ] has integration tests
* There would be a lot more value here than unit tests.  Automation could be done with something like selenium, cypress, etc.  Good test cases would be:
    * Load site, assert elements appear and have data.
    * Load site, click on country name, assert country view data appears as expected
    * Load site, assert top queries are shown
    * (not ideal for production) Load site, enter data in autocomplete, choose option, assert switched to Country view
    * (not for production, ever) Clear history, load site, assert when value entered in autocomplete, then the value shows in top queries
* From a Functions standpoint, good tests (not for production) would be:
    * clear data, save a query, call CommonQueries and assert exists
    * clear data, save 6 different queries, 5 of them twice (for 11 calls total), call CommonQueries, assert that the 1 with only 1 hit is not returned

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

id (string)
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