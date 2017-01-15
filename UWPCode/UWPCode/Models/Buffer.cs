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
        private string text;
        private bool isSaved;
        private StorageFile file;

        public Buffer()
        {
            name = "";
            text = "";
            isSaved = false;
            file = null;
        }


        public static async Task<Buffer> CreateBufferFromFileAsync(StorageFile file)
        {
            var buffer = new Buffer
            {
                Name = file.Name,
                Text = await FileIO.ReadTextAsync(file),
                isSaved = true,
            };
            return buffer;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        public bool IsSaved
        {
            get
            {
                return isSaved;
            }
        }

        public bool IsInFileSystem
        {
            get
            {
                return file != null;
            }
        }

        public StorageFile File
        {
            get
            {
                return file;
            }

            set
            {
                file = value;
            }
        }

        internal async Task<StorageFile> SaveFile()
        {
            CachedFileManager.DeferUpdates(file);
            name = file.Name;
            isSaved = true;
            await FileIO.WriteTextAsync(file, text);
            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            // TODO: code to notify when cannot save file
            return file;
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


