function remove_dangling_images() {
  Write-Output "Dangling images (untagged images):"
  docker images -f "dangling=true" -q

  Write-Output "Remove them..."
  docker images | ConvertFrom-String | Where-Object {$_.P2 -eq "<none>"} | ForEach-Object { docker rmi -f $_.P3 }

  # Write-Output "Delete stopped containers..."
  # docker ps -a -q | % { docker rm $_ }
}

# variables
$GITSHA1 = git rev-parse HEAD
$DOCKER_IMAGE_VERSION = $GITSHA1

Write-Output "Docker image version: $DOCKER_IMAGE_VERSION"

# set current path
Set-Location "."

# catalog service
$catalogImage = "shopping-cart/catalogs:$DOCKER_IMAGE_VERSION"
$catalogImageLatest = "shopping-cart/catalogs:latest"
$catalogDockerfile = "src/services/catalogs/Dockerfile"
Write-Output "Removing $catalogImage..."
docker rmi -f $catalogImage
Write-Output "Removing $catalogImage..."
docker build -f $catalogDockerfile -t $catalogImage -t $catalogImageLatest .
# docker tag $catalogImage $catalogImageLatest

remove_dangling_images
