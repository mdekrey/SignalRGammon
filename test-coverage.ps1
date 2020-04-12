# Passes through args to the dotnet test tool
# For example:
#     ./test-coverage.ps1 --filter DisplayName~SignalRGame.ClashOfClones.Rules.ArmyLayoutMatcherShould

# To get more test names:
#     dotnet test -t

dotnet tool install dotnet-reportgenerator-globaltool --tool-path ./tools
dotnet test $args /p:CollectCoverage=true /p:CoverletOutput="$(Get-Location)/TestResults/" /p:CoverletOutputFormat=lcov
.\tools\reportgenerator.exe -reports:"$(Get-Location)/TestResults/coverage.info" -targetdir:"$(Get-Location)/TestResults"
start "$(Get-Location)/TestResults/index.htm"

