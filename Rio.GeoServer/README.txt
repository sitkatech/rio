To install docker desktop
1. https://www.docker.com/products/docker-desktop
2. You will need to create an account with docker
	a. Docker account with a username such as "stewartgordonsitka'
3. Download and install for Windows


To start local docker container:
1. Make sure Docker Desktop is running	
2. Navigate to "C:\git\sitkatech\rio\Source\Rio.GeoServer" via CMD
3. Run "docker-compose -f docker-compose.yml up -d" (-d runs in detached mode)
4. Navigate to geoserver: http://127.0.0.1:8780/geoserver/

Notes:
* Before starting up your local docker container, it is a good idea to svn clean the data_dir directory
* Be careful when committing or releasing datastore.xml files with encrypted passwords in them, this may cause environments to not be able to connect to their database
* When developing in geoserver or adding new layers, try to avoid copying the workspace folders directly, this can cause multiple workspaces to have the same ID and cause them to be missing in geoserver


Release notes:
* Make sure that the following four connection parameters are present (among the other settings) in the datastore.xml files that are being committed:
<connectionParameters>
    <entry key="database">${datastore-database}</entry>
    <entry key="passwd">${datastore-password}</entry>
    <entry key="host">${datastore-host}</entry>
    <entry key="user">${datastore-user}</entry>
</connectionParameters>