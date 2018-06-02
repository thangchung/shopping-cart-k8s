# Shopping Cart Application with Microservices Approach

Building microservices application (Shopping Cart Application - Polyglot for services) using Kubernetes + Istio with its ecosystem parts.

> ### Disclamation
>
> * Should have `MINIKUBE_HOME` environment variable in your machine, and value should point to `C:\users\<your name>\`
> * Should run powershell script to create `minikube` machine in `C:` drive.
> * If it threw the exception that it couldn't find out `minikube` machine in Hyper-V so just simply delete everything in `<user>/.minikube` folder, but we could keep `cache` folder to avoid download everything from scratch, then runs it subsequently.

## Table of contents

* [Technical Stack](https://github.com/thangchung/shopping-cart-k8s#technical-stack)
* [Setup Local Kubernetes](https://github.com/thangchung/shopping-cart-k8s#setup-local-kubernetes)
* [Setup Istio](https://github.com/thangchung/shopping-cart-k8s#setup-istio)
* [Setup Ambassador](https://github.com/thangchung/shopping-cart-k8s#setup-ambassador)
* [Install and Work with Helm](https://github.com/thangchung/shopping-cart-k8s#install-and-work-with-helm-kubernetes-package-manager)
* [Build Our Own Microservices](https://github.com/thangchung/shopping-cart-k8s#build-our-own-microservices)
* [Available Microservices](https://github.com/thangchung/shopping-cart-k8s#available-microservices)
* [Develop A New Service](https://github.com/thangchung/shopping-cart-k8s#develop-a-new-service)
* [Metrics Collection, Distributed Tracing, and Visualization](https://github.com/thangchung/shopping-cart-k8s#metrics-collection-distributed-tracing-and-visualization)
* [Tips and Tricks](https://github.com/thangchung/shopping-cart-k8s#tips-and-tricks)

## Technical Stack

* [Hyper-V](https://docs.microsoft.com/en-us/windows-server/virtualization/hyper-v/hyper-v-technology-overview) or [VirtualBox](https://www.virtualbox.org)
* [Docker](https://www.docker.com)
* [Kubernetes](https://kubernetes.io) ([minikube](https://github.com/kubernetes/minikube) v0.25.2 for windows)
* [Istio](https://istio.io)
* [Ambassador](https://www.getambassador.io)
* [Helm](https://helm.sh)
* [Weave Scope](https://www.weave.works) on Kubernetes
* [.NET Core SDK](https://www.microsoft.com/net/download/windows)
* [NodeJS](https://nodejs.org)
* Windows Powershell
* [xip](http://xip.io) or [nip](http://nip.io) for access virtual
  hosts on your development web server from devices on your
  local network, like iPads, iPhones, and other computers.

## Setup Local Kubernetes

* Using `minikube` for `Windows` in this project, but you can use `Mac` or `Linux` version as well

* Download the appropriate package of your minikube at https://github.com/kubernetes/minikube/releases (Used `v0.25.2` for this project)

* Install it into your machine (Windows 10 in this case)

* After installed `minikube`, then run

`Hyper-V`

```
> minikube start --kubernetes-version="v1.9.0" --vm-driver=hyperv --hyperv-virtual-switch="minikube_switch" --cpus=4 --memory=4096 --v=999 --alsologtostderr
```

Then start with full option

```
> minikube start --extra-config=apiserver.Features.EnableSwaggerUI=true,apiserver.Authorization.Mode=RBAC,apiserver.Admission.PluginNames=NamespaceLifecycle,LimitRanger,ServiceAccount,DefaultStorageClass,DefaultTolerationSeconds,MutatingAdmissionWebhook,ValidatingAdmissionWebhook,ResourceQuota --v=999 --alsologtostderr
```

`VirtualBox v5.2.8`

```
> minikube start --vm-driver="virtualbox" --kubernetes-version="v1.10.0" --cpus=4 --memory 4096 --extra-config=apiserver.authorization-mode=RBAC,apiserver.Features.EnableSwaggerUI=true,apiserver.Admission.PluginNames=NamespaceLifecycle,LimitRanger,ServiceAccount,DefaultStorageClass,DefaultTolerationSeconds,MutatingAdmissionWebhook,ValidatingAdmissionWebhook,ResourceQuota --v=7 --alsologtostderr
```

## Setup Istio

* Download the appropriate package of Istio at https://github.com/istio/istio/releases

* Upzip it into your disk, let say `D:\istio\`

* `cd` into `D:\istio\`, then run

```
> kubectl create -f install/kubernetes/istio.yaml

