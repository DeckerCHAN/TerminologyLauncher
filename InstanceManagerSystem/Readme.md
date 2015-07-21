#InstanceManager
InstanceManager is here to manage a single minecraft pack as instance. An example instance showed below:

```JSON
{
   "icon":"https://example.com/packs/ExamplePack/icon.png",
   "background":"https://example.com/packs/ExamplePack/bg.png",
   "version":"2.3.3",
   "author":"Team Infinity Studio",
   "instanceName":"Example Pack",
   "description":"Fully power!",
   "fileSystem":{
      "entirePackageFile":{
         "downloadPath":"https://example.com/GregTech.lang",
         "md5":"0102030405060708"
      },
      "officialFiles":[
         {
            "localPath":"/1.7.10.jar",
            "md5":"0102030405060708",            
            "provideId":"9EB84A4A0BE74317940D05B32935738F"
         }
      ],
      "customFiles":[
         {
            "localPath":"/GregTech.lang",
            "md5":"0102030405060708",
            "downloadPath":"https://example.com/GregTech.lang"
         }
      ]
   },
   "startupScript":"java -jar  -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M"
}
```
