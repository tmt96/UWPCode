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

        public bool IsInFileSystem => File != null;

        public string Path => File != null ? File.Path : "";

        protected StorageFile File { get; set; }

        public Buffer(string name = "")
        {
            Name = name;
            Text = "";
            IsSaved = false;
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
            CachedFileManager.DeferUpdates(File);
            await FileIO.WriteTextAsync(File, Text);
            var status = await CachedFileManager.CompleteUpdatesAsync(File);
            // TODO: code to notify when cannot save file
            UpdateBuffer(File);
            return File;
        }

        internal async Task<StorageFile> SaveFile(StorageFile file)
        {
            File = file;
            return await SaveFile();
        }

        private void UpdateBuffer(StorageFile file)
        {
            File = file;
            IsSaved = true;
            Name = File.Name;
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
                    return Default.GetHashCode();
            }
        }
    }
}


