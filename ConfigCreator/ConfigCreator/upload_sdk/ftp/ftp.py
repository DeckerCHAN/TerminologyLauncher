import ftplib,os

class ftp_upload(ftplib.FTP):
    def __init__(self,port=21,address=None,username=None,password=None,local_path='',upload_path='',passive=True):
        ftplib.FTP.__init__(self)
        self.__port=port
        self.__address=address
        self.__username=username
        self.__password=password
        self.__local_path=local_path
        if not upload_path:
            self.__upload_path='/'+os.path.split(local_path)[1]
        else:
            self.__upload_path=upload_path
        self.__passive=passive

    def upload(self):
        if not self.__local_path:
            print 'No path specific!'
            return -1
        self.connect(self.__address,self.__port)
        self.login(self.__username,self.__password)
        self.cwd('/')
        if os.path.dirname(self.__upload_path):
            __dirname=os.path.dirname(self.__upload_path)
            try:
                self.cwd(__dirname)
            except:
                self.mkd(__dirname)
                self.cwd(__dirname)
        try:
            self.size(self.__upload_path)
            lock.acquire()
            __delete_permission=raw_input('File {} may exist on the ftp server. Delete it? (y/n) '.format(self.__upload_path))
            lock.release()
            if __delete_permission:
                self.delete(self.__upload_path)
        except:
            pass
        __File=open(self.__local_path,'rb')
        if self.__passive:
            self.set_pasv(True)
        self.storbinary('STOR {}'.format(os.path.split(self.__upload_path)[1]),__File)
        return 'ftp://'+self.__address+':'+str(self.__port)+self.__upload_path


