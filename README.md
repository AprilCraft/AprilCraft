# AprilCraft Website

Welcome to the official repository for AprilCraft, a design agency website. This project is developed using Blazor Server and is designed to read files from a folder and display them in a gallery showcase.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Overview

AprilCraft is a design agency specializing in crafting visual content that brings your ideas to life. This repository contains the source code for the AprilCraft website, built using Blazor Server. The website reads files from a designated folder and presents them in a visually appealing gallery for showcasing our design works.

## Key Features

- **Blazor Server:** Utilizes Blazor Server to provide a rich, interactive web experience.
- **Gallery Showcase:** Automatically reads files from a folder and displays them in a gallery format.
- **Responsive Design:** Ensures the website looks great on both desktop and mobile devices.
- **Easy Customization:** Simple to customize and extend the functionality as needed.

## Installation

### Prerequisites

- .NET 7.0 or later
- Visual Studio 2019 or later

### Steps

1. Clone the repository:
   ```sh
   git clone https://github.com/AprilCraft/AprilCraft.git
   cd AprilCraft
   ```

2. Open the solution in Visual Studio:
   ```sh
   AprilCraft.sln
   ```

3. Restore the NuGet packages:
   ```sh
   dotnet restore
   ```

4. Run the project:
   - Set `AprilCraft` as the startup project.
   - Press `F5` or click on the "Start" button in Visual Studio to run the project.

## Usage

Once the project is running, the website will be available at `http://localhost:5000` (or a different port if specified). The homepage will display a gallery that reads and showcases files from a designated folder.

### Customizing the Gallery Folder

1. Locate the folder path configuration in `appsettings.json`:
   ```json
   {
     "GalleryFolderPath": "wwwroot/images/gallery"
   }
   ```

2. Update the `GalleryFolderPath` value to the path where your gallery images are stored.

3. Add your images to the specified folder, and they will be automatically displayed in the gallery.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or new features.

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes.
4. Push your branch and create a pull request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
