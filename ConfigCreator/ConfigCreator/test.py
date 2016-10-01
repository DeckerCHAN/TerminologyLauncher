from upload_sdk import upyun

objects=upyun.UpYun('libertydome',username='upload',password='uploadSDK')

res=objects.getlist('/')

print res
