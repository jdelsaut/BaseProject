#!/bin/bash
#
# Execute this directly in Azure Cli Task by passing thoses arguments in the Argument text area
# Must run on LINUX AGENT !
#
#   curl -sL https://git.io/slathrop-az-aks | bash -s <namingPrefix> <suiteName> <adspPwd> [<location>]
#
#     [Required]  ${1}  <param_FUNCTION_APP_NAME> Name of the Function App that will be reading the KeyVault
#     [Required]  ${2}  <param_MICROSERVICE_RESOURCE_GROUP> Name of the ResourceGroup of the Function App and the CosmosDb
#     [Required]  ${3}  <param_KEYVAULT_NAME> Name of the KeyVault where the secret will be stored
#     [Optional]  ${4}  <param_KEYVAULT_RG> Name of the ResourceGroup of the KeyVault
#     [Optional]  ${5}  <param_COSMOSDB_NAME> Name of the CosmosDB associated with the Function App
#     [Optional]  ${6}  <param_MASTER_CONNECTIONSTRING_SECRET_NAME>  Secret name of the Master database connection string inside Keyvault
#     [Optional]  ${7}  <param_SERVICE_BUS_NAMESPACE_NAME>  The service bus namespace nameconnection string inside Keyvault
#     [Optional]  ${8}  <param_ASS_RESPONSE_TOPIC_NAME> Name of the service bus topic used by BaseProject microservice retrieve Assignment response
#     [Optional]  ${9}  <param_SAS_RESPONSE_TOPIC_NAME> Name of the service bus topic used by BaseProject microservice retrieve SearchAndSelect response
#     [Optional]  ${10} <param_MISSION_CREATION_TOPIC_NAME> Name of the service bus topic used by BaseProject microservice to send mission creation request to FollowUp microservice
#     [Optional]  ${11} <param_FWU_RESPONSE_TOPIC_NAME> Name of the service bus topic used by BaseProject microservice to listen followup responses

#Setting KeyVault Policies for AzureDevOps Agents + Azure function
spID=$(az resource show --resource-group ${2} --name ${1} --resource-type "Microsoft.Web/sites" --query identity.principalId --out tsv)

az keyvault set-policy --name "${3}" --resource-group "${4}" --object-id "$spID" --secret-permissions get
az keyvault set-policy --name "${3}" --resource-group "${4}" --spn $servicePrincipalId --secret-permissions get set

# Setting Azure Keyvault Secret : CosmosDB Primary Key
cosmosDbKey=$(az cosmosdb keys list --name "${5}" --resource-group "${2}" --query primaryMasterKey --output tsv)
secretPrimaryKeyURI=$(az keyvault secret set --vault-name "${3}" --name BaseProjectCosmosDbPrimaryKey --value $cosmosDbKey --query id --output tsv)

# Setting primaryKey secret URI on environement variable
secretPrimaryKeyURI="@Microsoft.KeyVault(SecretUri=$secretPrimaryKeyURI)"
echo "##vso[task.setvariable variable=BaseProjectCosmosDbPrimaryKey]$secretPrimaryKeyURI"

# Setting Azure Keyvault Secret : CosmosDB EndPoint
cosmosDnEndpoint=$(az cosmosdb show --name "${5}" --resource-group "${2}" --query documentEndpoint --output tsv)
cosmosDbEndpointSecretURI=$(az keyvault secret set --vault-name "${3}" --name BaseProjectCosmosDbEndpoint --value $cosmosDnEndpoint --query id --output tsv)

# Setting endpoint secret URI on environement variable
cosmosDbEndpointSecretURI="@Microsoft.KeyVault(SecretUri=$cosmosDbEndpointSecretURI)"
echo "##vso[task.setvariable variable=BaseProjectCosmosDbEndpoint]$cosmosDbEndpointSecretURI"

echo "Setting up the last master provider connectionstring for changefeeds"
masterCosmosDbConnectionString=$(az keyvault secret show --vault-name ${3} --name "${6}" --query id --output tsv)
masterCosmosDbConnectionString="@Microsoft.KeyVault(SecretUri=$masterCosmosDbConnectionString)"
echo "##vso[task.setvariable variable=masterCosmosDbConnectionString]$masterCosmosDbConnectionString"


