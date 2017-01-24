using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace UWPCode.Models
{
    public class Buffer
    {
        private string name;
        private bool isSaved;
        private StorageFile file;

        public Buffer()
        {
            name = "";
            Text = "";
            isSaved = false;
            file = null;
        }

        public Buffer(string name)
        {
            this.name = name;
            Text = "";
            isSaved = false;
            file = null;
        }

        public static async Task<Buffer> CreateBufferFromFileAsync(StorageFile file)
        {
            var buffer = new Buffer
            {
                name = file.Name,
                Text = await FileIO.ReadTextAsync(file),
                isSaved = true,
            };
            return buffer;
        }

        public string Name => IsInFileSystem ? file.Name : name;

        public string Text { get; set; }

        public bool IsSaved => isSaved;

        public bool IsInFileSystem => file != null;

        public StorageFile File => file;

        internal async Task<StorageFile> SaveFile()
        {
            CachedFileManager.DeferUpdates(file);
            name = file.Name;
            isSaved = true;
            await FileIO.WriteTextAsync(file, Text);
            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // TODO: code to notify when cannot save file
            return file;
        }

        internal async Task<StorageFile> SaveFile(StorageFile file)
        {
            this.file = file;
            return await SaveFile();
        }
    }


    class BufferComparer : EqualityComparer<Buffer>
    {
        public override bool Equals(Buffer x, Buffer y)
        {
            if (x.IsInFileSystem && y.IsInFileSystem)
                return x.File.Equals(y.File);
            else if (x.IsInFileSystem || y.IsInFileSystem)
                return false;
            else if (!x.Name.Equals(y.Name) || !x.Text.Equals(y.Text))
                return false;
            else return EqualityComparer<Buffer>.Default.Equals(x, y);
        }

        public override int GetHashCode(Buffer obj)
        {
            if (obj.IsInFileSystem)
                return EqualityComparer<StorageFile>.Default.GetHashCode();
            else
                return EqualityComparer<Buffer>.Default.GetHashCode();
        }
    }
}


