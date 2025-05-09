# SDAConsole Project Documentation

## Overview

SDAConsole stands for "Software Download Assistant Console" is a .NET Console application designed to reduce the complexity and time investment when creating Test Objects definition files, particulary when it comes to the assigment of the right/applicable CoMo expressions. This document provides guidelines for setting up, running, and contributing to the project.

## Project Structure

```
SDAConsole/
│── Program.cs                  # Main entry point of the application
│  │── GenerateTestObject.cs                  # Contains clases to Generate a Test Object JSON file
│── SDAConsole.csproj          # Project configuration file
├── nuget.config                # NUGET configuration that Connectos with the internal Volvo artifacts Feed
├── README.md                   # Project documentation (this file)

SDAConsole.Tests/              # Unit tests for the application
├── UnitTest1.cs                # Example test file
├── SDAConsole.Tests.csproj    # Test project file
├── nuget.config                # NUGET configuration that Connectos with the internal Volvo artifacts Feed


```

## Prerequisites

- .NET SDK (Latest version) [Download Here](https://dotnet.microsoft.com/en-us/download)
- Visual Studio Code or any preferred IDE
- NUnit & Moq for testing

## Setup Instructions

### Clone the Repository

```sh
git clone <repository-url>
cd SDAConsole
```

### Run the Application

```sh
dotnet run
```

### Run the Tests

Navigate to the test project:

```sh
cd SDAConsole.Tests
```

Run the test suite:

```sh
dotnet test
```

## Contribution Guidelines

1. **Fork the Repository**
2. **Create a New Branch**
   ```sh
   git checkout -b feature-branch
   ```
3. **Make Changes and Test**
4. **Commit and Push**
   ```sh
   git commit -m "Describe changes"
   git push origin feature-branch
   ```
5. **Create a Pull Request**

## Mock Data for Offline Testing

Since this project does not connect to real vehicles, mock data is used in unit tests:

- `Moq` is used to simulate vehicle responses
- Example mock:
  ```csharp
  var mockVehicle = new Mock<IVehicle>();
  mockVehicle.Setup(v => v.GetVin()).Returns("123456789ABCDEFG");
  ```

## Build Executable

To generate an .exe file:

You can publish it as a Self-Contained Executable:

- Run the following in your terminal:

```sh
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true
   ```

This creates a single-file executable for Windows 64-bit in:

```
bin\Release\net7.0\win-x64\publish\SDAConsole.exe
```


- (Optional) Cross-Platform Build

    - For Linux/macOS support, replace win-x64 with:

        - Linux: linux-x64
        - MacOS: osx-x64

Example for Linux:

```sh
   dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
   ```




## Additional Resources

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [NUnit Documentation](https://nunit.org/)
- [Moq Documentation](https://github.com/moq/moq4)

---

Note: The VTS token has an expiration of 1 year starting on 2025-04-11 When required the VTS team will contact the acting responsible for updating it.

## Licensing Options

| Use Case               | License           | Notes                           |
|------------------------|-------------------|----------------------------------|
| Open-source projects   | GPLv3              | Must disclose source            |
| Internal business use  | Commercial         | Contact author for details      |
| Enterprise integration | Commercial + SLA   | Optional support & dev services |

## Contact

📧 aess.technologies@gmail.com
🔗 GitHub: https://github.com/CarlosVolvoCars/SDAConsole

Maintained by Carlos Luna.

