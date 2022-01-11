<div id="top"></div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#before-you-begin">Before You Begin</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



## Before You Begin

The project contains a file called DummyConstants.cs and there is a reason behind the naming. All the project secrets are in this file and this is only for the purposes of this task. All project secrets should of course be read from environment variables during production or any external file outside source control in development. 

<p align="right">(<a href="#top">back to top</a>)</p>



### Built With

* [.NET 5](https://dotnet.microsoft.com)
* [SQL Server Docker image](https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple example steps.

### Prerequisites

Download .Net 5 SDK and a SQL Server docker image if you don't have one already.

### Installation


1. Clone the repo
   ```sh
   git clone https://github.com/hesham-elbahey/vending-machine.git
   ```
2. Install dotnet tool dotnet-ef using .NET CLI
   ```sh
   dotnet tool install --global dotnet-ef
   ```
3. From within the repository directory, open FlapKapVendingMachine/DummyConstants.cs
A constant named ConnectionString has all the data required to authenticate to a local database.

4. Open SQL Server and run the following series of queries
    ```sql
    USE master;
    GO
    CREATE DATABASE VendingMachine;
    GO
    CREATE LOGIN [machineUser] WITH PASSWORD=N'Admin#123';
    GO
    USE VendingMachine;
    GO
    CREATE USER [machineUser] FOR LOGIN [machineUser];
    GO
    ALTER ROLE [db_owner] ADD MEMBER [machineUser];
    GO
    ```

5. Again from within the repository directory
   ```sh
   cd FlapKapVendingMachine
   ```
   ```sh
   dotnet ef database update
   ```
6. Now that everything is ready, run the following command:
    ```sh
    dotnet run
    ```
    The application will be listening on http://localhost:5000. [Navigate there](http://localhost:5000/index.html) to view Swagger UI for the project.
    
<p align="right">(<a href="#top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

_For a full documentation, feel free to try out the endpoints using Swagger UI_

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- CONTACT -->
## Contact

Hesham Elbahey - [LinkedIn](https://www.linkedin.com/in/heshamelbahey/) - hesham.elbahey@gmail.com

Project Link: [https://github.com/hesham-elbahey/vending-machine.git](https://github.com/hesham-elbahey/vending-machine.git)

<p align="right">(<a href="#top">back to top</a>)</p>
