namespace Qin.CsvRelevant
{
    public interface IZipOperation
    {
        public bool ZipDirectory(string folderToZip, string zipedFile, string password);
        public bool ZipDirectory(string folderToZip, string zipedFile);
        public bool ZipFile(string fileToZip, string zipedFile, string password);
        public bool ZipFileByGb2312(string fileToZip, string zipedFile, string password);
        public bool ZipFile(string fileToZip, string zipedFile);
        public bool Zip(string fileToZip, string zipedFile, string password);
        public bool Zip(string fileToZip, string zipedFile);


        public bool UnZip(string fileToUnZip, string zipedFolder, string password);
        public bool UnZip(string fileToUnZip, string zipedFolder);
    }
}
