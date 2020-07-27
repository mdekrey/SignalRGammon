

To run it locally on port 3001, run the following in Powershell Core:

    cd SignalRGame.Functions.UI
    npm run build-docker
    npm run start-docker


To deploy it to the kubernetes cluster, run the following in Powershell Core:

    npm run build-docker
    ./publish.ps1
