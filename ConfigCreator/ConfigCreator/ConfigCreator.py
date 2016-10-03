import zipfile, os, json, hashlib, sys, threading, string
from threading import Thread

def zip_folder(threadID,destination,file_name,folder_location):      #Example: zip_folder('D:/','sample.zip','D:/test/')
    if os.path.exists(destination):
        if os.path.exists(destination+'/'+file_name):
            print 'Thread-'+str(threadID)+': '+'Zip File {} is existed! Please rename or change the path. '.format(destination+'/'+file_name)
            return 1
        else:
            File=zipfile.ZipFile(destination+'/'+file_name,'w',zipfile.ZIP_DEFLATED)
            print 'Thread-'+str(threadID)+': '+'Zip File created: '+destination+'/'+file_name
            file_list=[]
            folder_list=[]
            for temp in os.listdir(folder_location):
                if os.path.isdir(folder_location+'/'+temp):
                    folder_list.append(folder_location+'/'+temp+'/')
                else:
                    file_list.append(folder_location+'/'+temp)

            #List out each file path. Can be replace by os.walk, but I'm lazy xD
            if folder_list:
                folder_exist=True
                while folder_exist:
                    for temp in folder_list:
                        folder_exist=False
                        for temp1 in os.listdir(temp):
                            if os.path.isdir(temp+temp1):
                                folder_list.append(temp+temp1+'/')
                                folder_exist=True
                            else:
                                file_list.append(temp+temp1)
            file_in_zip_list={}

            for temp in file_list:
                md5_value=md5_generator(temp)
                print 'Thread-'+str(threadID)+': '+'md5 for {}: '.format(temp)
                file_in_zip_list[temp[len(folder_location):]]=md5_value
                print 'Thread-'+str(threadID)+': '+'Compressing: '+temp
                File.write(temp,temp[len(folder_location):])
            File.close()

            hash_value=md5_generator(destination+'/'+file_name)
            if not hash_value==-1:
                print 'Thread-'+str(threadID)+': '+'MD5: '+hash_value
            else:
                print 'Thread-'+str(threadID)+': '+'Internal problem occur during generating md5 value for {}'.format(file_name)
            return (destination+'/'+file_name,hash_value,file_in_zip_list)

def zip_file(threadID,Dot_Minecraft_Path,destination,file_name,file_or_list,folder_exclude_in_zip=False):
    if os.path.exists(destination):
        if os.path.exists(destination+'/'+file_name):
            print 'Thread-'+str(threadID)+': '+'Zip File {} is existed! Please rename or change the path. '.format(destination+'/'+file_name)
            return 1
        else:
            File=zipfile.ZipFile(destination+'/'+file_name,'w',zipfile.ZIP_DEFLATED)
            print 'Thread-'+str(threadID)+': '+'Zip File created: '+destination+'/'+file_name
    
    file_in_zip_list={}

    if isinstance(file_or_list,list):
        for file in file_or_list:
            if not folder_exclude_in_zip:
                md5_value=md5_generator(file)
                print 'Thread-'+str(threadID)+': '+'md5 for {}: {}'.format(file,md5_value)
                file_in_zip_list[file[len(Dot_Minecraft_Path):]]=md5_value
                print 'Thread-'+str(threadID)+': '+'Compressing: '+file
                File.write(file,file[len(Dot_Minecraft_Path):])
            elif folder_exclude_in_zip:
                md5_value=md5_generator(file)
                print 'Thread-'+str(threadID)+': '+'md5 for {}: {}'.format(file,md5_value)
                file_in_zip_list[file[len(Dot_Minecraft_Path):]]=md5_value
                print 'Thread-'+str(threadID)+': '+'Compressing: '+file
                file_in_zip_position=file[len(Dot_Minecraft_Path)+1:]
                file_in_zip_position=file_in_zip_position.find('/')+len(Dot_Minecraft_Path)+1
                File.write(file,file[file_in_zip_position:])
    elif isinstance(file_or_list,str):
        if not folder_exclude_in_zip:
            md5_value=md5_generator(file_or_list)
            print 'Thread-'+str(threadID)+': '+'md5 for {} : {}'.format(file_or_list,md5_value)
            file_in_zip_list[file_or_list[len(Dot_Minecraft_Path):]]=md5_value
            print 'Thread-'+str(threadID)+': '+'Compressing :'+file_or_list
            File.write(file_or_list,file_or_list[len(Dot_Minecraft_Path):])
        elif folder_exclude_in_zip:
            md5_value=md5_generator(file_or_list)
            print 'Thread-'+str(threadID)+': '+'md5 for {}: {}'.format(file_or_list,md5_value)
            file_in_zip_list[file_or_list[len(Dot_Minecraft_Path):]]=md5_value
            print 'Thread-'+str(threadID)+': '+'Compressing: '+file_or_list
            file_in_zip_position=file_or_list[len(Dot_Minecraft_Path)+1:]
            file_in_zip_position=file_in_zip_position.find('/')+len(Dot_Minecraft_Path)+1
            File.write(file_or_list,file_or_list[file_in_zip_position:])

    File.close()
    hash_value=md5_generator(destination+'/'+file_name)
    return (destination+'/'+file_name,hash_value,file_in_zip_list)

