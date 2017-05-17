using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;

namespace TerminologyLauncher.InstanceManagerSystem
{
    public sealed class InstanceCreator
    {
        public InstanceEntity CrtatedInstance { get; private set; }

        public String InstanceJson => Utils.SerializeUtils.JsonConverter.ConvertToJson(this.CrtatedInstance);

        public InstanceCreator()
        {
            this.Initallize();
        }

        public void Initallize()
        {
            this.CrtatedInstance = new InstanceEntity();
        }

        public void Initallize(InstanceEntity template)
        {
            this.Initallize();
            this.CrtatedInstance = template;
        }

        public void AddPackage(string name, string packFile, string instanceWideLocation, string downloadUrl)
        {
            if (string.IsNullOrEmpty(packFile) || !File.Exists(packFile))
            {
                throw new FileNotFoundException($"Pack file {packFile} is not found!");
            }

            if (!Path.GetExtension(packFile).Equals("zip"))
            {
                throw new NotSupportedException("Package system not support file except zip!");
            }

            if (this.CrtatedInstance.FileSystem.EntirePackageFiles.Any(package => package.Name.Equals(name)))
            {
                throw new InvalidOperationException($"Package name {name} already exists in current package!");
            }

            var md5 = Utils.EncodeUtils.CalculateFileMd5(packFile);

            var packObj = new EntirePackageFileEntity()
            {
                DownloadLink = downloadUrl,
                LocalPath = instanceWideLocation,
                Md5 = md5,
                Name = name
            };

            this.CrtatedInstance.FileSystem.EntirePackageFiles.Add(packObj);
        }

        public void AddOfficialFile(string name, string id)
        {
            throw new NotImplementedException();
        }

        public void AddCustomerFile(string name, string file, string instanceWideLocation, string downloadUrl)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                throw new FileNotFoundException($"Pack file {file} is not found!");
            }


            if (this.CrtatedInstance.FileSystem.CustomFiles.Any(cFile => cFile.Name.Equals(name)))
            {
                throw new InvalidOperationException($"Package name {name} already exists in current package!");
            }

            var md5 = Utils.EncodeUtils.CalculateFileMd5(file);

            var customerFileObj = new CustomFileEntity()
            {
                DownloadLink = downloadUrl,
                LocalPath = instanceWideLocation,
                Md5 = md5,
                Name = name
            };

            this.CrtatedInstance.FileSystem.CustomFiles.Add(customerFileObj);
        }
    }
}