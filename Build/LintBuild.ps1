
"Download Linter Docker Image"
docker pull github/super-linter:latest

"Run Linter"
docker run -e RUN_LOCAL=true -e VALIDATE_POWERSHELL=true -e VALIDATE_DOCKERFILE_HADOLINT=true -e VALIDATE_TERRAFORM_TFLINT=true -v c:\git\qanat:/tmp/lint github/super-linter
#docker run -e RUN_LOCAL=true -e VALIDATE_POWERSHELL=true -e VALIDATE_YAML=true -e VALIDATE_DOCKERFILE_HADOLINT=true -e VALIDATE_TERRAFORM_TFLINT=true -v c:\git\qanat:/tmp/lint github/super-linter