def md5_generator(file_path):
    if os.path.isfile(file_path):
        #Open Zipped File as read-bit mode
        File=open(file_path,mode='rb')

        #Calculate MD5
        block_size=8192
        hash_value=hashlib.md5()
        while True:
            tmp=File.read(block_size)
            if not tmp:
                del tmp
                break
            hash_value.update(tmp)
        File.close()

        hash_value=hash_value.hexdigest()
        return hash_value
    else:
        return -1

def Minecraft_directory_search(Dot_Minecraft_Path):       #Minecraft_directory_search('D:/MC/Test/1.7/.minecraft')
    if os.path.exists(Dot_Minecraft_Path):
        folder_path=[]
        file_path=[]
        for temp in os.listdir(Dot_Minecraft_Path):
            if os.path.isdir(Dot_Minecraft_Path+'/'+temp):
                folder_path.append(temp+'/')
            else:
                file_path.append(temp)

        del temp
        
        #Find enterence

        #General file path
        versions_list={}
        for temp in os.listdir(Dot_Minecraft_Path):
            if os.path.isfile(Dot_Minecraft_Path+'/'+temp) and os.path.splitext(temp)[1]=='.jar':
                versions_list[os.path.splitext(temp)[0]]={'Jar_file':Dot_Minecraft_Path+'/'+temp}
        if versions_list:
            for temp in os.listdir(Dot_Minecraft_Path):
                if os.path.isdir(Dot_Minecraft_Path+'/'+temp):
                    for dlls in os.listdir(Dot_Minecraft_Path+'/'+temp):
                        if os.path.splitext(dlls)[1]=='.dll' and dlls=='jinput-dx8.dll' or dlls=='jinput-dx8_64.dll':
                            versions_list[os.path.splitext(temp)[0]]['Native_files_folder']=Dot_Minecraft_Path+'/'+temp
                        else:
                            raise 'Native files are missing'
                    del dlls
        del temp

        #HMCL store path: .minecraft/versions
        if os.path.exists(Dot_Minecraft_Path+'/versions'):
            versions_address=os.listdir(Dot_Minecraft_Path+'/versions')
            if versions_address:
                for different_versions_folder in versions_address:
                    if os.path.isdir(Dot_Minecraft_Path+'/versions/'+different_versions_folder):
                        temp_dict={}
                        Jar_file_found=False
                        Native_file_found=False
                        Json_file_found=False
                        for temp2 in os.listdir(Dot_Minecraft_Path+'/versions/'+different_versions_folder):
                            if os.path.isdir(Dot_Minecraft_Path+'/versions/'+different_versions_folder+'/'+temp2):
                                for dlls in os.listdir(Dot_Minecraft_Path+'/versions/'+different_versions_folder+'/'+temp2):       #Find native dll file
                                    if os.path.splitext(dlls)[1]=='.dll' and dlls=='jinput-dx8.dll' or dlls=='jinput-dx8_64.dll':
                                        temp_dict['Native_files_folder']=Dot_Minecraft_Path+'/versions/'+different_versions_folder+'/'+temp2
                                        Native_file_found=True
                                        break
                                    else:
                                        continue
                                try:           #Sometimes folder might be empty, and may cause exception.
                                    dlls=0
                                    del dlls
                                except UnboundLocalError:
                                    continue
                            else:
                                if os.path.splitext(temp2)[1]=='.jar':       #Find jar file
                                    temp_dict['Jar_file']=Dot_Minecraft_Path+'/versions/'+different_versions_folder+'/'+temp2
                                    Jar_file_found=True
                                if os.path.splitext(temp2)[1]=='.json':      #Find json file for the jar file, if exist. It contained the address for downloading the libraries.
                                    temp_dict['Jar_file_config']=Dot_Minecraft_Path+'/versions/'+different_versions_folder+'/'+temp2
                                    Json_file_found=True

                        # Throw out the missing version
                        if not Jar_file_found and not Native_file_found and not Json_file_found:
                            sys.stderr.write('{} is missing important file: Natives, Jar, Config.'.format(different_versions_folder))
                            continue
                        elif not Jar_file_found and not Json_file_found:
                            sys.stderr.write('{} is missing important file: Jar, Config.'.format(different_versions_folder))
                            continue
                        if not Jar_file_found:
                            temp_dict['Jar_file']=None
                        if not Native_file_found:
                            temp_dict['Native_files_folder']=None
                        if not Json_file_found:
                            temp_dict['Jar_file_config']=None
                        versions_list[different_versions_folder]=temp_dict.copy()
            del different_versions_folder,temp2
            print versions_list
            return versions_list
        
    else:
        raise "Minecraft folder path not found!"

