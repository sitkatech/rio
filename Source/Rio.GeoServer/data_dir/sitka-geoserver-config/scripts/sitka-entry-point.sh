#!/bin/bash
set -e

echo "running $0"

set -o nounset # Don't allow using uninitialized variables
set -o verbose # Echo out what each command is that is run so we have a good tracing log
set -o errexit # Exit immediately if any commands return non-zero exit code

##### BEGIN geoserver-environment.properties SETUP #####

# 04/06/20 MF & JVV - In local environment on first load sometimes we get "cannot create regular file", sleep before copying seems to help
sleep 5 

# copy the template file for replacement 
cp -f "${GEOSERVER_DATA_DIR}/geoserver-environment.properties.template" "${GEOSERVER_DATA_DIR}/geoserver-environment.properties"

FIND="${GEOSERVER_SQL_SERVER_PASSWORD_SECRET}"
REPLACE=$(cat "${GEOSERVER_SQL_SERVER_PASSWORD_SECRET}")
# the //&/\\& stuff is to escape any ampersands in the passwords...
sed -i "s|${FIND}|${REPLACE//&/\\&}|g" ${GEOSERVER_DATA_DIR}/geoserver-environment.properties
##### END geoserver-environment.properties SETUP #####


##### BEGIN sitka-update-passwords.sh #####
if [ ${GEOSERVER_ADMIN_PASSWORD_SECRET} ]; then
    chmod +x "${GEOSERVER_DATA_DIR}/sitka-geoserver-config/scripts/sitka-update-passwords.sh"
	"${GEOSERVER_DATA_DIR}/sitka-geoserver-config/scripts/sitka-update-passwords.sh"
fi;
##### END sitka-update-passwords.sh #####


##### BEGIN SETTING UP TOMCAT WEBAPPS FOLDER #####
# setup the tomcat webapps root folder with files
cp -R "${GEOSERVER_DATA_DIR}/sitka-geoserver-config/tomcat/webapps/." /usr/local/tomcat/webapps
##### END SETTING UP TOMCAT WEBAPPS FOLDER #####

##### BEGIN ENABLE CORS
ENABLE_CORS=$(cat "${GEOSERVER_DATA_DIR}/sitka-geoserver-config/tomcat/enable_cors.txt")
perl -i -pe "s|<param-value>If-Modified-Since,Range,Origin</param-value>|${ENABLE_CORS}|g" /usr/local/tomcat/conf/web.xml
##### END ENABLE CORS

##### BEGIN SETTING TOMCAT HTTP TO HTTPS REDIRECT #####
if [[ ${SSL} =~ [Tt][Rr][Uu][Ee] ]]; then
    REDIRECT_REPLACE=$(cat "${GEOSERVER_DATA_DIR}/sitka-geoserver-config/tomcat/redirect_replace.txt")
    perl -i -pe "s|</web-app>|${REDIRECT_REPLACE}|g" /usr/local/tomcat/conf/web.xml
fi;
##### END SETTING TOMCAT HTTP TO HTTPS REDIRECT #####

exec /scripts/entrypoint.sh