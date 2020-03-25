

To run it locally on port 8001, run the following in Powershell Core:

    ./publish.ps1
    docker run --rm -p8001:80 dekreydotnet.azurecr.io/signalrgammon


To deploy it to the kubernetes cluster, run the following in Powershell Core:

    ./publish.ps1
    docker push dekreydotnet.azurecr.io/signalrgammon:latest
    kubectl -n signalr-gammon set image deployment signalr-gammon web=$(docker inspect --format='{{index .RepoDigests 0}}' dekreydotnet.azurecr.io/signalrgammon:latest)

(Above command from https://github.com/kubernetes/kubernetes/issues/33664#issuecomment-426500710)