import ftplib

class ftp_upload(ftplib.FTP):
    def __init__(self,address=None,username=None,password=None,path=None):
        ftplib.FTP.__init__(self,address,username,password)
        self.__path=path

    def upload(self):
        if not self.__path:
            print 'No path specific!'
            return -1
        self.login()
        print self.__path

objects=ftp_upload(address='192.168.1.131',path='/a')

objects.upload()

objects.retrlines('LIST')

