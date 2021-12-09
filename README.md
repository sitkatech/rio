Welcome to the Water Accounting Platform. The platform is designed to meet these objectives:
- Help water managers achieve groundwater sustainability goals and minimize economic costs to the community.
- Over the longer term, consider opportunities to generate additional benefits from fallowed land such as groundwater recharge basins and wildlife habitat.
- Exploring water trading policies, including reducing water trading transaction costs and facilitate effective accounting and management of available water resources.

This project is a collaborative effort by the Rosedale-Rio Bravo Water Storage District (https://www.rrbwsd.com), Environmental Defense Fund (https://www.edf.org), Westwater Research (http://www.waterexchange.com), and Sitka Technology Group (https://sitkatech.com). 

Please contact support@sitkatech.com with any questions about this project.

# Developer Set-up Instructions
## Prerequisites

1. Install Visual Studio 2019
2. Install Visual Studio Code
3. Install Docker
4. Install Node.js
5. Install these extensions for Visual Studio Code:

- Angular CLI
- Debugger for Chrome
- Docker
- npm support for VS Code
- npm commands for VS Code
- npm Intellisense

## Rio Set-up (API)

1. Clone the git repository to your development machine
2. Create an empty database called RioDB and create a user for it. Give the user the owner role.
3. Copy [repo root dir]\Source\docker-compose\.env.template to [repo root dir]\Source\docker-compose\.env
4. Update the values in the new .env file
5. Add an entry to your hosts file pointing RIO_WEB_URL from the .env file to 127.0.0.1
6. Open the solution in VS19 and set docker-compose as the startup project
7. Press the green "play" triangle to start the API server

## Rio Set-up (Web)
1. Open the rio-web-workspace in VSC ([repo root dir]\Source\Rio.Web)
2. Open the VSC terminal and run npm install, then npm build, then npm start.
3. Press F5 to open the web app in Google Chrome. You will be able to debug JavaScript directly in VSC.
