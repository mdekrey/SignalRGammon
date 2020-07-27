docker push dekreydotnet.azurecr.io/signalrgammon:latest
kubectl -n signalr-gammon set image deployment signalr-gammon web=$(docker inspect --format='{{index .RepoDigests 0}}' dekreydotnet.azurecr.io/signalrgammon:latest)

# Above command from https://github.com/kubernetes/kubernetes/issues/33664#issuecomment-426500710