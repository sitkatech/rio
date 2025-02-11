
"Download Linter Docker Image"
docker pull github/super-linter:latest

"Run Linter"
docker run -e RUN_LOCAL=true -e VALIDATE_CSS=true -e VALIDATE_TYPESCRIPT_ES=true -e VALIDATE_JAVASCRIPT_ES=true -e VALIDATE_HTML=true -v c:\git\qanat:/tmp/lint github/super-linter

#docker run -e RUN_LOCAL=true -e VALIDATE_POWERSHELL=true -v c:\git\qanat:/tmp/lint github/super-linter