def libraries_search(Dot_Minecraft_Path,selected_version_keyname,Full_version_info_list):        #Return Value=-1 - List for inherits missing; Return Value=-2 - Config file is uncomplete; Return Value=-3 - 'inheritsFrom' does not match any keys in the version list; Return Value=list - A list added libraries information
    depend_list=[selected_version_keyname]
    libraries_folder_path=[]
    count=0   # Make sure the main class is the first one
    count1=0   # Reserve for id recongize
    count2=0    # Find the main jar file if possible
    # Get the whole library list out.
    for inheritFrom in depend_list:
        if Full_version_info_list.has_key(inheritFrom):
            if Full_version_info_list[inheritFrom]['Jar_file_config'] and not Full_version_info_list[inheritFrom]['Jar_file_config']==None:
                temp=json.load(open(Full_version_info_list[inheritFrom]['Jar_file_config'],'r'))
                if temp.has_key('inheritsFrom'):
                    depend_list.append(temp['inheritsFrom'])
                elif not temp.has_key('inheritsFrom'):
                    if temp.has_key('libraries'):
                        liteloader_class_find=False
                        forge_class_find=False
                        officialMinecraft_class_find=False
                        for library_section in temp['libraries']:
                            if library_section['name'].find('com.mumfrey:liteloader')!=-1:
                                liteloader_class_find=True
                            elif library_section['name'].find('net.minecraftforge:forge')!=-1:
                                forge_class_find=True
                            elif library_section['name'].find('com.mojang:realms')!=-1:
                                officialMinecraft_class_find=True
                        if liteloader_class_find and forge_class_find:
                            pass
                        elif liteloader_class_find and not forge_class_find:
                            sys.stderr.write('Config file missing forge library infomation!')
                            return -2
                        elif forge_class_find and officialMinecraft_class_find and not liteloader_class_find:
                            pass
                        elif forge_class_find and not officialMinecraft_class_find and not liteloader_class_find:
                            sys.stderr.write('Config file missing Minecraft depend library information!')
                            return -2
                        elif officialMinecraft_class_find and not forge_class_find and not liteloader_class_find:
                            pass
                if temp.has_key('libraries'):
                    for s in libraries_list_analyzer(Dot_Minecraft_Path,temp['libraries']):
                        libraries_folder_path.append(s)
                else:
                    sys.stderr.write('No libraries information in the json file!')
                    return -2
                if temp.has_key('mainClass') and count<1:
                    Full_version_info_list[selected_version_keyname]['mainClass']=temp['mainClass']
                    count+=1
                if temp.has_key('minecraftArguments'):
                    tweakclass_start,tweakclass_end=(0,0)
                    while not tweakclass_start==-1:
                        tweakclass_start=temp['minecraftArguments'].find('--tweakClass ',tweakclass_end)
                        tweakclass_end=temp['minecraftArguments'].find(' --',tweakclass_start)
                        if not Full_version_info_list[selected_version_keyname].has_key('tweakClasses'):
                            Full_version_info_list[selected_version_keyname]['tweakClasses']=[]
                        if not tweakclass_start==-1:
                            Full_version_info_list[selected_version_keyname]['tweakClasses'].append(temp['minecraftArguments'][tweakclass_start+13:tweakclass_end])
                if temp.has_key('assets'):
                    Full_version_info_list[selected_version_keyname]['assetIndex']=temp['assets']
                    Full_version_info_list[selected_version_keyname]['version']=temp['assets']    # Lazy way to get version number >o<
                if temp.has_key('id') and count1<1:
                    Full_version_info_list[selected_version_keyname]['instanceName']=temp['id']
                if temp.has_key('jar') and count2<1:       # Fix the problem that may ignore the version only have config in the folder
                    if Full_version_info_list[selected_version_keyname]['Jar_file']!=None:
                        pass
                    elif Full_version_info_list[selected_version_keyname]['Jar_file']==None:
                        Full_version_info_list[selected_version_keyname]['Jar_file']=os.path.split(Full_version_info_list[temp['jar']]['Jar_file_config'])[0]+'/'+temp['jar']+'.jar'
            else:
                sys.stderr.write('Could not find the json file for '+inheritFrom+' , Process Abort!')
                return -1
            del temp
            count1+=1
        else:
            if count1<1:
                sys.stderr.write("Couldn't find {}! Check if name is correct!".format(inheritFrom))
            elif count1>1:
                sys.stderr.write('Could not fine depend list! Check the key "inheritsFrom" in the json file if you had ever change version names!')
                return -3
    if count<1:
        sys.stderr.write('Could not find main class for {}! Using default. '.format(selected_version_keyname))
    # Sort from a-z
    for x in range(len(libraries_folder_path)-1,0,-1):  #Limit times for switching
        for y in range(0,x):    #Limit range for switching
            if libraries_folder_path[y]>libraries_folder_path[y+1]:
                libraries_folder_path[y],libraries_folder_path[y+1]=libraries_folder_path[y+1],libraries_folder_path[y]
    # Kick out repeat path
    x=0
    for path1 in libraries_folder_path:
        length=len(libraries_folder_path)-1
        if x+1<=length:
            if path1==libraries_folder_path[x+1]:
                libraries_folder_path.pop(x+1)
        x+=1

    Full_version_info_list[selected_version_keyname]['libraries_path']=libraries_folder_path
    return Full_version_info_list

