# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

#login to azure 
Write-Host "Login to Azure"
az login 

#set parameters 
$subscriptionId = Read-Host "subscription Id"
$resource_group = Read-Host "resource group name"
$resource_name = Read-Host "unique name"
$synapseUser = Read-Host "username for synapse" 
$password = Read-Host "password for synapse"
$synapsepassword = ConvertTo-SecureString $password -AsPlainText -Force
$user = New-Object System.Management.Automation.PSCredential ($synapseUser, $synapsepassword)
$location = Read-Host "location"

Write-Host "creating resource group"
az group create --location $location --name $resource_group --subscription $subscriptionId 

$workspace_name = $resource_name + "ws"
$storage_account = $resource_name + "storage"
$file_system = $resource_name + "container"

Write-Host "creating data lake storage account"
az storage account create --name $storage_account --resource-group $resource_group --enable-hierarchical-namespace true

Write-Host "creating container in storage account"
az storage container create --name $file_system --account-name $storage_account

Write-Host "installing the Azure CLI for Azure Synapse"
Install-Module Az.Synapse
Import-Module Az.Synapse

Write-Host "creating synapse workspace" 
New-AzSynapseWorkspace -ResourceGroupName $resource_group -Name $workspace_name -Location $location -DefaultDataLakeStorageAccountName $storage_account -DefaultDataLakeStorageFilesystem $file_system -SqlAdministratorLoginCredential $user

$spark_pool = $resource_name + "sp"
 
Write-Host "creating Spark pool"
New-AzSynapseSparkPool -WorkspaceName $workspace_name -Name $spark_pool -NodeCount 3 -SparkVersion 2.4 -NodeSize Medium


$file_path = Read-Host "Local location of the Kaggle Datasets"
Write-Host "uploading the dataset to the storage account"
az storage blob upload-batch --destination $file_system --source $file_path --account-name $storage_account --destination-path "synapse/workspaces/"

$cosmos_db_account_name = Read-Host "Azure Cosmos DB Desired Account Name"
Write-Host "Creating the Azure Cosmos DB"
az cosmosdb create --name $cosmos_db_account_name --resource-group $resource_group