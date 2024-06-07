
variable "keyVaultName" {
  type = string
}

variable "storageAccountName" {
  type = string
}

variable "resourceGroupName" {
  type = string
}

variable "sqlUsername" {
  type = string
}

variable "sqlPassword" {
  type = string
}

variable "databaseName" {
  type = string
}

variable "dbServerName" {
  type = string
}

variable "databaseEdition" {
  type = string
}

variable "databaseTier" {
  type = string
}

variable "aspNetEnvironment" {
	type = string
}

variable "environment" {
  type = string
}

variable "azureClusterResourceGroup" {
  type = string
}

variable "databaseResourceGroup" {
  type = string
}

variable "sqlApiUsername" {
  type = string
}

variable "datadogApiKey" {
  type = string
  sensitive = true
}

variable "datadogAppKey" {
  type = string
  sensitive = true
}

variable "domainApi" {
  type = string
}

variable "domainWeb" {
  type = string
}

variable "domainGeoserver" {
  type = string
}

variable "elasticPoolName" {
  type = string
}

variable "sqlGeoserverUsername" {
  type = string
}

variable "team" {
  type = string
}

variable "projectNumber" {
  type = string
}

// this variable is used for the keepers for the random resources https://registry.terraform.io/providers/hashicorp/random/latest/docs
variable "amd_id" {
  type = string
  sensitive = false
  default = "1"
}

terraform {
	required_version   = ">= 0.11"
	backend "azurerm" {
		container_name          = "terraform"
		key                     = "terraform.tfstate"
	} 
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.91.0"
    }
    random = {
      source = "hashicorp/random"
      version = "~> 3.2.0"
    }
    datadog = {
      source = "DataDog/datadog"
    }
  }
}

# Configure the Azure Provider
provider "azurerm" {
  features {}
}

# Configure the Datadog provider
provider "datadog" {
  api_key = var.datadogApiKey
  app_key = var.datadogAppKey
}

data "azurerm_client_config" "current" {}


locals {
  tags = {
    "managed"       = "terraformed"
    "environment"   = var.environment
    "team"          = var.team
    "projectNumber" = var.projectNumber
  }
}


resource "azurerm_resource_group" "web" {
	name                         = var.resourceGroupName
  location                     = "West US"
  tags                         = local.tags
}


#blob storage
resource "azurerm_storage_account" "web" {
	name                         = var.storageAccountName
	resource_group_name          = azurerm_resource_group.web.name
	location                     = azurerm_resource_group.web.location
  account_replication_type	 	 = "GRS"
	account_tier								 = "Standard"
	tags                         = local.tags
}

output "application_storage_account_key" {
  sensitive = true
  value = azurerm_storage_account.web.primary_access_key
}

# the SAS token which is needed for the geoserver file transfer
data "azurerm_storage_account_sas" "web" {
  connection_string = azurerm_storage_account.web.primary_connection_string
  https_only        = true

  resource_types {
    service   = true
    container = true
    object    = true
  }

  services {
    blob  = true
    queue = false
    table = false
    file  = true
  }

  start  = timestamp()
  expiry = timeadd(timestamp(), "24h")

  permissions {
    read    = true
    write   = true
    delete  = true
    list    = true
    add     = true
    create  = true
    update  = true
    process = true
    tag     = false
    filter  = false
  }
}

# can be used in pipeline like $(TF_OUT_STORAGE_ACCOUNT_SAS_KEY)
output "storage_account_sas_key" {
  sensitive = true
  value = data.azurerm_storage_account_sas.web.sas
}

resource "azurerm_storage_share" "web" {
  name                 = "geoserver"
  storage_account_name = azurerm_storage_account.web.name
  quota                = 10 //10gb
}

#sql
data "azurerm_mssql_server" "spoke" {
  name                = var.dbServerName
  resource_group_name = var.databaseResourceGroup
}

data "azurerm_mssql_elasticpool" "spoke" {
  name                = var.elasticPoolName
  resource_group_name = var.databaseResourceGroup
  server_name         = var.dbServerName
}

