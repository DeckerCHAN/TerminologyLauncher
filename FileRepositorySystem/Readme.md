#File Repository
In order to simplify instance pack delivery procedure. We design this mechanism which could let instance packer to use to add some official files or mods on instance file and simply add it with provided Id at client. We have an official file repository but you could maintain you own file repository in order to boost download speed in specific area. Repository file format show below:  
```JSON
{
   "files":[
      {
         "id":"789871A39278B871239B32",
         "fileName":"GregTech.lang",
         "md5":"0102030405060708",
         "downloadPath":"https://OfficialRepo.com/GregTech.lang"
      },
      {
         "id":"789871A39278B871239B32",
         "fileName":"Railcraft.jar",
         "md5":"0102030405060708",
         "downloadPath":"https://OfficialRepo.com/Railcraft.jar"
      }
   ]
}
```

Client would try to update repository during start up phase. However, as network not connected, client would ignore this phase and use history repository instead.
