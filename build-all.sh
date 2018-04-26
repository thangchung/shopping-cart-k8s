GITSHA1=${GITSHA1:=$(git rev-parse HEAD)}
PackageVersion=${PackageVersion:='0.0.1'}
DOCKER_IMAGE_VERSION=${GITSHA1}

# catalog service
docker build -f src/services/catalogs/Dockerfile -t shopping-cart/catalogs:$DOCKER_IMAGE_VERSION -t shopping-cart/catalogs:latest .
# docker tag shopping-cart/catalogs:$DOCKER_IMAGE_VERSION shopping-cart/catalogs:latest