resource "azurerm_mssql_database" "database" {
	name            = var.databaseName
  server_id       = data.azurerm_mssql_server.spoke.id
  collation       = "SQL_Latin1_General_CP1_CI_AS"
  license_type    = "LicenseIncluded"
  max_size_gb     = 2
  read_scale      = false
  sku_name        = var.databaseTier
  zone_redundant  = false
  elastic_pool_id = data.azurerm_mssql_elasticpool.spoke.id

  long_term_retention_policy {
    weekly_retention  = "P3M"
    monthly_retention = "P1Y"
    yearly_retention  = "P3Y"
    week_of_year      = 7
  }

  short_term_retention_policy {
    retention_days = 30
  }

  tags            = local.tags
}

output "database_id" {
  value = azurerm_mssql_database.database.id
}

### BEGIN API Sql user/login ###
resource "random_password" "sqlApiPassword" {
  length           = 16
  special          = true
  override_special = "!+-"
  min_lower        = 3
  min_upper        = 3
  min_special      = 3
  min_numeric      = 3
  keepers = {
    amd_id = var.amd_id
  }
}

output "sql_api_password" {
  sensitive = true
  value = random_password.sqlApiPassword.result
  depends_on = [
    random_password.sqlApiPassword
  ]
}

### END API Sql user/login ###

### BEGIN Geoserver Sql user/login ###
resource "random_password" "geoserverAdminPassword" {
  length           = 16
  special          = true
  override_special = "!+-"
  min_lower        = 3
  min_upper        = 3
  min_special      = 3
  min_numeric      = 3
  keepers = {
    amd_id = var.amd_id
  }
}

output "geoserver_admin_password" {
  sensitive = true
  value = random_password.geoserverAdminPassword.result
  depends_on = [
    random_password.geoserverAdminPassword
  ]
}


resource "random_password" "sqlGeoserverPassword" {
  length           = 16
  special          = true
  override_special = "!+-"
  min_lower        = 3
  min_upper        = 3
  min_special      = 3
  min_numeric      = 3
  keepers = {
    amd_id = var.amd_id
  }
}

output "sql_geoserver_password" {
  sensitive = true
  value = random_password.sqlGeoserverPassword.result
  depends_on = [
    random_password.sqlGeoserverPassword
  ]
}

### END Geoserver Sql user/login ###

### BEGIN Hangfire password ###
resource "random_password" "hangfirePassword" {
  length           = 16
  special          = true
  override_special = "!*-_"
  min_special      = 1
  min_lower        = 1
  min_upper        = 1
  min_numeric      = 1
  keepers = {
    amd_id = var.amd_id
  }
}

output "hangfire_password" {
  sensitive = true
  value = random_password.hangfirePassword.result
  depends_on = [
    random_password.hangfirePassword
  ]
}
### END Hangfire password ###

#key vault was created prior to terraform run
resource "azurerm_key_vault" "web" {
  name                         = var.keyVaultName
	location                     = azurerm_resource_group.web.location
 
  resource_group_name          = azurerm_resource_group.web.name
	soft_delete_retention_days   = 7
  purge_protection_enabled     = false
  tenant_id                    = data.azurerm_client_config.current.tenant_id
  tags                         = local.tags

  sku_name = "standard"
}

resource "azurerm_key_vault_access_policy" "thisPipeline" {
  key_vault_id = azurerm_key_vault.web.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  key_permissions = [
    "Backup", "Create", "Decrypt", "Delete", "Encrypt", "Get", "Import", "List", "Purge", "Recover", "Restore", "Sign", "UnwrapKey", "Update", "Verify", "WrapKey"
  ]

  secret_permissions = [
    "Backup", "Delete", "Get", "List", "Purge", "Recover", "Restore", "Set"
  ]

  storage_permissions = [
    "Backup", "Delete", "DeleteSAS", "Get", "GetSAS", "List", "ListSAS", "Recover", "RegenerateKey", "Restore", "Set", "SetSAS", "Update"
  ]
}

resource "azurerm_key_vault_secret" "sqlAdminPass" {
  name                         = "sqlAdministratorPassword"
  value                        = var.sqlPassword
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}
 
resource "azurerm_key_vault_secret" "sqlAdminUser" {
  name                         = "sqlAdministratorUsername"
  value                        = var.sqlUsername
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}
  
 resource "azurerm_key_vault_secret" "sqlApiUsername" {
   name                         = "sqlApiUsername"
   value                        = var.sqlApiUsername
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
   depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
 }