or

> kubectl create -f install/kubernetes/istio-auth.yaml
```

![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/default-istio-images.PNG)

**_Notes: set `istio\bin\istioctl.exe` to the `PATH` of the windows._**

## Setup Ambassador

* If you're running in a cluster with RBAC enabled:

```
> kubectl apply -f https://getambassador.io/yaml/ambassador/ambassador-rbac.yaml
```

* Without RBAC, you can use:

```
> kubectl apply -f https://getambassador.io/yaml/ambassador/ambassador-no-rbac.yaml
```

* If you're going to **use Ambassador**, then run as following script

```
> cd k8s
> istioctl kube-inject -f istio-shopping-cart.yaml | kubectl apply -f -
> kubectl apply -f ambassador-service.yaml
```

**_Notes: for some reason, I couldn't run the no-rbac mode on my local development._**

## Dashboard

```
> minikube dashboard
```

![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/minikube-ui.PNG)

```
> kubectl get svc -n istio-system
```

```
> export GATEWAY_URL=$(kubectl get po -l istio-ingress -n istio-system -o jsonpath='{.items[0].status.hostIP}'):$(kubectl get svc istio-ingress -n istio-system -o jsonpath='{.spec.ports[0].nodePort}')
```

```
> curl $GETWAY_URL
```

## Install and Work with Helm (Kubernetes Package Manager)

```
> choco install kubernetes-helm
```

```
> cd <git repo>
> helm init
> helm repo update
> helm version
```

* Install RabbitMq

```
> helm install --name my-rabbitmq --set rbacEnabled=false stable/rabbitmq
```

Now we can use `amqp://my-rabbitmq.default.svc.cluster.local:5672` on Kubernetes Cluster, but what if we want to leverage it for the local development. The solution is `port-forward` it to our localhost as

```
> kubectl get pods | grep rabbitmq | awk '{print $1;}'
> kubectl port-forward <pod name just got> 15672
```

Or port-forward 5672 on Kubernetes (amqp protocol) to localhost:5672

```
> kubectl port-forward <pod name just got> 1234:5672
```

Now we have

```
> amqp://root:letmein@127.0.0.1:1234
```

* Install Redis

```
> helm install --name my-redis stable/redis
```

