SignalRGammon has so far been manually deployed, sorry the "charts" folder was a tease.

# Basic namespace

    kubectl create namespace cert-manager
    kubectl apply --validate=false -f https://raw.githubusercontent.com/jetstack/cert-manager/release-0.14/deploy/manifests/00-crds.yaml
    helm repo add jetstack https://charts.jetstack.io  --version 0.14.0
    helm install --namespace cert-manager --name cert-manager jetstack/cert-manager
    helm install  --namespace nginx --name nginx stable/nginx-ingress

# SignalRGammon namespace

    kubectl create namespace signalr-gammon

Setting up the container registry:

    az ad sp create-for-rbac --scopes /subscriptions/<Subscription ID>/resourcegroups/<ResourceGroup>/providers/Microsoft.ContainerRegistry/registries/<ContainerRegistry> --role Reader --name image-reader

Using the output there, then run this:

    kubectl -n signalr-gammon create secret docker-registry <secret-name> --docker-server "<ContainerRegistry>.azurecr.io" --docker-username=<appId> --docker-password <password>

Then:

    kubectl -n signalr-gammon apply -f ./templates/deployment.yaml
    kubectl -n signalr-gammon apply -f ./templates/service.yaml
    kubectl -n signalr-gammon apply -f ./templates/ingress.yaml
    kubectl -n signalr-gammon apply -f ./templates/production-issuer.yaml
    kubectl -n signalr-gammon apply -f ./templates/web-certificate.yaml