resource "azurerm_key_vault_secret" "sqlApiPassword" {
  name                         = "sqlApiPassword"
  value                        = random_password.sqlApiPassword.result
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "sqlApiConnectionString" {
  name                         = "sqlApiConnectionString"
  value                        = "Data Source=tcp:${data.azurerm_mssql_server.spoke.fully_qualified_domain_name},1433;Initial Catalog=${var.databaseName};Persist Security Info=True;User ID=${var.sqlApiUsername};Password=${random_password.sqlApiPassword.result}"
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "sqlGeoserverUsername" {
  name                         = "sqlGeoserverUsername"
  value                        = var.sqlGeoserverUsername
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "sqlGeoserverPassword" {
  name                         = "sqlGeoserverPassword"
  value                        = random_password.sqlGeoserverPassword.result
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "sqlGeoserverConnectionString" {
  name                         = "sqlGeoserverConnectionString"
  value                        = "Data Source=tcp:${data.azurerm_mssql_server.spoke.fully_qualified_domain_name},1433;Initial Catalog=${var.databaseName};Persist Security Info=True;User ID=${var.sqlGeoserverUsername};Password=${random_password.sqlGeoserverPassword.result}"
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "geoserverAdminPassword" {
  name                         = "geoserverAdminPassword"
  value                        = random_password.geoserverAdminPassword.result
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "azurerm_key_vault_secret" "hangfirePassword" {
  name                         = "hangfirePassword"
  value                        = random_password.hangfirePassword.result
  key_vault_id                 = azurerm_key_vault.web.id

  tags                         = local.tags
  depends_on = [
    azurerm_key_vault_access_policy.thisPipeline
  ]
}

resource "datadog_synthetics_test" "test_api" {
  type    = "api"
  subtype = "http"
  request_definition {
    method = "GET"
    url    = "https://${var.domainApi}"
  }
  request_headers = {
    Content-Type   = "application/json"
  }
  assertion {
    type     = "statusCode"
    operator = "is"
    target   = "200"
  }
  locations = ["aws:us-west-1","aws:us-east-1"]
  options_list {
    tick_every = 900

    retry {
      count    = 2
      interval = 30000
    }

    monitor_options {
      renotify_interval = 120
    }
  }
  name    = "${var.environment} - ${var.domainApi} API test"
  message = "Notify @rlee@esassoc.com @sgordon@esassoc.com @team-${var.team}${var.environment == "qa" ? "-qa" : ""}"
  tags    = ["env:${var.environment}", "managed:terraformed", "team:${var.team}"]

  status = "live"
}

resource "datadog_synthetics_test" "test_web" {
  type    = "api"
  subtype = "http"
  request_definition {
    method = "GET"
    url    = "https://${var.domainWeb}"
  }
  request_headers = {
    Content-Type   = "application/json"
  }
  assertion {
    type     = "statusCode"
    operator = "is"
    target   = "200"
  }
  locations = ["aws:us-west-1","aws:us-east-1"]
  options_list {
    tick_every = 900

    retry {
      count    = 2
      interval = 30000
    }

    monitor_options {
      renotify_interval = 120
    }
  }
  name    = "${var.environment} - ${var.domainWeb} Web test"
  message = "Notify @rlee@esassoc.com @sgordon@esassoc.com @team-${var.team}${var.environment == "qa" ? "-qa" : ""}"
  tags    = ["env:${var.environment}", "managed:terraformed", "team:${var.team}"]

  status = "live"
}

resource "datadog_synthetics_test" "test_geoserver" {
  type    = "api"
  subtype = "http"
  request_definition {
    method = "GET"
    url    = "https://${var.domainGeoserver}/geoserver/web/"
  }
  request_headers = {
    Content-Type   = "application/json"
  }
  assertion {
    type     = "statusCode"
    operator = "is"
    target   = "200"
  }
  locations = ["aws:us-west-1","aws:us-east-1"]
  options_list {
    tick_every = 900

    retry {
      count    = 2
      interval = 30000
    }

    monitor_options {
      renotify_interval = 120
    }
  }
  name    = "${var.environment} - ${var.domainGeoserver} Geoserver test"
  message = "Notify @rlee@esassoc.com @sgordon@esassoc.com @team-${var.team}${var.environment == "qa" ? "-qa" : ""}"
  tags    = ["env:${var.environment}", "managed:terraformed", "team:${var.team}"]

  status = "live"
}
