#!/bin/bash
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

resource_group="AzureKeyVaultResourceGroup"
vm_name="KeyVaultDemoVM"
vm_pub_ip="20.91.197.137"
vm_port=5000

# Publish the application
echo "Publishing application..."
dotnet publish "$PROJECT_DIR/KeyVaultDemo.csproj" --configuration Release --output "$PROJECT_DIR/publish"

# Stop the service before copying files
echo "Stopping the service..."
ssh azureuser@${vm_pub_ip} "sudo systemctl stop KeyVaultDemo.service" || true

# Copy files to VM
echo "Copying files to VM..."
scp -r "$PROJECT_DIR"/publish/* azureuser@${vm_pub_ip}:/opt/KeyVaultDemo/

# Start service
echo "Starting service..."
ssh azureuser@${vm_pub_ip} "sudo systemctl start KeyVaultDemo.service"

# Cleanup
rm -rf "$PROJECT_DIR/publish"

echo ""
echo "Deployment complete!"
echo "Application: http://$vm_pub_ip:$vm_port"