* References:
  * [Stable Helm Template](https://github.com/kubernetes/charts/blob/master/stable)
  * [How to install Helm](https://gist.github.com/ssudake21/e60d917ede9c0198f1ae56b07a10dd9a)
  * Issue: [cannot access to 127.0.0.1](https://github.com/kubernetes/helm/issues/2464)

## Build Our Own Microservices

* Run

```
> minikube docker-env
```

* Copy and Run

```
> @FOR /f "tokens=*" %i IN ('minikube docker-env') DO @%i
```

From now on, we can type `docker images` to list out all images in Kubernetes local node.

* Build our microservices by running

```
> powershell -f build-all.ps1
```

* Then if you want to just test it then run following commands

```
> cd k8s
> kubectl apply -f shopping-cart.yaml
```

![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/shopping-cart-images.PNG)

* In reality, we need to inject the **sidecards** into microservices as following

```
> cd k8s
> istioctl kube-inject -f shopping-cart.yaml | kubectl apply -f -
```

* In `Deployment`

  ![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/istio-injected-with-sidecar.PNG)

* In each `Pod`

  ![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/istio-injected-with-sidecar-pods.PNG)

## Available Microservices

* Get host IP

```
> minikube ip
```

* Get Ambassador port

```
> kubectl get svc ambassador -o jsonpath='{.spec.ports[0].nodePort}'
```

* Finally, open browser with `<IP>:<PORT>`

* Microservices
  * Catalog service: `www.<IP>.xip.io:<PORT>/c/swagger/`. For example, http://www.192.168.1.6.xip.io:32097/c/swagger/
  * Supplier service: `www.<IP>.xip.io:<PORT>/s/`
  * Security service: `www.<IP>.xip.io:<PORT>/id/account/login` or `www.<IP>.xip.io:<PORT>/id/.well-known/openid-configuration`
  * Email service: `www.<IP>.xip.io:<PORT>/e/`

## Develop A New Service

* Build the whole application using

```
> powershell -f build-all.ps1
```

* Then run

```
> kubectl delete -f shopping-cart.yaml
```

* And

```
> kubectl apply -f shopping-cart.yaml
```

* Waiting a sec for Kubernetes to refresh.

## Metrics Collection, Distributed Tracing, and Visualization

### Setup Prometheus

```
> cd istio\install\kubernetes\addons\
> kubectl apply -f prometheus.yaml
```

### Setup Grafana

```
> cd istio\install\kubernetes\addons\
> kubectl apply -f grafana.yaml
```

```
> kubectl -n istio-system port-forward $(kubectl -n istio-system get pod -l app=grafana -o jsonpath='{.items[0].metadata.name}') 3000:3000 &
```

```
> curl http://localhost:3000
```

![](https://github.com/thangchung/shopping-cart-k8s/blob/master/assets/grafana-ui.PNG)

### Setup Service Graph

TODO

### Setup Zipkin or Jaeger

TODO

### Setup Weave Scope

* Install and run it on the local

```
> kubectl apply -f "https://cloud.weave.works/k8s/scope.yaml?k8s-version=v1.9.0"
```

* Then `port-forward` it out as following

```
> kubectl get -n weave pod --selector=weave-scope-component=app -o jsonpath='{.items..metadata.name}'
> kubectl port-forward -n <weave scope name> 4040
```

* Go to `http://localhost:4040`

### Tips and Tricks

* Print out environment variables in one container

```
> kubectl get pods
```

```
> kubectl exec <pod name> env
```

* Switch to another use-context

Let say we have a profile named `minikube19`, then just type the command as below

```
> kubectl config use-context minikube19

Switched to context "minikube19".
```

```
> minikube config set profile minikube19
```

## Working with Ubuntu on Hyper-V

* Install Ubuntu Server VM (ubuntu-18.04-live-server-amd64.iso) on Hyper-V (External Network)
* `ssh` into that machine, then `sudo apt-get install docker.io`
* [Install Virtual Box (5.2) on Ubuntu VM](https://websiteforstudents.com/virtualbox-5-2-on-ubuntu-16-04-lts-server-headless)
* [Install `kubectl`](https://kubernetes.io/docs/tasks/tools/install-kubectl/)

```
> apt-get update && apt-get install -y apt-transport-https
> curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | apt-key add -
> cat <<EOF >/etc/apt/sources.list.d/kubernetes.list
deb http://apt.kubernetes.io/ kubernetes-xenial main
EOF
> apt-get update
> apt-get install -y kubectl
```

* Install `minikube`

```
> curl -Lo minikube https://storage.googleapis.com/minikube/releases/v0.27.0/minikube-linux-amd64 && chmod +x minikube && sudo mv minikube /usr/local/bin/
```

* Alias command

```
> alias k='kubectl'
> alias mk='/usr/local/bin/minikube'
```

* Then run following script

```
> minikube start --vm-driver="virtualbox" --kubernetes-version="v1.9.0" --cpus=4 --memory 4096 --extra-config=apiserver.authorization-mode=RBAC --v=7 --alsologtostderr
```

* Make Hyper-V treats well with Ubuntu by [enable nested vitualization](https://www.petri.com/enable-nested-hyper-v-virtualization-windows-10-build-10565), before of that we need to make [Powershell run well](http://tritoneco.com/2014/02/21/fix-for-powershell-script-not-digitally-signed/)
* [Make the internal Kubernetes Dashboard can be accessed by outside](https://www.debuntu.org/how-to-redirecting-network-traffic-to-a-new-ip-using-iptables/)

```
> sysctl net.ipv4.ip_forward=1
> iptables -t nat -A PREROUTING -p tcp -d <VM IP> --dport 30000 -j DNAT --to-destination 192.168.99.100:30000
> iptables -t nat -A POSTROUTING -j MASQUERADE
```

```
> sudo iptables -t nat -v -x -n -L
> sudo dhclient eth0 -v
```

* [Install `samba` on ubuntu VM](https://tutorials.ubuntu.com/tutorial/install-and-configure-samba)

* `socat` on Ubuntu Server

```
> sudo apt-get install socat
> socat -v tcp-listen:30000,fork tcp:192.168.99.100:30000
```

* ngrok on Ubuntu server

```
> wget https://bin.equinox.io/c/4VmDzA7iaHb/ngrok-stable-linux-amd64.zip
> unzip ngrok-stable-linux-amd64.zip
> mk dashboard --url
> ./ngrok http 192.168.99.100:30000 -region ap
```

* Open ubuntu VM port (if needed)

```
> sudo iptables -I INPUT -p tcp --dport 2375 -j ACCEPT
or
> sudo ufw allow 2375
then
> nc -l 2375
```
