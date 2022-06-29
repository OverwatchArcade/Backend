<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
![](https://img.shields.io/github/stars/OverwatchArcade/Backend)
![](https://img.shields.io/github/issues/OverwatchArcade/Backend)
![](https://img.shields.io/github/workflow/status/overwatcharcade/backend/Main)

[![Maintainability](https://api.codeclimate.com/v1/badges/d2022cf80cb7eea655e6/maintainability)](https://codeclimate.com/github/OverwatchArcade/Backend/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/d2022cf80cb7eea655e6/test_coverage)](https://codeclimate.com/github/OverwatchArcade/Backend/test_coverage)


<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/OverwatchArcade/Backend">
    <img src="https://i.imgur.com/9vS4il3.jpg" alt="Logo" width="200" height="200"> 
  </a>

<h3 align="center">OverwatchArcade.Today Backend</h3>
  <p align="center">
    Discover daily Overwatch arcade gamemodes without having to login to Overwatch.
    <br />
    <a href="https://overwatcharcade.today/"><strong>View website »</strong></a>
    <br />
    <br />
    <a href="https://github.com/OverwatchArcade/Backend/projects/1">View Todo's</a>
    ·
    <a href="https://github.com/OverwatchArcade/Backend/issues">Report Bug</a>
    ·
    <a href="https://github.com/OverwatchArcade/Backend/issues">Request Feature</a>
  </p>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary><h2 style="display: inline-block">Table of Contents</h2></summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
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
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

<a href="https://github.com/OverwatchArcade/Backend">
<img src="https://i.imgur.com/5ubUysA.png" width="400">
</a>

Discover daily Overwatch arcade gamemodes without having to login to Overwatch. Daily submittions are posted by contributors (registered members).
OverwatchArcade.Today also offers an open API for developers to intergrate in their apps, Discord communities and etc.

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

### Prerequisites

- Discord OAuth2 App (Member registration whitelisting)
- Twitter OAuth2 App (Twitter posting)
- APIFlash account needed for (Screenshot service for Tweet media)
- Dotnet CLI (Run migration)
- Docker


### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/OverwatchArcade/Backend.git
   ```
2. Copy appsettings.json to appsettings.(Environment).json
   ```
   cp appsettings.json appsettings.Development.json
   ```
3. Create the hangfire database, the overwatch database can be created automatically by dotnet cli.
4. Run the dotnet cli *database update* command.
   ```
   dotnet ef database update
   ```
5. Whitelist your Discord ID in the Whitelist table and visit /login if you wish to login.



<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/OverwatchArcade/Backend/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the CC BY-NC-SA 4.0 License. See `LICENSE` for more information.


<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements

* [OWLib](https://github.com/overtools/OWLib)
* [The contributors who keep this project alive 💗](https://overwatcharcade.today/contributors)