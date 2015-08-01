#InstanceManager
InstanceManager is here to manage a single minecraft pack as instance. An example instance showed below:

```JSON
{
   "icon":"https://example.com/packs/ExamplePack/icon.png",
   "background":"https://example.com/packs/ExamplePack/bg.png",
   "version":"2.3.3",
   "updatePath":"https://example.com/packs/ExamplePack/pack.json",
   "author":"Team Infinity Studio",
   "instanceName":"Example Pack",
   "description":"Fully power!",
   "fileSystem":{
      "entirePackageFiles":[
	  {
		"name":"Lang Package",
	    "downloadPath":"https://package.example.com/lang.zip",
		"localPath":"{root}",
        "md5":"0102030405060708"
	  },
	  {
		"name":"Config Package",
	    "downloadPath":"https://package.example.com/config.zip",
		"localPath":"{root}/configs",
        "md5":"0102030405060708"
	  }
      ],
      "officialFiles":[
         {
			"name":"Mod MFR",
			"provideId": "789871A39278B871239B32",
			"localPath":"{root}/mods/",
			"md5":"0102030405060708"
         }
      ],
      "customFiles":[
         {
			"name":"Single lang file",
			"downloadPath":"https://package.example.com/GregTech.lang",
			"localPath":"{root}",
			"md5":"0102030405060708"
         }
      ]
   },
   "startupArguments":"java -jar  -Xmx1G -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M"
}
```
