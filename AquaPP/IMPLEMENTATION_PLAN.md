# Smart CSV Import Feature: Implementation and Architectural Plan

## 1. Overview

This document outlines the plan for implementing a "Smart CSV Import" feature in the AquaPP application. This feature will provide users with pre-defined templates for water quality data and a guided workflow for importing custom CSV files, including intelligent column mapping and validation.

This plan also includes a strategy for refactoring the `DataEntryViewModel` to improve its maintainability and scalability by separating concerns.

## 2. Design Patterns and Architectural Principles

To ensure a robust, scalable, and maintainable implementation, we will adhere to the following principles:

*   **Single Responsibility Principle (SRP)**: We will break down the `DataEntryViewModel` into smaller, more focused components, each with a single responsibility.
*   **Strategy Pattern**: This pattern will be used to handle different aspects of the import process, such as data mapping and validation. This will allow us to easily add new mapping or validation strategies in the future.
*   **MVVM (Model-View-ViewModel)**: We will continue to follow the MVVM pattern to maintain a clean separation between the UI (View), the presentation logic (ViewModel), and the data (Model).
*   **Dependency Injection**: We will leverage dependency injection to provide services to our ViewModels, making them easier to test and maintain.

## 3. Refactoring the `DataEntryViewModel`

The current `DataEntryViewModel` handles data loading, saving, deleting, unit conversion, and CSV export. We will refactor this into a more modular structure:

1.  **`DataEntryViewModel` (The Coordinator)**: The primary role of this ViewModel will be to coordinate the different parts of the data entry page. It will hold the collection of readings and delegate tasks to specialized services.
2.  **`IDataManagementService`**: A new service will be created to encapsulate the core CRUD (Create, Read, Update, Delete) operations for `WaterQualityReading`. This will move the data loading, saving, and deletion logic out of the ViewModel.
3.  **`IUnitConversionService`**: This service already exists and is well-defined. We will continue to use it for all unit conversions.
4.  **`ICsvService`**: This will be a new service responsible for both exporting and importing CSV data. It will contain the logic for file parsing, data mapping, and validation.

This refactoring will result in a leaner, more focused `DataEntryViewModel` and a set of testable, reusable services.

## 4. Smart CSV Import Implementation Plan

### Step 1: Create the `ICsvService` and Supporting Models

*   Define an `ICsvService` interface with methods for `Import` and `Export`.
*   Create a `CsvMappingResult` model to hold the results of the column mapping process (e.g., mapped columns, unmapped columns, validation errors).
*   Implement a `CsvService` class that implements the `ICsvService` interface.

### Step 2: Implement the Column Mapping Logic

*   In the `CsvService`, create a method to read the CSV header.
*   Implement a mapping strategy that uses a dictionary of predefined column names and their variations (e.g., `{"pH", "ph", "potential of hydrogen"}`).
*   This strategy will return a `CsvMappingResult` indicating which columns were successfully mapped.

### Step 3: Build the Import UI and ViewModel Logic

*   Add an "Import" button to the `DataEntryView.axaml`.
*   Create a new `[RelayCommand]` in the `DataEntryViewModel` called `ImportCsvCommand`.
*   This command will:
    *   Use the `IFilePickerService` to let the user select a CSV file.
    *   Call the `ICsvService.Import` method.
    *   If the import is successful, it will display the data in the grid.
    *   If there are unmapped columns, it will trigger a dialog or a UI change to prompt the user for manual mapping.

### Step 4: Develop the Manual Column Mapping UI

*   We will create a custom `DataGrid` column header template.
*   For unmapped columns, this template will display a dropdown (`ComboBox`) containing the available `WaterQualityReading` properties and a "Custom" option.
*   When the user selects a property, the `DataEntryViewModel` will update the mapping and re-validate the data.

### Step 5: Finalize Data Validation and Saving

*   Once all columns are mapped and the user confirms, the `DataEntryViewModel` will perform a final validation of the data.
*   The validated data will then be saved to the database via the `IDataManagementService`.