def libraries_list_analyzer(Dot_Minecraft_Path,libraries_list):        #Return Value=list - A list of libraries path (not absolute path)
    libraries_detail_path_list=[]
    for individual_library_dict in libraries_list:
        library_name=individual_library_dict['name']
        first_seperate_sign=library_name.find(':')
        # Turn every dot to slash before the first ':'
        if first_seperate_sign!=-1:
            while True:
                indexplace=library_name[0:first_seperate_sign].find('.')
                if indexplace!=-1:
                    library_name=library_name[0:indexplace]+'/'+library_name[indexplace+1:]
                elif indexplace==-1:
                    break
        # Change the ':'
        while True:
            indexplace=library_name.find(':')
            if indexplace!=-1:
                library_name=library_name[0:indexplace]+'/'+library_name[indexplace+1:]
            elif indexplace==-1:
                break
        try:
            file_list=os.listdir(Dot_Minecraft_Path+'/libraries/'+library_name)
            for i in file_list:
                libraries_detail_path_list.append('libraries/'+library_name+'/'+i)
        except WindowsError:
            sys.stderr.write('library file is missing: '+Dot_Minecraft_Path+'/libraries/'+library_name+' . Minecraft may not work!'+'\n')
    return libraries_detail_path_list

def assets_list_analyzer(Dot_Minecraft_Path,Full_version_dict,selected_version):
    version_dict=Full_version_dict[selected_version]
    hash_list=[]
    assets_list=[]

    if version_dict.has_key('assetIndex'):
        assetIndex=version_dict['assetIndex']
        if os.path.isdir(Dot_Minecraft_Path+'/assets/indexes'):
            for i in os.listdir(Dot_Minecraft_Path+'/assets/indexes'):
                if assetIndex==os.path.splitext(os.path.basename(i))[0]:
                    assets_list.append(Dot_Minecraft_Path+'/assets/indexes/'+i)
                    File=open(Dot_Minecraft_Path+'/assets/indexes/'+i,'r+')
                    asset_list=json.load(File)
                    if asset_list.has_key('objects'):
                        asset_list=asset_list['objects']
                        for keys in asset_list.keys():
                            hash=asset_list[keys]['hash']
                            hash_list.append(hash)

    for i in hash_list:
        if os.path.isfile(Dot_Minecraft_Path+'/assets/objects/'+i[0:2]+'/'+i):
            assets_list.append(Dot_Minecraft_Path+'/assets/objects/'+i[0:2]+'/'+i)

    return assets_list