echo "Setting assResponseListenerConnection into KeyVault secrets"
rawServiceBusTopicConnectionString=$(az servicebus topic authorization-rule keys list \
	--resource-group ${4} \
	--namespace-name ${7} \
	--topic-name ${8} \
	--name listener \
	--query primaryConnectionString \
	--output tsv)

assResponseListenerConnection=$(sed 's/'EntityPath=${8}'//g' <<< $rawServiceBusTopicConnectionString)

assResponseListenerConnectionSecretURI=$(az keyvault secret set \
	--vault-name "${3}" \
	--name assResponseListenerConnection \
	--value $assResponseListenerConnection \
	--query id \
	--output tsv)

echo "Setting assResponseListenerConnection on environment variable"
assResponseListenerConnectionSecretURI="@Microsoft.KeyVault(SecretUri=$assResponseListenerConnectionSecretURI)"
echo "##vso[task.setvariable variable=assResponseListenerConnection]$assResponseListenerConnectionSecretURI"

echo "Setting sasResponseListenerConnection into KeyVault secrets"
rawServiceBusTopicConnectionString=$(az servicebus topic authorization-rule keys list \
	--resource-group ${4} \
	--namespace-name ${7} \
	--topic-name ${9} \
	--name listener \
	--query primaryConnectionString \
	--output tsv)

sasResponseListenerConnection=$(sed 's/'EntityPath=${9}'//g' <<< $rawServiceBusTopicConnectionString)

sasResponseListenerConnectionSecretURI=$(az keyvault secret set \
	--vault-name "${3}" \
	--name sasResponseListenerConnection \
	--value $sasResponseListenerConnection \
	--query id \
	--output tsv)

echo "Setting sasResponseListenerConnection on environment variable"
sasResponseListenerConnectionSecretURI="@Microsoft.KeyVault(SecretUri=$sasResponseListenerConnectionSecretURI)"
echo "##vso[task.setvariable variable=sasResponseListenerConnection]$sasResponseListenerConnectionSecretURI"

echo "Setting missionCreationRequestSenderConnection into KeyVault secrets"
rawServiceBusTopicConnectionString=$(az servicebus topic authorization-rule keys list \
	--resource-group ${4} \
	--namespace-name ${7} \
	--topic-name ${10} \
	--name sender \
	--query primaryConnectionString \
	--output tsv)

missionCreationRequestSenderConnection=$(sed 's/'EntityPath=${10}'//g' <<< $rawServiceBusTopicConnectionString)

missionCreationRequestSenderConnectionSecretURI=$(az keyvault secret set \
	--vault-name "${3}" \
	--name missionCreationRequestSenderConnection \
	--value $missionCreationRequestSenderConnection \
	--query id \
	--output tsv)

echo "Setting missionCreationRequestSenderConnection on environment variable"
missionCreationRequestSenderConnectionSecretURI="@Microsoft.KeyVault(SecretUri=$missionCreationRequestSenderConnectionSecretURI)"
echo "##vso[task.setvariable variable=missionCreationRequestSenderConnection]$missionCreationRequestSenderConnectionSecretURI"


echo "Setting fwuResponseListenerConnection into KeyVault secrets"
rawServiceBusTopicConnectionString=$(az servicebus topic authorization-rule keys list \
	--resource-group ${4} \
	--namespace-name ${7} \
	--topic-name ${11} \
	--name listener \
	--query primaryConnectionString \
	--output tsv)

fwuResponseListenerConnection=$(sed 's/'EntityPath=${11}'//g' <<< $rawServiceBusTopicConnectionString)

fwuResponseListenerConnectionSecretURI=$(az keyvault secret set \
	--vault-name "${3}" \
	--name fwuResponseListenerConnection \
	--value $fwuResponseListenerConnection \
	--query id \
	--output tsv)

echo "Setting fwuResponseListenerConnection on environment variable"
fwuResponseListenerConnectionSecretURI="@Microsoft.KeyVault(SecretUri=$fwuResponseListenerConnectionSecretURI)"
echo "##vso[task.setvariable variable=fwuResponseListenerConnection]$fwuResponseListenerConnectionSecretURI"

echo "Done."