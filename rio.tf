
variable "appInsightsName" {
  type = string
}

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

terraform {
	required_version   = ">= 0.11"
	backend "azurerm" {
		container_name          = "terraform"
		key                     = "terraform.tfstate"
	} 
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.46.0"
    }
  }
}

# Configure the Azure Provider
provider "azurerm" {
	# whilst the `version` attribute is optional, we recommend pinning to a given version of the Provider
  version = "=2.46.0"
  features {}
}


data "azurerm_client_config" "current" {}


locals {
  tags = {
    "managed"     = "terraformed"
    "environment" = var.aspNetEnvironment
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

# outputs like this will be set as pipeline variables
# in this case the pipeline will have access to "$(TF_OUT_APPLICATiON_STORAGE_ACCOUNT_KEY)"
# to make this happen, you can do this with your pipeline:
# - task: TerraformCLI@0
#   displayName: 'terraform output'
#   inputs:
#     command: output
output "application_storage_account_key" {
  sensitive = false
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
  }
}

# can be used in pipeline like $(TF_OUT_STORAGE_ACCOUNT_SAS_KEY)
output "storage_account_sas_key" {
  sensitive = false
  value = data.azurerm_storage_account_sas.web.sas
}

resource "azurerm_storage_share" "web" {
  name                 = "geoserver"
  storage_account_name = azurerm_storage_account.web.name
  quota                = 10 //10gb
}

#sql
resource "azurerm_sql_server" "web" {
  name                				 = var.dbServerName
	resource_group_name          = azurerm_resource_group.web.name
	location                     = azurerm_resource_group.web.location
	version                      = "12.0"
	administrator_login          = var.sqlUsername
	administrator_login_password = var.sqlPassword
	tags                         = local.tags
}

resource "azurerm_sql_firewall_rule" "test" {
	name                		     = "AccessToAzureFirewallRule"
	resource_group_name          = azurerm_resource_group.web.name
	server_name         		     = azurerm_sql_server.web.name
	start_ip_address    		     = "0.0.0.0"
	end_ip_address      		     = "0.0.0.0"
}

resource "azurerm_sql_database" "web" {
	name                               = var.databaseName
	resource_group_name                = azurerm_sql_server.web.resource_group_name
	location                           = azurerm_resource_group.web.location
	server_name                        = azurerm_sql_server.web.name
  max_size_bytes                     = ""
 
  edition                            = var.databaseEdition
  requested_service_objective_name   = var.databaseTier

	tags                               = local.tags
}

resource "azurerm_application_insights" "web" {
	name                         = var.appInsightsName
	resource_group_name          = azurerm_resource_group.web.name
	location                     = azurerm_resource_group.web.location
	application_type             = "web"
	tags                         = local.tags
}

output "instrumentation_key" {
	value = azurerm_application_insights.web.instrumentation_key
}


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

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "backup", "create", "decrypt", "delete", "encrypt", "get", "import", "list", "purge", "recover", "restore", "sign", "unwrapkey", "update", "verify", "wrapkey"
    ]

    secret_permissions = [
      "backup", "delete", "get", "list", "purge", "recover", "restore", "set"
    ]

    storage_permissions = [
      "backup", "delete", "deletesas", "get", "getsas", "list", "listsas", "recover", "regeneratekey", "restore", "set", "setsas", "update"
    ]
  }
}

resource "azurerm_key_vault_secret" "sqlAdminPass" {
   name                         = "sqlAdministratorPassword"
   value                        = var.sqlPassword
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
 }
 
 resource "azurerm_key_vault_secret" "sqlAdminUser" {
   name                         = "sqlAdministratorUsername"
   value                        = var.sqlUsername
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
 }
 
 resource "azurerm_key_vault_secret" "sqlAdminConnectionString" {
   name                         = "AdminConnectionString"
   value                        = "Data Source=tcp:${azurerm_sql_server.web.fully_qualified_domain_name},1433;Initial Catalog=${var.databaseName};Persist Security Info=True;User ID=${var.sqlUsername};Password=${var.sqlPassword}"
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
 }
 
 resource "azurerm_key_vault_secret" "sqlConnectionString" {
   name                         = "sqlConnectionString"
   value                        = "Data Source=tcp:${azurerm_sql_server.web.fully_qualified_domain_name},1433;Initial Catalog=${var.databaseName};Persist Security Info=True;User ID=${var.sqlUsername};Password=${var.sqlPassword}"
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
 }
 
 resource "azurerm_key_vault_secret" "appInsightsInstrumentationKey" {
   name                         = "appInsightsInstrumentationKey"
   value                        = azurerm_application_insights.web.instrumentation_key
   key_vault_id                 = azurerm_key_vault.web.id
 
   tags                         = local.tags
 }