def calculate_file_to_zip(Dot_Minecraft_Path,Full_version_dict,selected_version):
    version_dict=Full_version_dict[selected_version]

    file_list={}
    folder_list={}
    # Jar file and native folder
    file_list['others']=[version_dict['Jar_file']]
    if version_dict['Native_files_folder']:
        folder_list['natives']=version_dict['Native_files_folder']

    # Mod file
    folder_list['mods']=Dot_Minecraft_Path+'/mods'

    # Libraries file
    file_list['libraries']=[]
    for i in version_dict['libraries_path']:
        file_list['libraries'].append(Dot_Minecraft_Path+'/'+i)

    # Assets file
    file_list['assets']=assets_list_analyzer(Dot_Minecraft_Path,Full_version_dict,selected_version)

    # Scripts file
    folder_list['scripts']=Dot_Minecraft_Path+'/scripts'

    # Root path file
    for i in os.listdir(Dot_Minecraft_Path):
        if os.path.isfile(Dot_Minecraft_Path+'/'+i):
            file_list['others'].append(Dot_Minecraft_Path+'/'+i)

    # Config file
    folder_list['config']=Dot_Minecraft_Path+'/config'

    return (file_list,folder_list)

def json_dump(upload_path,Dot_Minecraft_Path,Full_version_dict,selected_version,entirePackageFiles):
    Templete=json.load(open(os.path.split(Dot_Minecraft_Path)[0]+'/'+'Example.json','r'))

    json_structure=Templete.copy()
    startupArguments=json_structure['startupArguments']
    startupArguments['libraryPaths']=Full_version_dict[selected_version]['libraries_path']
    startupArguments['nativespath']='natives'
    startupArguments['mainJarPath']=Full_version_dict[selected_version]['Jar_file'][len(Dot_Minecraft_Path)+1:]
    startupArguments['mainClass']=Full_version_dict[selected_version]['mainClass']
    startupArguments['version']=Full_version_dict[selected_version]['version']
    startupArguments['assetsDir']='assets'
    startupArguments['assetIndex']=Full_version_dict[selected_version]['assetIndex']
    startupArguments['tweakClasses']=Full_version_dict[selected_version]['tweakClasses']

    json_structure['fileSystem']['entirePackageFiles']=entirePackageFiles

    json_structure['startupArguments']=startupArguments

    #os.mknod(upload_path+json_structure['instanceName']+'.json')
    if not json_structure['instanceName']:
        json_file=open(upload_path+'/'+selected_version+'.json','w+')
    else:
        json_file=open(upload_path+'/'+json_structure['instanceName']+'.json','w+')
    json_file.write(json.dumps(json_structure, indent=4))
    json_file.close()
    return json_structure

