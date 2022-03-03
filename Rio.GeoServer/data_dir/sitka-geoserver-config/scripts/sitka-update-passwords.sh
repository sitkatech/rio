#!/bin/bash
echo "running $0"

set -o nounset # Don't allow using uninitialized variables
set -o verbose # Echo out what each command is that is run so we have a good tracing log
set -o errexit # Exit immediately if any commands return non-zero exit code

# Credits https://github.com/geosolutions-it/docker-geoserver for this script that allows a user to pass a password
# or username on runtime.
# Sitka comments: we are running our own version of the update_passwords.sh script here that sets up the lockfile, so that 
# the default kartoza one will not run. This lets us set up our own admin password that follows our security standards
SETUP_LOCKFILE="${GEOSERVER_DATA_DIR}/.updatepassword.lock"

# We need to bootstrap the geoserver security folder, so that we can change the username and password
# Hopefully this is the right place in the container, we also found an identical security folder
# at /usr/local/tomcat/webapps/geoserver/data/security. There is some confusion reading the kartoza repository on this,
# might reach out to them

# We are *always* starting over with security folder because sometimes we get stuck in between no security and half a security folder and then Tomcat starts but Geoserver kills itself
# If we need to keep more security settings than the default plus our own username/password then revisit this code
#
# If you have a half baked "security" folder GeoServer will only partly startup then shut itself down, with this is log file:
# SEVERE [main] org.apache.catalina.core.StandardContext.startInternal One or more listeners failed to start. Full details will be found in the appropriate container log file,
# WARNING [main] org.apache.catalina.loader.WebappClassLoaderBase.clearReferencesThreads The web application [geoserver] appears to have started a thread named [FileSystemWatcher-1] but has failed to stop it. This is very likely to create a memory leak. Stack trace of thread:,
#
# We were getting this because when geoserver running, it locks two files in the "security" so that sometimes if you purge data_dir those two files are locked by GeoServer and don't get purged security\role\default\roles.xml, security\usergroup\default\users.xml
#
# c:\svn\sitkatech\trunk\ProjectFirma\Build>..\..\Build\mc.exe ls --recursive ProjectFirmaQA/projectfirmabucket/data_dir
#[2020-04-21 15:34:56 PDT]      0B gwc\tmp\
#[2020-04-21 15:41:57 PDT]      0B security\role\default\roles.xml
#[2020-04-21 15:41:37 PDT]      0B security\usergroup\default\users.xml
#[2020-04-21 15:34:56 PDT]      0B temp\wps\
#[2020-04-21 15:34:56 PDT]      0B tmp\
rm -r -f "${GEOSERVER_DATA_DIR}/security"
cp -r ${CATALINA_HOME}/data/security ${GEOSERVER_DATA_DIR}

GEOSERVER_ADMIN_USER=${GEOSERVER_ADMIN_USER}
GEOSERVER_ADMIN_PASSWORD=$(cat "${GEOSERVER_ADMIN_PASSWORD_SECRET}")
USERS_XML=${USERS_XML:-${GEOSERVER_DATA_DIR}/security/usergroup/default/users.xml}
ROLES_XML=${ROLES_XML:-${GEOSERVER_DATA_DIR}/security/role/default/roles.xml}
CLASSPATH=${CLASSPATH:-/usr/local/tomcat/webapps/geoserver/WEB-INF/lib/}

make_hash(){
    NEW_PASSWORD=$1
    (echo "digest1:" && java -classpath $(find $CLASSPATH -regex ".*jasypt-[0-9]\.[0-9]\.[0-9].*jar") org.jasypt.intf.cli.JasyptStringDigestCLI digest.sh algorithm=SHA-256 saltSizeBytes=16 iterations=100000 input="$NEW_PASSWORD" verbose=0) | tr -d '\n'
}

#users
PWD_HASH=$(make_hash $GEOSERVER_ADMIN_PASSWORD)
cp $USERS_XML $USERS_XML.orig

# <user enabled="true" name="admin" password="digest1:7/qC5lIvXIcOKcoQcCyQmPK8NCpsvbj6PcS/r3S7zqDEsIuBe731ZwpTtcSe9IiK"/>

cat $USERS_XML.orig | sed -e "s/ name=\".*\" / name=\"${GEOSERVER_ADMIN_USER}\" /" | sed -e "s/ password=\".*\"/ password=\"${PWD_HASH//\//\\/}\"/" > $USERS_XML

#roles
cp $ROLES_XML $ROLES_XML.orig

cat $ROLES_XML.orig | sed -e "s/ username=\".*\"/ username=\"${GEOSERVER_ADMIN_USER}\"/" > $ROLES_XML

# Put lock file to make sure password is not reinitialized on restart
touch ${SETUP_LOCKFILE}