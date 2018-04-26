# Shopping Cart (Microservices)
Building microservices application (Shopping Cart Application) using Kubernetes + Istio with its ecosystem parts.

> ### Disclamation 
> - Should have `MINIKUBE_HOME` environment variable in your machine, and value should point to `C:\users\<your name>\`
> - Should run powershell script to create `minikube` machine in `C:` drive.
> - If it threw the exception that it couldn't find out `minikube` machine in Hyper-V so just simply delete everything in `<user>/.minikube` folder, but we could keep `cache` folder to avoid download everything from scratch, then runs it subsequently.

## Prerequisites
- Hyper-V
- Docker
- Kubernetes (minikube v0.25.2 for windows)
- Istio
- .NET Core SDK
- NodeJS

## Setup Local Kubernetes
- Using minikube for Windows in this project, but you can use Mac or Linux version as well

- Download the appropriate package of your minikube at https://github.com/kubernetes/minikube/releases (We use `v0.25.2` in this project)

- Install it into your machine (Windows 10 in this case)

- After installed `minikube`, then run 

```
> minikube start --vm-driver hyperv --kubernetes-version="v1.9.0" --hyperv-virtual-switch="minikube_switch" --memory 4096 --extra-config=apiserver.authorization-mode=RBAC --extra-config=apiserver.Features.EnableSwaggerUI=true -extra-config=apiserver.Admission.PluginNames=NamespaceLifecycle,LimitRanger,ServiceAccount,DefaultStorageClass,DefaultTolerationSeconds,MutatingAdmissionWebhook,ValidatingAdmissionWebhook,ResourceQuota --v=7 --alsologtostderr
```

## Setup Istio
- Download the appropriate package of Istio at https://github.com/istio/istio/releases 

- Upzip it into your disk, let say `D:\istio\` 

- Run

```
> minikube docker-env
```

- Copy and Run

```
> @FOR /f "tokens=*" %i IN ('minikube docker-env') DO @%i
```

From now on, we can type `docker images` to list out all images in Kubernetes local node.

- `cd` into `D:\istio\`, then run

```
> kubectl apply -f install/kubernetes/istio.yaml
> kubectl apply -f install/kubernetes/istio-auth.yaml
```

## Build our own microservices

- Build our microservices by running

```
> powershell -f build-all.ps1
```

- Then if you want to just test it then run following commands

```
> cd k8s
> kubectl apply -f shopping-cart.yaml
```

- In reality, we need to inject the **sidecards** into microservices as following 

```
> cd k8s
> istioctl kube-inject -f shopping-cart.yaml | kubectl apply -f -
```

### Dashboard

```
> minikube dashboard
```

![](assets\minikube-ui.png)

```
> kubectl get svc -n istio-system
```

```
> export GATEWAY_URL=$(kubectl get po -l istio-ingress -n istio-system -o jsonpath='{.items[0].status.hostIP}'):$(kubectl get svc istio-ingress -n istio-system -o jsonpath='{.spec.ports[0].nodePort}')
```

```
> curl $GETWAY_URL
```