def zipping(Dot_Minecraft_Path,destination,file_and_folder_tuple):
    file_dict,folder_dict=file_and_folder_tuple
    entirePackageFiles=[]
    zip_file_path=[]
    section_sample={'downloadLink':'','md5':'','name':'','localPath':''}
    threadLock=threading.Lock()

    def file_processing(threadID,Dot_Minecraft_Path,destination,file_name,file_or_list,folder_exclude_in_zip=False):
        temp=section_sample.copy()
        return_value=zip_file(threadID,Dot_Minecraft_Path,destination,file_name,file_or_list,folder_exclude_in_zip)
        if return_value!=1:
            if file_name=='others.zip':
                temp['localPath']=''
            else:
                temp['localPath']=os.path.splitext(file_name)[0]+'/'
            temp['name']=os.path.splitext(file_name)[0]
            temp['md5']=return_value[1]
            threadLock.acquire()
            entirePackageFiles.append(temp)
            zip_file_path.append(destination+'/'+file_name)
            threadLock.release()
        print 'Thread-{} exiting! Result: {}'.format(threadID,return_value)

    def folder_processing(threadID,destination,file_name,folder_location):
        temp=section_sample.copy()
        return_value=zip_folder(threadID,destination,file_name,folder_location)
        if return_value!=1:
            temp['localPath']=os.path.splitext(file_name)[0]+'/'
            temp['name']=os.path.splitext(file_name)[0]
            temp['md5']=return_value[1]
            threadLock.acquire()
            entirePackageFiles.append(temp)
            zip_file_path.append(destination+'/'+file_name)
            threadLock.release()
        print 'Thread-{} exiting! Result: {}'.format(threadID,return_value)

    objects={}
    threadID=1

    for keys in file_dict.keys():
        if keys!='others':
            objects[keys]=Thread(target=file_processing,args=(threadID,Dot_Minecraft_Path, destination, keys+'.zip',file_dict[keys],True))
        elif keys=='others':
            objects[keys]=Thread(target=file_processing, args=(threadID,Dot_Minecraft_Path, destination, keys+'.zip',file_dict[keys]))
        threadID+=1
    for keys in folder_dict.keys():
        objects[keys]=Thread(target=folder_processing, args=(threadID,destination,keys+'.zip',folder_dict[keys]))
        threadID+=1
    for object in objects.keys():
        objects[object].start()
     
    for object in objects.keys():
        objects[object].join()

    return (entirePackageFiles,zip_file_path)

class ftp_upload(threading.Thread):
    def __init__(self,address,port=21,username=None, password=None,local_path='',upload_path=''):
        print 'Initializing upload sdks...'
        from upload_sdk.ftp import ftp
        self.ftp=ftp
        print 'Initialized!'
        threading.Thread.__init__(self)

        self.__address=address
        self.__port=port
        self.__username=username
        self.__password=password
        self.__local_path=local_path
        self.__upload_path=upload_path

    def Ftp(self):
        '''A simplfy function for uploading files through ftp
        Arguments: address, username & password (default is None), local_path, upload_path (default is /)
        '''
        if not self.__port:
            self.__port=21         # Related to the problem that if port = '', ftp will raise socket error. Link: http://www.programgo.com/article/14342614841/
        ftp_object=self.ftp
        ftp_object=ftp_object.ftp_upload(self.__port, self.__address,local_path=self.__local_path,upload_path=self.__upload_path)
        __download_addr=ftp_object.upload()
        return __download_addr
    def run(self):
        __download_addr=self.Ftp()
        download_address[os.path.split(self.__local_path)[1]]=__download_addr
        print 'Job {} Done!'.format(os.path.split(self.__local_path)[1])
