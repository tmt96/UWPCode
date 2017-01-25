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

        public string Name { get; private set; }

        public string Text { get; set; }

        public bool IsSaved { get; set; }

        public bool IsInFileSystem { get; private set; }

        public string Path { get; private set; }

        public Buffer(string name = "")
        {
            Name = name;
            Text = "";
            IsSaved = false;
            Path = "";
        }

        public static async Task<Buffer> CreateBufferFromFileAsync(StorageFile file)
        {
            var buffer = new Buffer
            {
                Text = await FileIO.ReadTextAsync(file),
            };
            buffer.UpdateBuffer(file);
            return buffer;
        }


        internal async Task<StorageFile> SaveFile()
        {
            var file = await StorageFile.GetFileFromPathAsync(Path);
            if (file != null) return await SaveFile(file);
            return file;
        }

        internal async Task<StorageFile> SaveFile(StorageFile file)
        {
            CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file, Text);
            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // TODO: code to notify when cannot save file
            UpdateBuffer(file);
            return file;
        }

        private void UpdateBuffer(StorageFile file)
        {
            Name = file.Name;
            Path = file.Path;
            IsSaved = true;
            IsInFileSystem = true;
        }
    }


    class BufferComparer : EqualityComparer<Buffer>
    {
        public override bool Equals(Buffer x, Buffer y)
        {
            if (x.IsInFileSystem && y.IsInFileSystem)
                return x.Path.Equals(y.Path);
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


