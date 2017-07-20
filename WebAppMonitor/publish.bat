start /wait cmd /c "ng build -prod --aot=false"
rem Angular build finished
dotnet msbuild WebAppMonitor.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=%~dp0Properties\PublishProfiles\FolderProfile.pubxml
pause