class upyun_upload():
    def __init__(self):
        print 'Initializing upload sdks...'
        from upload_sdk import upyun
        self.upyun=upyun
        print 'Initialized!'

    def Upyun(self, bucket, domain='.b0.upaiyun.com', username=None, password=None, secret=None, local_path='', upload_directory=''):
        '''Upload function for Upyun
        Arguments: 
            bucket: the service name that you are going to upload to. 
            username & password: the OPERATOR username and password
            secret: (optional) A bunch of keys allow the clinet directly transfer file to the service. 
            local_path & upload_path
        '''
        if not upload_directory:
            upload_path='/'+os.path.split(local_path)[1]
        elif upload_directory:
            upload_path=upload_directory+'/'+os.path.split(local_path)[1]

        upyun_object=self.upyun
        try:
            upyun_object=upyun_object.UpYun(bucket, username, password, secret)
            from upload_sdk.upyun import print_reporter
            if secret:
                print 'Begin upload with multipart: '+local_path
                with open(local_path, 'rb') as __file:
                    result=upyun_object.put(upload_path, __file, checksum=True, multipart=True, block_size=1024*1024, reporter=print_reporter, params='Uploading ')
                    print 'Success upload {}'.format(os.path.split(local_path)[1])
                    return (result,bucket+domain+upload_path)
            elif not secret:
                from upload_sdk.upyun import FileStore
                print 'Begin upload: '+local_path
                with open(local_path, 'rb') as __file:
                    result=upyun_object.put(upload_path, __file, checksum=True, need_resume=True,store=FileStore(),reporter=print_reporter, params='Uploading ')
                    print 'Success upload {}'.format(os.path.split(local_path)[1])
                    return (result,bucket+domain+upload_path)
        except self.upyun.UpYunServiceException as se:
            print 'Except an UpYunServiceException ...'
            print 'Request Id: ' + se.request_id
            print 'HTTP Status Code: ' + str(se.status)
            print 'Error Message:    ' + se.msg + '\n'
        except self.upyun.UpYunClientException as ce:
            print 'Except an UpYunClientException ...'
            print 'Error Message: ' + ce.msg + '\n'

    def run(self, bucket, domain='.b0.upaiyun.com', username=None, password=None, secret=None, local_path='', upload_path=''):
        self.Upyun(bucket, domain, username, password, secret, local_path, upload_path)

