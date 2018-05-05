function remove_dangling_images() {
  Write-Output "Dangling images (untagged images):"
  docker images -f "dangling=true" -q

  Write-Output "Remove them..."
  docker images | ConvertFrom-String | Where-Object {$_.P2 -eq "<none>"} | ForEach-Object { docker rmi -f $_.P3 }

  # Write-Output "Delete stopped containers..."
  # docker ps -a -q | % { docker rm $_ }
}

# variables
$GITSHA1 = git rev-parse --short HEAD
$DOCKER_IMAGE_VERSION = $GITSHA1

Write-Output "Docker image version: $DOCKER_IMAGE_VERSION"

# set current path
Set-Location "."

# catalog service
$catalogImage = "shopping-cart/catalog:$DOCKER_IMAGE_VERSION"
$catalogImageLatest = "shopping-cart/catalog:latest"
$catalogDockerfile = "src/services/catalog/Dockerfile"
# Write-Output "Removing $catalogImage..."
# docker rmi -f $catalogImage
docker build -f $catalogDockerfile -t $catalogImage -t $catalogImageLatest .

# supplier service
$supplierImage = "shopping-cart/supplier:$DOCKER_IMAGE_VERSION"
$supplierImageLatest = "shopping-cart/supplier:latest"
$supplierDockerfile = "src/services/supplier/Dockerfile"
# Write-Output "Removing $supplierImage..."
# docker rmi -f $supplierImage
docker build -f $supplierDockerfile -t $supplierImage -t $supplierImageLatest --build-arg service_version=$DOCKER_IMAGE_VERSION .

# security service
$securityImage = "shopping-cart/security:$DOCKER_IMAGE_VERSION"
$securityImageLatest = "shopping-cart/security:latest"
$securityDockerfile = "src/services/security/Dockerfile"
# Write-Output "Removing $securityImage..."
# docker rmi -f $securityImage
docker build -f $securityDockerfile -t $securityImage -t $securityImageLatest .

remove_dangling_images
