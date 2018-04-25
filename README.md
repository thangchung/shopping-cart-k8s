### Shopping Cart (Microservices)
Building microservices application (Shopping Cart Application) using Kubernetes + Istio with its ecosystem parts.

### Prerequisites
- Hyper-V
- Docker
- Kubernetes (minikube v0.25.2 for windows)
- Istio
- .NET Core SDK
- NodeJS

### Setup Kubernetes
- Using minikube for Windows in this project, but you can use Mac or Linux version as well

### Setup Istio
- After you have minikube installed on your machine, run following command:

```
minikube start --vm-driver hyperv --kubernetes-version="v1.9.0" --hyperv-virtual-switch="minikube_switch" --memory 4096 --extra-config=apiserver.authorization-mode=RBAC --extra-config=apiserver.Features.EnableSwaggerUI=true -extra-config=apiserver.Admission.PluginNames=NamespaceLifecycle,LimitRanger,ServiceAccount,DefaultStorageClass,DefaultTolerationSeconds,MutatingAdmissionWebhook,ValidatingAdmissionWebhook,ResourceQuota --v=7 --alsologtostderr
```

- Run 

```
minikube docker-env
```

- Istio

```
cd istio
kubectl apply -f install/kubernetes/istio.yaml
kubectl apply -f install/kubernetes/istio-auth.yaml
```

- Inject SideCard into services as

```
istioctl kube-inject -f k8s/shopping-cart.yaml | kubectl apply -f -
```