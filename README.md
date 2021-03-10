![Datasilk Logo](http://www.markentingh.com/projects/datasilk/logo.png)

# Datasilk Core Template

This Github repository is a project template for an ASP.NET Core web site based on [Datasilk Core](http://github.com/Datasilk/Core), [Datasilk Core Js](http://github.com/Datasilk/CoreJs), [Selector](http://github.com/websilk/selector), and [Tapestry](http://github.com/Websilk/Tapestry). The website includes a user system, login page, and account dashboard. 

This project is meant to be forked and used as a starting point for developing large-scale, high-performing websites.

## Requirements

* Visual Studio 2019
* ASP.NET 5.0
* SQL Server 2017
* Node.js
* Gulp

## Installation

Since this project is a starting point for developing your own web applications, you wouldn't neccessarily want to fork this repository. Instead, you should clone the "bare" repository and push it to your own repository.
	
1. Clone the **bare** repository

```
git clone --bare http://github.com/datasilk/coretemplate YourProjectName
cd YourProjectName
```

> NOTE: replace `YourProjectName` with the name of your project

2. Push the cloned repository into your own repository & delete original cloned **folder**
	
```
git push --mirror http://github.com/YourAccount/YourRepository
cd ..
rm -rf YourProjectName
```

3. Clone **your** repository

```
git clone --recursive http://github.com/YourAccount/YourRepository
```

2. Replace all case-sensitive instances of `CoreTemplate` to `YourProjectName` and `coretemplate` to `yourprojectname` in all files within the repository
3. Rename file `CoreTemplate.sln` to `YourProjectName.sln` and file `App/CoreTemplate.csproj` to `App/YourProjectName.csproj`
2. Run command ```npm install```
3. Run command ```gulp default```
4. In Visual Studio, publish the SQL project to SQL Server 2017 (or greater), with your own database name
5. Open `config.json` and make sure the database connection string for property `SqlServerTrusted` points to your database.
6. Click Play in Visual Studio 2019


## Features
* **SQL Server project** includes tables & stored procedures for **Users** and **Security Groups**
* **Query project** uses [Dapper](http://github.com/StackExchange/Dapper) to populate C# models from SQL
* **App project** uses ASP.NET 5.0 and [Datasilk Core MVC](http://github.com/Datasilk/Core) to host web pages & web APIs.
	* When accessing website for the first time, you're able to create a new admin user account
    * Can log into user account at `/login` or from any secure page when access is denied
    * Redirects to `/dashboard` after user logs into account
    * Default web page for URL `/` is `App/Controllers/Home.cs`
    * Dashboard contains a sidebar with a menu system
    * UI provided by [Tapestry](http://github.com/Websilk/Tapestry), a **CSS/LESS** UI framework.
    * Javascript uses [Selector](http://github.com/Websilk/Selector) as a light-weight replacement for jQuery (under 5KB)
	* [Datasilk Core Js](http://github.com/Datasilk/CoreJs) is used as a simple **client-side framework** for structuring page-level Javascript code, making AJAX requests, and calling utility functions
	* Build **MVC** web pages using html files & **mustache variables**. For example:
     
**Example: HTML with mustache variables & blocks**
```
<html><body>
	<div class="menu">{{menu}}</div>
	{{has-sub-menu}}
		<div class="menu sub">{{sub-menu}}</div>
	{{/has-sub-menu}}
	<div class="body">{{content}}</div>
</body></html>
```

All above projects were concieved & developed by [Mark Entingh](http://www.markentingh.com), who has a strong passion for web development.




