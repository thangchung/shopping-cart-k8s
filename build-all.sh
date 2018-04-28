GITSHA1=${GITSHA1:=$(git rev-parse HEAD)}
PackageVersion=${PackageVersion:='0.0.1'}
DOCKER_IMAGE_VERSION=${GITSHA1}

# catalog service
docker build -f src/services/catalog/Dockerfile -t shopping-cart/catalog:$DOCKER_IMAGE_VERSION -t shopping-cart/catalog:latest .
# docker tag shopping-cart/catalog:$DOCKER_IMAGE_VERSION shopping-cart/catalog:latest
