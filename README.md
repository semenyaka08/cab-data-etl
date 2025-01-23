# Cab Data ETL

## Description

The Cab Data ETL project is designed for extracting and loading (ETL) cab trip data. It is implemented using C# and SQL for data processing.

## Project Structure

*  **CabTripETL:** The main directory containing the project source code.

*  **CabTripETL.sln:** Visual Studio solution file for building and managing the project.

*  **.gitignore:** Specifies files and directories to be ignored by Git.

## Features

*   **Data Extraction:** Load cab trip data from csv file.
*   **Data Loading:** Save the processed data into a target database for further analysis.
*   **Data Indexing:** Index columns that are frequently queried to optimize search performance.


## Data Statistics

*   **Number of unique records in the table:** 29,840

## Requirements

*   **Programming Language:** C#
*   **Database:** SQL Server

## Installation

1.  Clone the repository:

    ```bash
    git clone https://github.com/semenyaka08/cab-data-etl.git
    ```

2.  Open the `CabTripETL.sln` file.
3.  Configure the database connection in the appsettings.json configuration file.
4.  Build and run the project.

## Usage

Once started, the project will execute the ETL process, which includes:

*   Loading cab trip data.
*   Cleaning and transforming the data.
*   Saving the processed data into the database.

## Important Considerations for Large Datasets:

If the program is expected to handle a 10GB CSV input file, the following changes are essential:

*   **Batch Processing:** Instead of loading and processing the entire file at once, implement batch processing to handle smaller subsets of data. This will prevent memory overflow and increase reliability.

---
