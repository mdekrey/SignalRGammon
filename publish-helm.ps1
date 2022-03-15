
Push-Location "$PSScriptRoot"

$tag = (Get-Date).ToString('yyyy-MM-ddTHH_mm_ss')

## Does not use Azure functions - is standalone
# docker build . -f SignalRGame.Server\Dockerfile -t dekreydotnet.azurecr.io/signalrgammon:$tag

# Functions are deployed to azure via Visual Studio
docker build . -f SignalRGame.Functions.UI\Dockerfile -t dekreydotnet.azurecr.io/signalrgammon:$tag

az acr login --name dekreydotnet
docker push dekreydotnet.azurecr.io/signalrgammon:$tag

Pop-Location


$ns = 'signalr-gammon'
$name = 'main'
$fullImageName = 'dekreydotnet.azurecr.io/signalrgammon'
$domain = 'games.dekrey.net'

helm upgrade --install -n $ns $name --create-namespace mdekrey/single-container `
     --set-string "image.repository=$($fullImageName)" `
     --set-string "image.tag=$tag" `
     --set-string "ingress.annotations.cert-manager\.io/cluster-issuer=letsencrypt" `
     --set-string "ingress.hosts[0].host=$domain"
