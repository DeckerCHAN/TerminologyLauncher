# File Repository
## What is it
In order to simplify instance pack delivery procedure. We've designed this mechanism which could let instance packer to use to add fixed files or mods on instance file and simply add it with provided Id at client. We have an official file repository but you could maintain you own file repository in order to boost download speed in specific area.

## How it works
### Start up
Client would try to update repository during start up phase. However, as network not connected, client would ignore this phase and use history repository instead.
### Building
During building phase, instance manager will decode **Official Files** section in instance file. Then, it will send an query to repository manager and try to find which file with matched id then download it or directly use cache.
### Management
Repository file is available to defined by everyone and shared through URL located in configuration file. Exactly procedure shown below:

1. Create your own repository file. Repository is a json format file and each object contained id, name , MD5 and download path. Here is an example repository file:

    ```JSON
    {
       "files":[
          {
             "provideId":"369071A33847B871D39B32",
             "name":"GregTech.lang",
             "md5":"0102030405060708",             "downloadPath":"https://OfficialRepo.com/GregTech.lang"
          },
          {
             "provideId":"789871A39278B871239B32",
             "name":"Railcraft.jar",
             "md5":"0102030405060708",             "downloadPath":"https://OfficialRepo.com/Railcraft.jar"
          }
       ]
    }
    ```
2.  Share through any http/https service such as Apache, CND service, IIS etc.
3.  Share repository file URL to your players. They could set the URL at configuration file shown below:
	```JSON
		{
			"fileRepositoryUrl": "http://example.com/Repo.json",
			"repoRootFolder":"RepoCache",
    		"repoFilePath": "Repo.list"
		}
	```
4. **Launcher reboot is required when changing repository.**
