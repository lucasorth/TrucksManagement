# Trucks Management
Web App to manage trucks with CRUD features and unit test as a requirements to selection process of a company.
## Prerequisites
Make sure you have installed the following prerequisites on your development machine:
* .NET SDKs- [Download & Install .NET SDKs](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks). If you have Visual Studio 2019, may be it alrealy installed.
* Visual Studio 2019 - [Visual Studio Download Page](https://visualstudio.microsoft.com/pt-br/downloads/). You can use another source code editor if you like. I chose this one.
* Nuget packages will be restored automatically when you build solution.

## Solution layout
### Trucks.WebApp
This is a ASP.NET Core MVC project with EF Core. This project contains code that builds a web application to manage trucks.
### Trucks.Test
This project builds a xUnit test, that uses to assert about WebApp requirements.

## Building the App
Once the project runs, a local SQL database will be created named **TrucksDb**. Make sure this database name is available.
After that, you can access database through SQL Server Management Studio using *(localdb)\\mssqllocaldb* host.

### Running Tests
Open Visual Studio, than open Trucks.Test context menu, and hit **Run Tests**, or open *Test Explorer* to run by selection. It creates a SQL InMemory database instead of in *localdb*.

## Features
This software is able to do:

- [x] View a list of trucks
- [x] Create a new truck
- [x] Edit an exists truck
- [x] View details of truck, like user who created and date of creation
- [x] Delete a truck with confirmation option