def upload_section(entirePackageFiles, zip_file_path):
    modules=['ftp','upyun']
    objects=[]
    lock=threading.Lock()
    for letter,module in zip(string.ascii_lowercase[:len(modules)],modules):
        print letter+': '+module
    selection=raw_input('Please select platform: ')
    match=False
    for letter,module in zip(string.ascii_lowercase[:len(modules)],modules):
        if selection==letter:
            selection=module
            match=True
    if not match:
        print 'Incorrect input. Please enter another option!'
        entirePackageFiles=upload_section(entirePackageFiles, zip_file_path)
    elif match:
        if selection==modules[0]:
            address=raw_input('Please enter the domain/ip address: ')
            port=raw_input('Please enter the port (default: 21): ')
            if port:
                port=int(port)
            username=raw_input('Please enter the username (if annoymous, ingore it): ')
            if username:
                password=raw_input('Please enter the password: ')
            elif not username:
                username=None
                password=None
            upload_path=raw_input('Upload to which directory (default: /)? ')
            global download_address
            download_address={}
            for temp in zip_file_path:
                instance=ftp_upload(address, port, username, password, temp, upload_path+'/'+os.path.split(temp)[1])
                instance.start()
                objects.append(instance)
            for temp in objects:
                temp.join()
            for keys in download_address:
                for dicts,index in zip(entirePackageFiles,range(len(entirePackageFiles))):
                    if dicts['name']==os.path.splitext(keys)[0]:
                        entirePackageFiles[index]['downloadLink']=download_address[keys]
        if selection==modules[1]:
            bucket=raw_input('Please enter the service name: ')
            domain=raw_input('Please enter the domain (default: .b0.upaiyun.com)')
            username=raw_input('Please enter the operator name: ')
            password=raw_input('Please enter the password: ')
            secret=raw_input('Enter the form key if available: ')
            upload_path=raw_input('Upload to which directory (default: /)? ')
            for temp in zip_file_path:
                instance=upyun_upload()
                try:
                    result=instance.Upyun(bucket, domain, username, password, secret, temp, upload_path)
                    for dicts,index in zip(entirePackageFiles,range(len(entirePackageFiles))):
                        if dicts['name']==os.path.splitext(os.path.split(temp)[1])[0]:
                            entirePackageFiles[index]['downloadLink']=result[1]
                except:
                    print 'Exception caught from Job {}! Pass...'.format(temp)
                    continue
    return entirePackageFiles

def pause():
    input('Press [Enter] to continue...')


def test():
    Dot_Minecraft_Path='D:/MC/NUK3TOWN/.minecraft'
    if not os.path.isdir('D:/MC/NUK3TOWN/upload'):
        os.mkdir('D:/MC/NUK3TOWN/upload')
    dict_a=Minecraft_directory_search(Dot_Minecraft_Path)
    selection_list=zip([a for a in string.lowercase[:len(dict_a.keys())]],dict_a.keys())
    for option, version in selection_list:
        print option+': '+version
    select=raw_input('Select version: ')
    found_version=False
    for option, version in selection_list:
        if select==option:
            select=version
            found_version=True
            break
        else:
            continue
    if not found_version: 
        print 'Your option is wrong! Please restart the program！'
        return -1
    dict_a=libraries_search(Dot_Minecraft_Path,select,dict_a)
    file_tuple=calculate_file_to_zip(Dot_Minecraft_Path,dict_a,select)
    entirePackageFiles=zipping(Dot_Minecraft_Path,'D:/MC/NUK3TOWN/upload',file_tuple)
    json_dump('D:/MC/NUK3TOWN/upload',Dot_Minecraft_Path,dict_a,select,entirePackageFiles)

def main():
    Dot_Minecraft_Path=os.getcwd()+'/.minecraft'
    upload_path=os.getcwd()+'/upload'
    if not os.path.isdir(upload_path):
        os.mkdir(upload_path)
    dict_a=Minecraft_directory_search(Dot_Minecraft_Path)

    selection_list=zip([a for a in string.lowercase[:len(dict_a.keys())]],dict_a.keys())
    for option, version in selection_list:
        print option+': '+version
    select=raw_input('Select version: ')
    found_version=False
    for option, version in selection_list:
        if select==option:
            select=version
            found_version=True
            break
        else:
            continue
    if not found_version: 
        print 'Your option is wrong! Please restart the program！'
        return -1
    dict_a=libraries_search(Dot_Minecraft_Path,select,dict_a)
    file_and_tuple=calculate_file_to_zip(Dot_Minecraft_Path,dict_a,select)
    entirePackageFiles,zip_file_path=zipping(Dot_Minecraft_Path,upload_path,file_and_tuple)
    entirePackageFiles=upload_section(entirePackageFiles,zip_file_path)
    json_dump(upload_path,Dot_Minecraft_Path,dict_a,select,entirePackageFiles)
    print 'Complete!'
    pause()

main()
#print upload_section([{'name':'ll'}],['E:/ll.mp4'])